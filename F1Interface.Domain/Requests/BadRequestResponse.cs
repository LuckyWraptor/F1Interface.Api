using System.Collections.Generic;
using System.Text.Json.Serialization;
using F1Interface.Domain.Models;

namespace F1Interface.Domain.Requests
{
    public class BadRequestResponse
    {
        /// <summary>
        /// Error message (for front-end use).
        /// </summary>
        public string Error { get; set; }
        /// <summary>
        /// Skylark error code
        /// </summary>
        [JsonPropertyName("sklylark_error_code")]
        public string ErrorCode { get; set; }
        /// <summary>
        /// Form validation errors list
        /// </summary>
        [JsonExtensionData, JsonPropertyName("form_validation_errors")]
        public IDictionary<string, FieldError> ValidationErrors { get; set; }
    }
}