using System;
using Microsoft.Extensions.Logging;

using PlaywrightSharp;

namespace F1Interface.Api.Generic
{
    public class ServiceBase
    {
        protected readonly ILogger _logger;
        protected readonly IBrowser _browser;

        internal ServiceBase(IBrowser apiBrowser, ILogger logger)
        {
            if (apiBrowser == null)
            {
                throw new ArgumentNullException("Provided IApiBrowser cannot be null!");
            }

            _browser = apiBrowser;
            _logger = logger;
        }
    }
}
