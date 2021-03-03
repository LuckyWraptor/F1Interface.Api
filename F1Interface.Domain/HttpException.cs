using System.Net;

namespace F1Interface.Domain
{
    public class HttpException : F1InterfaceException
    {
        /// <summary>
        /// Statuscode of the request
        /// </summary>
        public HttpStatusCode StatusCode { get; private init; }

        public HttpException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}