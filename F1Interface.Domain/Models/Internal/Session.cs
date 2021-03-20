namespace F1Interface.Domain.Models.Internal
{
    internal class Session : Container<SessionMetadata, ulong>
    {
        public string PlatformName { get; set; }
        public MeetingProperties[] Properties { get; set; }
    }
}