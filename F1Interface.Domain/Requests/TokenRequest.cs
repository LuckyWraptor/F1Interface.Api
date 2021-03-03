using System.Text.Json.Serialization;

namespace F1Interface.Domain.Requests
{
     public class TokenRequest
    {
        /// <summary>
        /// Access JWT token
        /// </summary>
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        /// <summary>
        /// IdentityProvider Url
        /// </summary>
        [JsonPropertyName("identity_provider_url")]
        public string IdentityProviderUrl { get; set; }
    }
}