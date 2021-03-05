using System.Drawing;

using System.Threading.Tasks;
using Xunit;

using FluentAssertions;
using Moq;
using PlaywrightSharp;
using PlaywrightSharp.Input;
using F1Interface.Domain.Models;
using System.Text.Json;
using System.Net;
using F1Interface.Domain.Responses;
using F1Interface.Domain;
using System;
using System.Collections.Generic;
using F1Interface.Domain.Requests;
using System.Threading;

namespace F1Interface.Tests
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

            Service = new AuthenticationService(LoggerMock.Object, BrowserMock.Object);
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
                        .Throw<HttpException>()
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
                        .Throw<HttpException>()
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
                        .Throw<F1InterfaceException>()
                        .WithMessage("Unhandled response (this shouldn't happen!)");
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
                responseMock.Setup(x => x.GetJsonAsync<TokenObject>(It.IsAny<JsonSerializerOptions>()))
                    .Returns(Task.FromResult(new TokenObject
                    {
                        Token = "valid_token"
                    }));
                responseMock.Setup(x => x.Status)
                    .Returns(HttpStatusCode.OK);

                response = responseMock.Object;
            }
        }
    }
}
