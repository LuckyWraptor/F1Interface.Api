using F1Interface.Domain.Models;

namespace F1Interface
{
    public class ContentParser
    {
        public static ContentType DetermineType(string contentSubType)
        {
            if (string.IsNullOrWhiteSpace(contentSubType))
            {
                return ContentType.Unspecified;
            }

            return contentSubType.ToUpper() switch
            {
                "ANALYSIS" or "1" => ContentType.Analysis,
                "DOCUMENTARY" or "2" => ContentType.Documentary,
                "FEATURE" or "3" => ContentType.Feature,
                "HIGHLIGHTS" or "4" => ContentType.Highlights,
                "MEETING" or "5" => ContentType.Meeting,
                "PRESS CONFERENCE" or "6" => ContentType.PressConference,
                "REPLAY" or "7" => ContentType.Replay,
                "SHOW" or "8" => ContentType.Show,
                "LIVE" or "9" => ContentType.Live,
                _ => ContentType.Unspecified
            };
        }

        public static string TypeToString(ContentType type)
            => type switch
            {
                ContentType.Analysis => "Analysis",
                ContentType.Documentary => "Documentary",
                ContentType.Feature => "Feature",
                ContentType.Highlights => "Highlights",
                ContentType.Meeting => "Meeting",
                ContentType.PressConference => "Press Conference",
                ContentType.Replay => "Replay",
                ContentType.Show => "Show",
                ContentType.Live => "Live",
                _ => null
            };
    }
}