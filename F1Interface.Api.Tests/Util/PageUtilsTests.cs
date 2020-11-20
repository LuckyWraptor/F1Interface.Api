using System;
using System.Text;
using System.Threading.Tasks;
using Xunit;

using FluentAssertions;
using Moq;
using PlaywrightSharp;
using PlaywrightSharp.Input;

using F1Interface.Api.Util;

namespace F1Interface.Api.Tests.Util
{
    public class PageUtilsTests
    {
        [Fact]
        public async Task ClickAsync_ShouldClickSelector()
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
            await PageUtils.ClickAsync(pageMoq.Object, "specificSelector");

            // Assert
            clicked.Should()
                        .BeTrue();
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
            await PageUtils.ClickAsync(pageMoq.Object, "randomSelector");

            // Assert
            clicked.Should()
                        .BeFalse();
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
            await PageUtils.TypeAsync(pageMoq.Object,
                    "specificSelector",
                    inputText);


            // Assert
            stringBuilder.ToString()
                        .Should()
                        .BeEquivalentTo(inputText);
        }
        [Fact]
        public async Task TypeAsync_SelectorNotFound()
        {
            // Arrange
            var pageMoq = new Mock<IPage>();
            pageMoq.Setup(x => x.QuerySelectorAsync("specificSelector"))
                        .Returns(Task.FromResult(default(IElementHandle)));

            Func<Task> action = () => PageUtils.TypeAsync(pageMoq.Object,
                    "specificSelector",
                    "SomeText");

            // Assert
            action.Should()
                .NotThrow();
        }
    }
}
