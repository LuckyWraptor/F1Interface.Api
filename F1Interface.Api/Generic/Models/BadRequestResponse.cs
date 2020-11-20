using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace F1Interface.Api.Generic.Models
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
