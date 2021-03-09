using System;
using System.Text.Json.Serialization;

namespace F1Interface.Domain.Models
{
    /// <summary>
    /// An FIA-planned session (most likely FP, Q, race or testing)
    /// </summary>
    public class Session : Content
    {
        public uint EventId { get; set; }
        public DateTime Starts { get; set; }
        public DateTime Ends { get; set; }
        public bool Testing { get; set; }
        public bool IsLive { get; set; }

        public FIAEvent Event { get; set; }
    }
}