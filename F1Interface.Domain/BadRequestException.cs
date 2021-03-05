using F1Interface.Domain.Responses;

namespace F1Interface.Domain
{
    public class BadRequestException : F1InterfaceException
    {
        /// <summary>
        /// Bad request object containing the error
        /// </summary>
        public BadRequestResponse BadRequest { get; private init; }

        public BadRequestException(BadRequestResponse badRequest = null)
            : base(badRequest?.Error)
        {
            BadRequest = badRequest;
        }
    }
}