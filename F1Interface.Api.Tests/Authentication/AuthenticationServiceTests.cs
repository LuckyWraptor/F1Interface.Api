using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

using FluentAssertions;
using Moq;
using PlaywrightSharp;
using PlaywrightSharp.Input;

using F1Interface.Api.Authentication;
using F1Interface.Api.Authentication.Models;
using F1Interface.Api.Constants;
using F1Interface.Api.Generic;
using F1Interface.Api.Generic.Models;

namespace F1Interface.Api.Tests.Authentication
{
    public class AuthenticationServiceTests : ServiceTestBase<AuthenticationService>
    {
        public AuthenticationServiceTests()
        {
            Mock<IElementHandle> elementMock = new Mock<IElementHandle>();
            elementMock.Setup(x => x.FocusAsync())
                .Returns(Task.Delay(0));

            Mock<IKeyboard> keyboardMock = new Mock<IKeyboard>();
            keyboardMock.Setup(x => x.PressAsync(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(Task.Delay(0));

            PageMock.Setup(x => x.ClickAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<MouseButton>(),
                            It.IsAny<int>(), It.IsAny<Modifier[]>(), It.IsAny<Point?>(), It.IsAny<int?>(), It.IsAny<bool>(), It.IsAny<bool?>()))
                        .Returns(Task.Delay(0));
            PageMock.Setup(x => x.GoToAsync(It.IsAny<string>(), It.IsAny<LifecycleEvent>(), It.IsAny<string>(), It.IsAny<int>()))
                        .Returns(Task.FromResult(default(IResponse)));
            PageMock.Setup(x => x.QuerySelectorAsync(It.IsAny<string>()))
                        .Returns(Task.FromResult(elementMock.Object));
            PageMock.Setup(x => x.CloseAsync(It.IsAny<bool>()))
                        .Callback(() => PageMock.SetupGet(x => x.IsClosed).Returns(true))
                        .Returns(Task.Delay(1));
            PageMock.Setup(x => x.Keyboard)
                        .Returns(keyboardMock.Object);

            Service = new AuthenticationService(BrowserMock.Object, LoggerMock.Object);
        }

        [Fact]
        public async Task AuthenticateAsync_Valid()
        {
            // Arrange
            SetAuthenticationResponse();

            // Act
            var response = await Service.AuthenticateAsync(PageMock.Object, "username", "password");

            // Assert
            response.Should()
                        .NotBeNull();
            response.SessionId.Should()
                        .NotBeNull()
                        .And.BeEquivalentTo("valid_oauth_jwt_token");

            response.Session.Should()
                        .NotBeNull();
            response.Session.Status.Should()
                        .NotBeNull()
                        .And.BeEquivalentTo("valid");
            response.Session.Token.Should()
                        .NotBeNull()
                        .And.BeEquivalentTo("valid_jwt_token");

            PageMock.Object.IsClosed.Should()
                        .BeFalse();
        }

        [Fact]
        public async Task AuthenticateAsync_Valid_WithoutIPageParameter()
        {
            // Arrange 
            SetAuthenticationResponse();

            // Act
            await Service.AuthenticateAsync("username", "password");

            // Assert
            PageMock.Object.IsClosed.Should()
                        .BeTrue();
        }

        [Fact]
        public void AuthenticateAsync_InvalidResponse()
        {
            // Arrange
            var responseMock = new Mock<IResponse>();
            responseMock.Setup(x => x.Status)
                .Returns(HttpStatusCode.OK);
            responseMock.Setup(x => x.GetJsonAsync<AuthenticationResponse>(It.IsAny<JsonSerializerOptions>()))
                        .Returns(Task.FromResult(default(AuthenticationResponse)));
            SetAuthenticationResponse(responseMock.Object);

            Func<Task> action = () => Service.AuthenticateAsync("username", "password");

            // Act & Assert
            action.Should()
                        .Throw<AuthenticationException>()
                        .WithMessage("Authentication failed as an invalid/incomprehensive response was retrieved.")
                        .And.StatusCode.Should()
                                .BeEquivalentTo(HttpStatusCode.Forbidden);
        }

        [Fact]
        public void AuthenticateAsync_Cancelled()
        {
            // Arrange
            SetAuthenticationResponse();
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            Func<Task> action = () => Service.AuthenticateAsync("username", "password", tokenSource.Token);

            // Act & Assert
            tokenSource.Cancel();
            action.Should()
                        .Throw<TaskCanceledException>();
        }

        [Fact]
        public void AuthenticateAsync_Unauthorized()
        {
            // Arrange
            var responseMock = new Mock<IResponse>();
            responseMock.Setup(x => x.Status)
                .Returns(HttpStatusCode.Unauthorized);
            SetAuthenticationResponse(responseMock.Object);

            Func<Task> action = () => Service.AuthenticateAsync("username", "password");

            // Act & Assert
            action.Should()
                        .Throw<AuthenticationException>()
                        .WithMessage("Invalid credentials provided.")
                        .And.StatusCode.Should()
                                .BeEquivalentTo(HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData(HttpStatusCode.Forbidden)]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.NotFound)]
        public void AuthenticateAsync_UnhandledException(HttpStatusCode statusCode)
        {
            // Arrange
            var responseMock = new Mock<IResponse>();
            responseMock.Setup(x => x.Status)
                .Returns(statusCode);
            SetAuthenticationResponse(responseMock.Object);

            // Act
            Func<Task> action = () => Service.AuthenticateAsync("username", "password");

            // Assert
            action.Should()
                        .Throw<ApiException>()
                        .WithMessage("Unhandled response (this shouldn't happen!)");
        }

