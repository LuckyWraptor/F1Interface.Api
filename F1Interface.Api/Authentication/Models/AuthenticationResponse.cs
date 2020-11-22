using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using F1Interface.Api.Account.Models;

namespace F1Interface.Api.Authentication.Models
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
