namespace F1Interface.Domain
{
    public static class Constants
    {
        internal static readonly string[] UserAgents = new string[]
        {
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 11_0_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36"
        };

        public static class QueryParameters
        {
            public const string ChannelId = "channelId";
            public const string ContentId = "contentId";
            public const string FilterByEvent = "filter_MeetingKey";
            public const string FilterBySeason = "filter_season";
            public const string FilterBySeries = "filter_Series";
            public const string FilterByType = "filter_objectSubtype";
            public const string FilterOrderDate = "filter_orderByFom";
            public const string FilterPastEvent = "filter_meetingsPast";
            public const string FilterUpcomingEvent = "filter_meetingsUpcoming";
            public const string FilterFetchAll = "filter_fetchAll";
            public const string FilterByYear = "filter_year";
            public const string OrderBy = "orderBy";
            public const string SortOrder = "sortOrder";
        }

        public static class Categories
        {
            public const string F1 = "FORMULA 1";
            public const string F2 = "FORMULA 2";
            public const string F3 = "FORMULA 3";
            public const string PorscheSupercup = "PORSCHE";

            public static readonly string[] KnownCategories = new[]
            {
                F1, F2, F3, PorscheSupercup
            };
        }
    }
}