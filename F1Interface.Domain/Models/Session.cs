using System;
using System.Text.Json.Serialization;

namespace F1Interface.Domain.Models
{
    /// <summary>
    /// An FIA-planned session (most likely FP, Q, race or testing)
    /// </summary>
    public class Session
    {
        public ulong Id { get; set; }
        public string ImageId { get; set; }
        public string Series { get; set; }
        public uint EventId { get; set; }
        public DateTime Starts { get; set; }
        public DateTime Ends { get; set; }
        public bool Testing { get; set; }
        public bool IsLive { get; set; }

        public FIAEvent Event { get; set;}
    }
}