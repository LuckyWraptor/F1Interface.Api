namespace F1Interface.Domain
{
    internal static class Endpoints
    {
        /// <summary>
        /// Login page url
        /// </summary>
        public const string LoginPage = "https://account.formula1.com/#/en/login";
        /// <summary>
        /// Password authentication endpoint
        /// </summary>
        public const string AuthenticationByPassword = "https://api.formula1.com/v2/account/subscriber/authenticate/by-password";
    }
}