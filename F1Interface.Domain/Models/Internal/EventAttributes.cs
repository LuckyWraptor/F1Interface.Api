using System.Text.Json.Serialization;

namespace F1Interface.Domain.Models.Internal
{
    internal class EventAttributes : MeetingAttributes
    {
        [JsonPropertyName("CircuitKey")]
        public uint CircuitKey { get; set; }
        [JsonPropertyName("Circuit_Location")]
        public string CircuitLocation { get; set; }
        [JsonPropertyName("Circuit_Official_Name")]
        public string CircuitOfficialName { get; set; }
        [JsonPropertyName("Circuit_Short_Name")]
        public string CircuitShortName { get; set; }
        public bool IsTestEvent { get; set; }
    }
}