using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Interface.Api.Constants
{
    public class Endpoints
    {
        /// <summary>
        /// Login page url
        /// </summary>
        public const string LoginPage = "https://account.formula1.com/#/en/login";
        /// <summary>
        /// Password authentication endpoint
        /// </summary>
        public const string AuthenticationByPassword = "https://api.formula1.com/v2/account/subscriber/authenticate/by-password";

        public class F1TV
        {
            /// <summary>
            /// API endpoint for the F1TV Service
            /// </summary>
            public const string Api = "https://f1tv-api.formula1.com/agl/1.0/unk/en/all_devices/global/";
            /// <summary>
            /// F1TV authentication token.
            /// </summary>
            public const string AuthenticateToken = Api + "authenticate";
        }
    }
}
