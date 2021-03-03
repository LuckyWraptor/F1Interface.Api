using F1Interface.Contracts;

namespace F1Interface
{
    public class F1ApiInterface : IF1ApiInterface
    {
        public IAuthenticationService Authentication { get; private init; }
        public IContentService Content { get; private init; }

        public F1ApiInterface(IAuthenticationService authenticationService, IContentService contentService)
        {
            Authentication = authenticationService;
            Content = contentService;
        }
    }
}