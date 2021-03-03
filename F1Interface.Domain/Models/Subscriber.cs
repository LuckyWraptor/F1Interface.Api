namespace F1Interface.Domain.Models
{
    public class Subscriber
    {
        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Country of origin.
        /// </summary>
        public string HomeCountry { get; set; }
        /// <summary>
        /// Account identifier
        /// </summary>
        public uint Id { get; set; }
        /// <summary>
        /// Email address
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Login username/identifier (seems like it's just the email)
        /// </summary>
        public string Login { get; set; }
    }
}