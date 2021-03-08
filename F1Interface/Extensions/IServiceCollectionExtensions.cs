using Esprima.Ast;
using F1Interface.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace F1Interface
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddF1Services(this IServiceCollection collection)
            => collection.AddScoped<IContentService, ContentService>()
                .AddScoped<IAuthenticationService, AuthenticationService>()
                .AddScoped<IF1ApiInterface, F1ApiInterface>();
    }
}