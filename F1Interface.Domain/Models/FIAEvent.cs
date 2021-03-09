using System;
using System.Text.Json.Serialization;

namespace F1Interface.Domain.Models
{
    /// <summary>
    /// An FIA-planned event (race/testing)
    /// </summary>
    public class FIAEvent
    {
        public uint Id { get; set; }
        public uint SeasonId { get; set; }
        public string Name { get; set; }
        public string OfficialName { get; set; }
        public string Sponsor { get; set; }

        public DateTime Starts { get; set; }
        public DateTime Ends { get; set; }

        public Circuit Circuit { get; set; }

        public FIASession[] Sessions { get; set; }
    }
}