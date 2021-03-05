namespace F1Interface.Domain.Models.Internal
{
    internal class Event : Container<EventMetadata>
    {
        public MeetingProperties[] Properties { get; set; }
    }
}