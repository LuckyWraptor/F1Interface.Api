namespace F1Interface.Contracts
{
    public interface IF1ApiInterface
    {
        /// <summary>
        /// F1 Account authentication service
        /// </summary>
        IAuthenticationService Authentication { get; }
        /// <summary>
        /// F1TV Content service to access your paid content
        /// </summary>
        IContentService Content { get; }
    }
}