        [Theory]
        [InlineData(null)]
        [InlineData(AuthenticationService.IdentityProvider)]
        public async Task AuthenticateF1TVTokenAsync_Valid(string identityProvider)
        {
            // Arrange
            SetTokenResponse();

            // Act
            var response = await Service.AuthenticateF1TVTokenAsync(PageMock.Object, "valid_session_token", identityProvider);

            // Assert
            response.Should()
                        .NotBeNull();
            response.Token.Should()
                        .NotBeNull()
                        .And.BeEquivalentTo("valid_token");
        }

        [Fact]
        public async Task AuthenticateF1TVTokenAsync_Valid_WithoutIPageParameter()
        {
            // Arrange 
            SetTokenResponse();

            // Act
            await Service.AuthenticateF1TVTokenAsync("username");

            // Assert
            PageMock.Object.IsClosed.Should()
                        .BeTrue();
        }

        [Fact]
        public void AuthenticateF1TVTokenAsync_BadRequest()
        {
            // Arrange
            var responseMock = new Mock<IResponse>();
            responseMock.Setup(x => x.Status)
                        .Returns(HttpStatusCode.BadRequest);
            responseMock.Setup(x => x.GetJsonAsync<BadRequestResponse>(It.IsAny<JsonSerializerOptions>()))
                        .Returns(Task.FromResult(new BadRequestResponse
                        {
                            Error = "Some api error message",
                            ErrorCode = "ERROR_CODE",
                            ValidationErrors = new Dictionary<string, FieldError>
                            {
                                {  "property1", new FieldError { Code = "required", Message = "Some Message" } },
                                {  "property2", new FieldError { Code = "required1", Message = "Some Message" } }
                            }
                        }));

            SetTokenResponse(responseMock.Object);

            // Act
            Func<Task> action = () => Service.AuthenticateF1TVTokenAsync("invalid_token");

            // Assert
            var assertable = action.Should()
                        .Throw<BadRequestException>()
                        .And.Should()
                                .NotBeNull();
        }

        [Theory]
        [InlineData(HttpStatusCode.Forbidden)]
        [InlineData(HttpStatusCode.NotFound)]
        public void AuthenticateF1TVTokenAsync_UnhandledException(HttpStatusCode statusCode)
        {
            // Arrange
            var responseMock = new Mock<IResponse>();
            responseMock.Setup(x => x.Status)
                .Returns(statusCode);
            SetTokenResponse(responseMock.Object);

            // Act
            Func<Task> action = () => Service.AuthenticateF1TVTokenAsync("valid_token");

            // Assert
            action.Should()
                        .Throw<ApiException>()
                        .WithMessage("Unhandled response (this shouldn't happen!)");
        }

        [Fact]
        public void AuthenticateF1TVTokenAsync_InvalidResponse()
        {
            // Arrange
            var responseMock = new Mock<IResponse>();
            responseMock.Setup(x => x.GetJsonAsync<AuthenticationToken>(It.IsAny<JsonSerializerOptions>()))
                .Returns(Task.FromResult(default(AuthenticationToken)));
            responseMock.Setup(x => x.Status)
                .Returns(HttpStatusCode.OK);
            SetTokenResponse(responseMock.Object);

            // Act
            Func<Task> action = () => Service.AuthenticateF1TVTokenAsync("valid_token");

            // Assert
            action.Should()
                        .Throw<AuthenticationException>()
                        .WithMessage("F1TV Token failed as an invalid/incomprehensive response was retrieved.");
        }

        private void SetAuthenticationResponse(IResponse response = null)
        {
            if (response == null)
            {
                var responseMock = new Mock<IResponse>();
                responseMock.Setup(x => x.GetJsonAsync<AuthenticationResponse>(It.IsAny<JsonSerializerOptions>()))
                    .Returns(Task.FromResult(new AuthenticationResponse
                    {
                        SessionId = "valid_oauth_jwt_token",
                        Session = new Subscription
                        {
                            Status = "valid",
                            Token = "valid_jwt_token"
                        }
                    }));
                responseMock.Setup(x => x.Status)
                    .Returns(HttpStatusCode.OK);

                response = responseMock.Object;
            }

            PageMock.Setup(x => x.WaitForResponseAsync(Endpoints.AuthenticationByPassword, It.IsAny<int?>()))
                        .Returns(Task.FromResult(response));
        }
        private void SetTokenResponse(IResponse response = null)
        {
            if (response == null)
            {
                var responseMock = new Mock<IResponse>();
                responseMock.Setup(x => x.GetJsonAsync<AuthenticationToken>(It.IsAny<JsonSerializerOptions>()))
                    .Returns(Task.FromResult(new AuthenticationToken
                    {
                        Token = "valid_token"
                    }));
                responseMock.Setup(x => x.Status)
                    .Returns(HttpStatusCode.OK);

                response = responseMock.Object;
            }

            PageMock.Setup(x => x.GoToAsync(Endpoints.F1TV.AuthenticateToken, It.IsAny<LifecycleEvent?>(), It.IsAny<string>(), It.IsAny<int?>()))
                        .Returns(Task.FromResult(response));
        }
    }
}
