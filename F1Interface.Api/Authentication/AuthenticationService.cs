using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using PlaywrightSharp;

using F1Interface.Api.Authentication.Contracts;
using F1Interface.Api.Authentication.Models;
using F1Interface.Api.Constants;
using F1Interface.Api.Generic;
using F1Interface.Api.Util;
using F1Interface.Api.Generic.Models;

namespace F1Interface.Api.Authentication
{
    public class AuthenticationService : ServiceBase, IAuthenticationService
    {
        public const string IdentityProvider = "/api/identity-providers/iden_732298a17f9c458890a1877880d140f3/";

        private const string LoginSelector = "#loginform input[type=text].txtLogin";
        private const string PasswordSelector = "#loginform input[type=password].txtPassword";

        private const string LoginButtonSelector = "#loginform button[type=submit]";
        private const string ConsentButtonSelector = "button#truste-consent-button";

        private readonly Random _random;

        public AuthenticationService(IBrowser apiBrowser, ILogger<AuthenticationService> logger)
            : base(apiBrowser, logger)
        {
            _random = new Random();
        }

        /// <summary>
        /// Authenticate against the F1 login page.
        /// </summary>
        /// <param name="login">Username/Email to authenticate with</param>
        /// <param name="password">Password to authenticate with</param>
        /// <param name="cancellationToken">Possible cancellation request token</param>
        /// <returns>Authentication response values</returns>
        /// <exception cref="AuthenticationException">Authentication failure exception</exception>
        public async Task<AuthenticationResponse> AuthenticateAsync(string login, string password, CancellationToken cancellationToken = default)
        {
#if DEBUG
            _logger.LogTrace("Creating new page for authentication");
#endif
            IPage page = await BrowserUtils.NewPageAsync(_browser, _random);
#if DEBUG
            _logger.LogDebug("Created new page for authentication");
#endif
            AuthenticationResponse response;
            try
            {
                response = await AuthenticateAsync(page, login, password, cancellationToken)
                            .ConfigureAwait(false);
            }
            finally
            {
#if DEBUG
                _logger.LogTrace("Closing single-use page after authentication");
#endif
                await page.CloseAsync()
                            .ConfigureAwait(false);
#if DEBUG
                _logger.LogDebug("Closed single-use page after authentication");
#endif
            }

            return response;
        }
        /// <summary>
        /// Authenticate against the F1 login page.
        /// </summary>
        /// <param name="page">Page to login through.</param>
        /// <param name="login">Username/Email to authenticate with</param>
        /// <param name="password">Password to authenticate with</param>
        /// <param name="cancellationToken">Possible cancellation request token</param>
        /// <returns>Authentication response values</returns>
        /// <exception cref="AuthenticationException">Authentication failure exception</exception>
        public async Task<AuthenticationResponse> AuthenticateAsync(IPage page, string login, string password, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Navigating to {Endpoint} to authenticate {User}", Endpoints.LoginPage, login);
            await page.GoToAsync(Endpoints.LoginPage, LifecycleEvent.DOMContentLoaded)
                           .ConfigureAwait(false);
#if DEBUG
            _logger.LogTrace("Navigated to page {Endpoint} to authenticate {User}", Endpoints.LoginPage, login);
#endif
            await TaskUtils.RandomDelay(_random, 200, 1500, cancellationToken);
            await PageUtils.ClickAsync(page, ConsentButtonSelector, _random);

            await TaskUtils.RandomDelay(_random, cancellationToken);
            await PageUtils.TypeAsync(page, LoginSelector, login, 50, 300, _random, cancellationToken)
                        .ConfigureAwait(false);

            await TaskUtils.RandomDelay(_random, cancellationToken);
            await PageUtils.TypeAsync(page, PasswordSelector, password, 50, 300, _random, cancellationToken)
                        .ConfigureAwait(false);

            await TaskUtils.RandomDelay(_random, cancellationToken);
            Task<IResponse> responseTask = page.WaitForResponseAsync(Endpoints.AuthenticationByPassword);
            Task clickTask = PageUtils.ClickAsync(page, LoginButtonSelector, _random);
#if DEBUG
            _logger.LogTrace("Awaiting click & response");
#endif

            Task.WaitAll(responseTask, clickTask);
#if DEBUG
            _logger.LogDebug("Response received for {Endpoint}", Endpoints.AuthenticationByPassword);
#endif

            switch (responseTask.Result.Status)
            {
                case HttpStatusCode.OK:
                    AuthenticationResponse response = await responseTask.Result.GetJsonAsync<AuthenticationResponse>()
                                .ConfigureAwait(false);
                    if (response != null && response.Session != null && !string.IsNullOrWhiteSpace(response.Session.Token))
                    {
                        _logger.LogInformation("Authenticated {User} successfully.", login);
                        return response;
                    }
                    throw new AuthenticationException("Authentication failed as an invalid/incomprehensive response was retrieved.", HttpStatusCode.Forbidden);
                case HttpStatusCode.Unauthorized:
                    throw new AuthenticationException("Invalid credentials provided.", responseTask.Result.Status);
            }
            throw new ApiException("Unhandled response (this shouldn't happen!)");
        }
        /// <summary>
        /// Authenticate the sessionToken to retrieve F1TV authorization
        /// </summary>
        /// <param name="sessionToken">Session token provided by the AuthenticateResponse</param>
        /// <param name="identityProvider">IdentityProvider (defaults to normal provider)</param>
        /// <returns></returns>
        public async Task<AuthenticationToken> AuthenticateF1TVTokenAsync(string sessionToken, string identityProvider = default)
        {
#if DEBUG
            _logger.LogTrace("Creating new page for F1TV token authentication");
#endif
            IPage page = await BrowserUtils.NewPageAsync(_browser, _random)
                        .ConfigureAwait(false);
#if DEBUG
            _logger.LogTrace("Created new page for F1TV token authentication");
#endif
            AuthenticationToken response;
            try
            {
                response = await AuthenticateF1TVTokenAsync(page, sessionToken, identityProvider)
                            .ConfigureAwait(false);
            }
            finally
            {
#if DEBUG
                _logger.LogTrace("Closing single-use page after authentication");
#endif
                await page.CloseAsync()
                            .ConfigureAwait(false);
#if DEBUG
                _logger.LogTrace("Closed single-use page after authentication");
#endif
            }

            return response;
        }
        /// <summary>
        /// Authenticate the sessionToken to retrieve F1TV authorization
        /// </summary>
        /// <param name="page">Session token provided by the AuthenticateResponse</param>
        /// <param name="sessionToken">Session token provided by the AuthenticateResponse</param>
        /// <param name="identityProvider">IdentityProvider (defaults to normal provider)</param>
        /// <returns></returns>
        public async Task<AuthenticationToken> AuthenticateF1TVTokenAsync(IPage page, string sessionToken, string identityProvider = default)
        {
            TokenRequest tokenRequest = new TokenRequest
            {
                AccessToken = sessionToken,
                IdentityProviderUrl = (identityProvider ?? IdentityProvider)
            };

            Action<Route, IRequest> routeAction = (route, request) =>
            {
                request.Headers["content-type"] = System.Net.Mime.MediaTypeNames.Application.Json;
                route.ContinueAsync(HttpMethod.Post, JsonSerializer.Serialize(tokenRequest));
            };
            await page.RouteAsync(Endpoints.F1TV.AuthenticateToken, routeAction);
#if DEBUG
            _logger.LogTrace("Rerouted {Endpoint} to POST the TokenRequest instead.", Endpoints.F1TV.AuthenticateToken);
#endif

#if DEBUG
            _logger.LogTrace("Navigating to {Endpoint}.", Endpoints.F1TV.AuthenticateToken);
#endif
            IResponse response = await page.GoToAsync(Endpoints.F1TV.AuthenticateToken, LifecycleEvent.Networkidle, Referers.F1TV)
                        .ConfigureAwait(false);
#if DEBUG
            _logger.LogTrace("Navigated to {Endpoint}", Endpoints.F1TV.AuthenticateToken);
#endif

            await page.UnrouteAsync(Endpoints.F1TV.AuthenticateToken, routeAction);
#if DEBUG
            _logger.LogTrace("Unrouted {Endpoint}", Endpoints.F1TV.AuthenticateToken);
#endif

            switch (response.Status)
            {
                case HttpStatusCode.OK:
                    AuthenticationToken token = await response.GetJsonAsync<AuthenticationToken>();
                    if (token != null && !string.IsNullOrWhiteSpace(token.Token))
                    {
                        _logger.LogInformation("Authenticated F1TVToken successfully");
                        return token;
                    }

                    throw new AuthenticationException("F1TV Token failed as an invalid/incomprehensive response was retrieved.", HttpStatusCode.Forbidden);
                case HttpStatusCode.BadRequest:
                    throw new BadRequestException(await response.GetJsonAsync<BadRequestResponse>());
            }

            throw new ApiException("Unhandled response (this shouldn't happen!)");
        }
    }
}
