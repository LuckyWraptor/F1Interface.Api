using System;
using System.Collections.Generic;
using System.Text;

namespace F1Interface.Api
{
    /// <summary>
    /// Generic F1Interface exception
    /// </summary>
    public class ApiException : Exception
    {
        public ApiException(string message)
            : base(message) { }
    }
}
