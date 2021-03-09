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
            public const string Api = "https://f1tv.formula1.com/2.0/R/ENG/WEB_HLS/";
            public const string Suffix = "/F1_TV_Pro_Annual/12";
            public const string SearchEndpoint = Api + "ALL/PAGE/SEARCH/VOD" + Suffix;
            public const string ContentEndpoint = Api + "ALL/CONTENT/VIDEO/{{CONTENT_ID}}" + Suffix;
            public const string PlaybackEndpoint = Api + "ALL/CONTENT/PLAY?contentId={{CONTENT_ID}}";
        }
    }
}