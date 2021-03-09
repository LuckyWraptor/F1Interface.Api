using System;
using System.Text.Json.Serialization;

namespace F1Interface.Domain.Models
{
    /// <summary>
    /// An FIA-planned session (most likely FP, Q, race or testing)
    /// </summary>
    public class FIASession : Content
    {
        public uint EventId { get; set; }
        public DateTime Starts { get; set; }
        public DateTime Ends { get; set; }
        public bool Testing { get; set; }
        public bool Available { get; set; }
        public bool Temporary { get; set; }
        public string[] Teams { get; set; }
        public string[] Drivers { get; set; }

        public AdditionalPlayback[] SideChannels { get; set; }

        public FIAEvent Event { get; set; }
    }
}