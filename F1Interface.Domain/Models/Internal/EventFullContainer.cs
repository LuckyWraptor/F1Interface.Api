using System.Text.Json.Serialization;

namespace F1Interface.Domain.Models.Internal
{
    internal class EventFullContainer
    {
        public string Layout { get; set; }
        [JsonPropertyName("retrieveItems")]
        public EventFullCategoryHolder Items { get; set; }
    }
}