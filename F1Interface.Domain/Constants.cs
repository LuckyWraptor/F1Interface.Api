using System.Security.Cryptography;

namespace F1Interface.Domain
{
    public static class Constants
    {
        internal static readonly string[] UserAgents = new string[]
        {
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36",
            "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 11_0_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36"
        };

        public static class QueryParameters
        {
            public const string FilterBySeason = "filter_season";
            public const string FilterByEvent = "filter_MeetingKey";
            public const string FilterByType = "filter_contentSubtype";
            public const string FilterBySeries = "filter_Series";
            public const string FilterPastEvent = "filter_meetingsPast";
            public const string FilterUpcomingEvent = "filter_meetingsUpcoming";
            public const string OrderBy = "orderBy";
            public const string SortOrder = "sortOrder";
        }

        public static class Categories
        {
            public const string F1 = "F1";
            public const string F2 = "F2";
            public const string F3 = "F3";
            public const string PorscheSupercup = "PORSCHE";

            public static readonly string[] KnownCategories = new[]
            {
                F1, F2, F3, PorscheSupercup
            };
        }
    }
}