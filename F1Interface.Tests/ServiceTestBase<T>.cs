using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using PlaywrightSharp;

namespace F1Interface.Tests
{
    public class ServiceTestBase<T>
    {
        protected Mock<IBrowser> BrowserMock { get; private init; }
        protected Mock<IPage> PageMock { get; private init; }
        protected Mock<ILogger<T>> LoggerMock { get; private init; }

        protected T Service { get; init; }

        internal ServiceTestBase()
        {
            PageMock = new Mock<IPage>();

            BrowserMock = new Mock<IBrowser>();
            BrowserMock.Setup(x => x.NewPageAsync(It.IsAny<BrowserContextOptions>()))
                .Returns(Task.FromResult(PageMock.Object));

            LoggerMock = new Mock<ILogger<T>>();
        }
    }
}