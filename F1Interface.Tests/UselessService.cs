using Microsoft.Extensions.Logging;
using PlaywrightSharp;

namespace F1Interface.Tests
{
    public class UselessService : ServiceBase<UselessService>
    {
        public UselessService(ILogger<UselessService> logger, IBrowser browser)
            : base(logger, browser)
        {

        }
    }
}