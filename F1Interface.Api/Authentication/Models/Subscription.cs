using System.Text.Json.Serialization;

namespace F1Interface.Api.Authentication.Models
{
    public class Subscription
    {

        /// <summary>
        /// Status
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
        public bool Active { get => (Status == "active"); }
    }
}
