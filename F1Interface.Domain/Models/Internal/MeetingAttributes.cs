using System;
using System.Text.Json.Serialization;

namespace F1Interface.Domain.Models.Internal
{
    internal class MeetingAttributes
    {
        public string MeetingKey { get; set; }
        public string MeetingSessionKey { get; set; }
        [JsonPropertyName("Meeting_Name")]
        public string MeetingName { get; set; }
        [JsonPropertyName("Meeting_Official_Name")]
        public string MeetingOfficialName { get; set; }
        [JsonPropertyName("MeetingCountryKey")]
        public string MeetingCountryKey { get; set; }
        [JsonPropertyName("Meeting_Location")]
        public string MeetingLocation { get; set; }
        [JsonPropertyName("Meeting_Country_Name")]
        public string MeetingCountryName { get; set; }
        [JsonPropertyName("Meeting_Sponsor")]
        public string MeetingSponsor { get; set; }
        [JsonPropertyName("Meeting_End_Date")]
        public DateTime MeetingEnds { get; set; }
        [JsonPropertyName("Meeting_Start_Date")]
        public DateTime MeetingStarts { get; set; }
    }
}