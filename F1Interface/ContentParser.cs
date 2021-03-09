using F1Interface.Domain.Models;

namespace F1Interface
{
    public class ContentParser
    {
        public static ContentType DetermineType(string contentSubType)
            => contentSubType.ToUpper() switch
            {
                "ANALYSIS" => ContentType.Analysis,
                "DOCUMENTARY" => ContentType.Documentary,
                "FEATURE" => ContentType.Feature,
                "HIGHLIGHTS" => ContentType.Highlights,
                "MEETING" => ContentType.Meeting,
                "PRESS CONFERENCE" => ContentType.PressConference,
                "REPLAY" => ContentType.Replay,
                "SHOW" => ContentType.Show,
                _ => ContentType.Unknown
            };

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
                _ => null
            };
    }
}