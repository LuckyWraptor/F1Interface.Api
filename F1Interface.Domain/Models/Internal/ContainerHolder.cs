namespace F1Interface.Domain.Models.Internal
{
    internal class ContainerHolder<T>
    {
        public T[] Containers { get; set; }
        public TitleMetadata Metadata { get; set; }
        public int Total { get; set; }
    }
}