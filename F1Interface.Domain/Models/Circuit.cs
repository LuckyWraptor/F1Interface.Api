namespace F1Interface.Domain.Models
{
    public class Circuit
    {
        /// <summary>
        /// Circuit identifier
        /// </summary>
        public uint Id { get; set; }
        /// <summary>
        /// Short name of the circuit
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The full and official name of the circuit
        /// </summary>
        public string OfficialName { get; set; }
        /// <summary>
        /// Location of the circuit
        /// </summary>
        public string Location { get; set; }   
    }
}