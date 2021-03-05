using System.Threading;
using System.Threading.Tasks;
using F1Interface.Domain.Models;
using F1Interface.Domain.Responses;
using PlaywrightSharp;

namespace F1Interface.Contracts
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
    }
}