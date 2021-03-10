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
        public string Entitlement { get; set; }
        public string[] Genres { get; set; }
        public string Language { get; set; }
        public string LongDescription { get; set; }
        [JsonPropertyName("contentType")]
        public string Type { get; set; }
        [JsonPropertyName("pictureUrl")]
        public string PictureId { get; set; }
        public uint Season
        {
            get => (season > 0) ? season : year;
            set => season = value;
        }
        [JsonIgnore]
        public uint Year
        {
            get => (year > 0) ? year : season;
        }
        public uint StarRating { get; set; }
        public string TitleBrief { get; set; }
        public string UiDuration { get; set; }
        [JsonPropertyName("emfAttributes")]
        public EventAttributes Attributes { get; set; }

        [JsonPropertyName("year")]
        public string YearHandler
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && uint.TryParse(value, out uint year))
                {
                    if (season == 0)
                    {
                        season = year;
                    }

                    this.year = year;
                }
            }
        }

        private uint year;
        private uint season;
    }
}