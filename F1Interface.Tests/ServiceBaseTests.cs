using System;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using F1Interface.Domain;
using FluentAssertions;
using Moq;
using PlaywrightSharp;
using PlaywrightSharp.Input;
using Xunit;

namespace F1Interface.Tests
{
    public class ServiceBaseTests : ServiceTestBase<UselessService>, IDisposable
    {
        public ServiceBaseTests()
        {
            Service = new UselessService(LoggerMock.Object, BrowserMock.Object);
        }

        [Fact]
        public async Task NewPageAsync_Valid()
        {
            // Arrange
            BrowserContextOptions browserContextOptions = null;
            BrowserMock.Setup(x => x.NewPageAsync(It.IsAny<BrowserContextOptions>()))
                .Callback<BrowserContextOptions>(x => browserContextOptions = x)
                .Returns(Task.FromResult(PageMock.Object));

            // Act
            var page = await Service.NewPageAsync();

            // Assert
            Assert.Equal(PageMock.Object, page);
            Assert.Equal(979, browserContextOptions.Viewport.Height);
            Assert.Equal(1920, browserContextOptions.Viewport.Width);
            Assert.Contains(browserContextOptions.UserAgent, Constants.UserAgents);
        }

        
        [Fact]
        public async Task ClickButtonAsync_Valid()
        {
            // Arrange
            XUnitUtils.IsUnitTesting = false;

            bool called = false;
            string receivedSelector = null;
            int receivedDelay = 0;
            PageMock.Setup(x => x.ClickAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<MouseButton>(),
                    It.IsAny<int>(), It.IsAny<Modifier[]>(), It.IsAny<Point?>(), It.IsAny<int?>(),
                    It.IsAny<bool>(), It.IsAny<bool?>()))
                .Callback<string, int, MouseButton, int, Modifier[], Point?, int?, bool, bool?>((selector, delay, _, _, _, _, _, _, _) =>
                {
                    receivedSelector = selector;
                    receivedDelay = delay;
                })
                .Returns(Task.Run(() => called = true));

            // Act
            await Service.ClickButtonAsync(PageMock.Object, "SomeNeatSelector");

            // Assert
            Assert.True(called);
            Assert.Equal("SomeNeatSelector", receivedSelector);
            Assert.True(receivedDelay < 250 && receivedDelay >= 50);
        }

        [Fact]
        public async Task ClickAsync_ShouldNotClickSelector()
        {
            // Arrange
            var clicked = false;

            var pageMoq = new Mock<IPage>();
            pageMoq.Setup(x => x.ClickAsync("specificSelector",
                    It.IsAny<int>(),
                    It.IsAny<MouseButton>(), It.IsAny<int>(),
                    It.IsAny<Modifier[]>(),
                    It.IsAny<System.Drawing.Point?>(),
                    It.IsAny<int?>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool?>()))
                .Callback(() => clicked = true);

            // Act
            await Service.ClickButtonAsync(pageMoq.Object, "randomSelector");

            // Assert
            Assert.False(clicked);
        }

        [Theory]
        [InlineData("")]
        [InlineData("some_text")]
        [InlineData("Other text stuff")]
        [InlineData("     ")]
        public async Task TypeAsync_Valid(string inputText)
        {
            // Arrange
            StringBuilder stringBuilder = new StringBuilder();

            var elementMoq = new Mock<IElementHandle>();
            elementMoq.Setup(x => x.PressAsync(It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int?>(),
                    It.IsAny<bool?>()))
                .Callback<string, int, int?, bool?>((input, _, _, _) => stringBuilder.Append(input));

            var pageMoq = new Mock<IPage>();
            pageMoq.Setup(x => x.QuerySelectorAsync("specificSelector"))
                .Returns(Task.FromResult(elementMoq.Object));

            // Act
            await Service.TypeAsync(pageMoq.Object, "specificSelector", inputText);

            // Assert
            Assert.Equal(inputText, stringBuilder.ToString());
        }

        [Fact]
        public async Task TypeAsync_SelectorNotFound()
        {
            // Arrange
            var pageMoq = new Mock<IPage>();
            pageMoq.Setup(x => x.QuerySelectorAsync("specificSelector"))
                .Returns(Task.FromResult(default(IElementHandle)));

            Func<Task> action = 
                () => Service.TypeAsync(pageMoq.Object,
                    "specificSelector",
                    "SomeText");

            // Assert
            await action.Should()
                .NotThrowAsync();
        }

        public void Dispose()
        {
            // Ensure unit testing is re-enabled at all times
            XUnitUtils.IsUnitTesting = true;
        }
    }
}