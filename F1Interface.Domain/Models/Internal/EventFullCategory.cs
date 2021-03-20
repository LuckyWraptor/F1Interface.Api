using System.Text.Json.Serialization;

namespace F1Interface.Domain.Models.Internal
{
    internal class EventFullCategory
    {
        [JsonPropertyName("eventName")]
        public string Name { get; set; }
        public Event[] Events { get; set; }
    }
}