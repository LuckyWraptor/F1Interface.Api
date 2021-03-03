using System.Text.Json.Serialization;
using F1Interface.Domain.Models;

namespace F1Interface.Domain.Responses
{
    public class AuthenticationResponse
    {
        /// <summary>
        /// Session identifier JWT token
        /// </summary>
        public string SessionId { get; set; }
        /// <summary>
        /// Specifies if password is temporary and should be renewed?
        /// </summary>
        public bool PasswordIsTemporary { get; set; }
        /// <summary>
        /// Subscriber details
        /// </summary>
        public Subscriber Subscriber { get; set; }
        /// <summary>
        /// Country of authentication
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// Authentication session contents
        /// </summary>
        [JsonPropertyName("data")]
        public Subscription Session { get; set; }
    }
}