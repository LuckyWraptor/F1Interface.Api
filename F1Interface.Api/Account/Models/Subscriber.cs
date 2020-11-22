using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Interface.Api.Account.Models
{
    public class Subscriber
    {
        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Country of origin.
        /// </summary>
        public string HomeCountry { get; set; }
        /// <summary>
        /// Account identifier
        /// </summary>
        public uint Id { get; set; }
        /// <summary>
        /// Email address
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Login username/identifier
        /// </summary>
        public string Login { get; set; }
    }
}
