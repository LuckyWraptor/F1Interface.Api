namespace F1Interface.Domain.Models.Internal
{
    internal class Event : Container<EventMetadata, uint>
    {
        public MeetingProperties[] Properties { get; set; }
    }
}