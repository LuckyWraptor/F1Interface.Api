using System;
using System.Text.Json.Serialization;

namespace F1Interface.Domain.Models.Internal
{
    internal class EventMetadata : TitleMetadata
    {
        [JsonPropertyName("availableAlso")]
        public string[] AvailablePlatforms { get; set; }
        public ulong ContentId { get; set; }
        public string ContentSubtype { get; set; }
        public ulong ContractEndDate { get; set; }
        public ulong ContractStartDate { get; set; }
        public ulong Duration { get; set; }
        public string ExternalId { get; set; }
        public string[] Genres { get; set; }
        public string Language { get; set; }
        public string LongDescription { get; set; }
        [JsonPropertyName("contentType")]
        public string Type { get; set; }
        [JsonPropertyName("pictureUrl")]
        public string PictureId { get; set; }
        public uint Season { get; set; }
        public uint StarRating { get; set; }
        public string TitleBrief { get; set; }
        public string UiDuration { get; set; }
        [JsonPropertyName("emfAttributes")]
        public MeetingAttributes Attributes { get; set; }
    }
}