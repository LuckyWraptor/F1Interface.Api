using Xunit;

using FluentAssertions;

using F1Interface.Api.Authentication;

namespace F1Interface.Api.Tests.Authentication
{
    public class AuthenticationExceptionTests
    {
        [Fact]
        public void Constructor_Valid()
        {
            // Arrange & Act
            var exception = new AuthenticationException("Authentication error message", System.Net.HttpStatusCode.Unauthorized);

            // Assert
            exception.Should()
                        .NotBeNull();
            exception.Message.Should()
                        .NotBeNull()
                        .And.BeEquivalentTo("Authentication error message");
            exception.StatusCode.Should()
                        .BeEquivalentTo(System.Net.HttpStatusCode.Unauthorized);
        }
    }
}
