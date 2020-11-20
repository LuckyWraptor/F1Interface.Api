﻿using F1Interface.Api.Generic.Models;

namespace F1Interface.Api.Generic
{
    public class BadRequestException : ApiException
    {
        /// <summary>
        /// Bad request object containing the error
        /// </summary>
        public BadRequestResponse BadRequest { get; private init; }
        public BadRequestException(BadRequestResponse badRequest)
            : base(badRequest.Error)
        {
            BadRequest = badRequest;
        }
    }
}