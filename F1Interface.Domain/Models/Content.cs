using System;

namespace F1Interface.Domain.Models
{
    public class Content
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public string OfficialName { get; set; }
        public string ImageId { get; set; }
        public string Series { get; set; }
        public ulong Duration { get; set; }
        public ContentType Type { get; set; }
    }
}