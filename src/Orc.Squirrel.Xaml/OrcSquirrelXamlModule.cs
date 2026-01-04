namespace Orc.Squirrel
{
    using Catel.Services;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Core module which allows the registration of default services in the service collection.
    /// </summary>
    public static class OrcSquirrelXamlModule
    {
        public static IServiceCollection AddOrcSquirrelXaml(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ILanguageSource>(new LanguageResourceSource("Orc.Squirrel.Xaml", "Orc.Squirrel.Properties", "Resources"));

            return serviceCollection;
        }
    }
}
