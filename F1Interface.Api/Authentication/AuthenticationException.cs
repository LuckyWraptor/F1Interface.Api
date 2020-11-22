using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace F1Interface.Api.Authentication
{
    public class AuthenticationException : ApiException
    {
        /// <summary>
        /// Statuscode of the request
        /// </summary>
        public HttpStatusCode StatusCode { get; private init; }

        public AuthenticationException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
