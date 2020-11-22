using System.Collections.Generic;
using Xunit;

using FluentAssertions;

using F1Interface.Api.Generic;
using F1Interface.Api.Generic.Models;

namespace F1Interface.Api.Tests.Generic
{
    public class BadRequestExceptionTests
    {
        [Fact]
        public void Constructor_Valid_WithBadRequest()
        {
            // Arrange & Act
            var exception = new BadRequestException(new BadRequestResponse
            {
                Error = "Some api error message",
                ErrorCode = "ERROR_CODE",
                ValidationErrors = new Dictionary<string, FieldError>
                    {
                        {  "property1", new FieldError { Code = "required", Message = "Some Message" } },
                        {  "property2", new FieldError { Code = "required1", Message = "Some Message" } }
                    }
            });

            // Assert
            exception.Should()
                        .NotBeNull();
            exception.Message.Should()
                        .NotBeNull()
                        .And.BeEquivalentTo("Some api error message");
            exception.BadRequest.Should()
                        .NotBeNull();

            exception.BadRequest.ErrorCode.Should()
                        .NotBeNull()
                        .And.BeEquivalentTo("ERROR_CODE");
            exception.BadRequest.ValidationErrors.Should()
                        .NotBeNull()
                        .And.BeEquivalentTo(new Dictionary<string, FieldError>
                        {
                            {  "property1", new FieldError { Code = "required", Message = "Some Message" } },
                            {  "property2", new FieldError { Code = "required1", Message = "Some Message" } }
                        });
        }

        [Fact]
        public void Constructor_Valid_NullBadRequest()
        {
            // Arrange & Act
            var exception = new BadRequestException(null);

            // Assert
            exception.Should()
                        .NotBeNull();

            exception.BadRequest.Should()
                        .BeNull();

            exception.Message.Should()
                        .BeEquivalentTo("A bad request exception occurred");
        }
    }
}
