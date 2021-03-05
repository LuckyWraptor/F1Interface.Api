namespace F1Interface.Domain.Models
{
    /// <summary>
    /// A season object containing all planned events.
    /// </summary>
    public class FIASeason
    {
        public uint Season { get; set; }
        /// <summary>
        /// Season title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// All the planned events of the season
        /// </summary>
        public Event Events { get; set; }
    }
}