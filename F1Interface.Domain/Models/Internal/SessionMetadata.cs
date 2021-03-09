namespace F1Interface.Domain.Models.Internal
{
    internal class SessionMetadata : EventMetadata
    {
        public uint Year { get; set; }
        public string[] Directors { get; set; }
        public string[] Actors { get; set; }
        public bool LeavingSoon { get; set; }
        public AdditionalPlayback[] AdditionalStreams { get; set; }
    }
}