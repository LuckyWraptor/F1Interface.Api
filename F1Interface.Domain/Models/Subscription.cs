using System.Text.Json.Serialization;

namespace F1Interface.Domain.Models
{
    public class Subscription
    {

        /// <summary>
        /// Subscription payment status
        /// </summary>
        [JsonPropertyName("subscriptionStatus")]
        public string Status { get; set; }
        /// <summary>
        /// Subscription access token
        /// </summary>
        [JsonPropertyName("subscriptionToken")]
        public string Token { get; set; }

        /// <summary>
        /// Subscription active status
        /// </summary>
        [JsonIgnore]
        public bool Active { get => (Status == "active"); }
    }
}