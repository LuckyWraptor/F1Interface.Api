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
        public static class F1TV
        {
            public const string Api = "https://f1tv.formula1.com/2.0/A/ENG/WEB_HLS/";
            public const string SearchEndpoint = Api + "ALL/PAGE/SEARCH/VOD/F1_TV_Pro_Annual/2";
            public const string ContentEndpoint = Api + "ALL/CONTENT/VIDEO/{{CONTENT_ID}}/F1TV_Pro_Annual/2";
        }
    }
}