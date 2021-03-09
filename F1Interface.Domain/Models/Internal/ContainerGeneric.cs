using System.Collections.Generic;

namespace F1Interface.Domain.Models.Internal
{
    internal class Container<T, T2>
    {
        public T2 Id { get; set; }
        public string Layout { get; set; }
        public Action[] Actions { get; set; }
        public T Metadata { get; set; }
        public int TotalDepth { get; set; }
        public Dictionary<string, string> Translations { get; set; }
    }
}