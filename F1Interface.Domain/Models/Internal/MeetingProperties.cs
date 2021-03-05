using System.Text.Json.Serialization;

namespace F1Interface.Domain.Models.Internal
{
    internal class MeetingProperties
    {
        [JsonPropertyName("lastUpdatedDate")]
        public ulong LastUpdated { get; set; }
        [JsonPropertyName("meeting_End_Date")]
        public ulong End { get; set; }
        [JsonPropertyName("meeting_Number")]
        public ulong Id { get; set; }
        [JsonPropertyName("meeting_Start_Date")]
        public ulong Start { get; set; }
        public uint Season { get; set; }
        [JsonPropertyName("season_Meeting_Ordinal")]
        public uint SeasonOrdinalId { get; set; }
        public string Series { get; set; }
        [JsonPropertyName("sessionEndTime")]
        public ulong SessionEnd { get; set; }
        [JsonPropertyName("session_index")]
        public uint SessionIndex { get; set; }
    }
}