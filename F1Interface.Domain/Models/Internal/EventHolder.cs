namespace F1Interface.Domain.Models.Internal
{
    internal class EventHolder : ContainerHolder<Event>
    {
        public string TypeOriginal { get; set; }
        public string UriOriginal { get; set; }
    }
}