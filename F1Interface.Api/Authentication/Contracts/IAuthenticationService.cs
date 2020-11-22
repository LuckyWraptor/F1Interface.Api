using System.Threading;
using System.Threading.Tasks;

using PlaywrightSharp;

using F1Interface.Api.Authentication.Models;

namespace F1Interface.Api.Authentication.Contracts
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Authenticate using login credentials.
        /// </summary>
        /// <param name="login">Customer login identifier (email)</param>
        /// <param name="password">Customer password</param>
        /// <param name="cancellationToken">Request cancellation token</param>
        /// <returns>Authentication response</returns>
        /// <exception cref="AuthenticationException">Authentication failure exception</exception>
        Task<AuthenticationResponse> AuthenticateAsync(string login, string password, CancellationToken cancellationToken = default);
        /// <summary>
        /// Authenticate using login credentials.
        /// </summary>
        /// <param name="page">Browser page to login through</param>
        /// <param name="login">Customer login identifier (email)</param>
        /// <param name="password">Customer password</param>
        /// <param name="cancellationToken">Request cancellation token</param>
        /// <returns>Authentication response</returns>
        /// <exception cref="AuthenticationException">Authentication failure exception</exception>
        Task<AuthenticationResponse> AuthenticateAsync(IPage page, string login, string password, CancellationToken cancellationToken = default);
        /// <summary>
        /// Authenticate token for F1TV access
        /// </summary>
        /// <param name="sessionToken">Access/session token</param>
        /// <param name="identityProvider">Identityprovider for authentication</param>
        /// <param name="cancellationToken">Request cancellation token</param>
        /// <returns>Authentication token response</returns>
        /// <exception cref="AuthenticationException">Authentication failure exception</exception>
        Task<AuthenticationToken> AuthenticateF1TVTokenAsync(string sessionToken, string identityProvider = default);
        /// <summary>
        /// Authenticate token for F1TV access
        /// </summary>
        /// <param name="page">Browser page to authenticate through</param>
        /// <param name="sessionToken">Access/session token</param>
        /// <param name="identityProvider">Identityprovider for authentication</param>
        /// <param name="cancellationToken">Request cancellation token</param>
        /// <returns>Authentication token response</returns>
        /// <exception cref="AuthenticationException">Authentication failure exception</exception>
        Task<AuthenticationToken> AuthenticateF1TVTokenAsync(IPage page, string sessionToken, string identityProvider = default);
    }
}
