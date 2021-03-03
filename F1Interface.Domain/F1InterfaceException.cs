using System;

namespace F1Interface.Domain
{
    /// <summary>
    /// Generic F1Interface exception
    /// </summary>
    public class F1InterfaceException : Exception
    {
        public F1InterfaceException(string message)
            : base(message)
        {

        }
    }
}