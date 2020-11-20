using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Interface.Api.Generic.Models
{
    public class FieldError
    {
        /// <summary>
        /// Field error message
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Field error code
        /// </summary>
        public string Code { get; set; }
    }
}
