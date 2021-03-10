using System;
using System.Text.Json.Serialization;

namespace F1Interface.Domain.Models.Internal
{
    internal class MeetingAttributes
    {
        public uint MeetingId { get; private set; }
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
        public DateTime MeetingEnds { get; set; }
        public DateTime MeetingStarts { get; set; }



        [JsonPropertyName("Meeting_Start_Date")]
        public string MeetingStartDate
        {
            get => null;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && DateTime.TryParse(value, out DateTime dt))
                {
                    MeetingStarts = dt;
                }
            }
        }
        [JsonPropertyName("Meeting_End_Date")]
        public string MeetingEndDate
        {
            get => null;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && DateTime.TryParse(value, out DateTime dt))
                {
                    MeetingEnds = dt;
                }
            }
        }

        public string MeetingKey
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && uint.TryParse(value, out uint key))
                {
                    MeetingId = key;
                }
            }
        }
    }
}