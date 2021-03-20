using System.Text.Json.Serialization;

namespace F1Interface.Domain.Models.Internal
{
    internal class EventFullCategoryHolder : Container<EventMetadata, string>
    {
        [JsonPropertyName("resultObj")]
        public ContainerHolder<EventFullCategory> Categories { get; set; }
    }
}