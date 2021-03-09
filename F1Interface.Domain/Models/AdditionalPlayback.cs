using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace F1Interface.Domain.Models
{
    public class AdditionalPlayback
    {
        public string ConstructorName { get; set; }
        public string DriverFirstName { get; set; }
        public string DriverLastName { get; set; }
        [JsonPropertyName("hex")]
        public string HexCode { get; set; }
        public uint ChannelId { get; private set; }
        public uint RacingNumber { get; set; }
        public string TeamName { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }

        public string PlaybackUrl
        {
            set {
                Match channelMatch = Regex.Match(value, @"\d+");

                ChannelId = uint.Parse(channelMatch.Value);
            }
        }
    }
}