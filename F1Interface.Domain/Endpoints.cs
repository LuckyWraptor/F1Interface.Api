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


        internal static class F1TV
        {
            /// <summary>
            /// API endpoint for the F1TV Service
            /// </summary>
            public const string Api = "https://f1tv-api.formula1.com/agl/1.0/unk/en/all_devices/global/";
            /// <summary>
            /// F1TV authentication token.
            /// </summary>
            public const string AuthenticateToken = Api + "authenticate";
        }
    }
}