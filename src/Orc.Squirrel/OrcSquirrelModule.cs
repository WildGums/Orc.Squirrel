namespace Orc.Squirrel
{
    using Catel.Services;
    using Catel.ThirdPartyNotices;
    using global::Velopack.Locators;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Orc.Squirrel.Velopack;

    /// <summary>
    /// Core module which allows the registration of default services in the service collection.
    /// </summary>
    public static class OrcSquirrelModule
    {
        public static IServiceCollection AddOrcSquirrel(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IUpdateExecutableLocationService, UpdateExecutableLocationService>();
            serviceCollection.TryAddSingleton<IUpdateService, UpdateService>();
            serviceCollection.TryAddSingleton<IAppMetadataProvider, AppMetadataProvider>();
            serviceCollection.TryAddSingleton<IVelopackLocator, SquirrelVelopackLocator>();

            serviceCollection.AddSingleton<ILanguageSource>(new LanguageResourceSource("Orc.Squirrel", "Orc.Squirrel.Properties", "Resources"));

            serviceCollection.AddSingleton<IThirdPartyNotice>((x) => new LibraryThirdPartyNotice("Orc.Squirrel", "https://github.com/wildgums/orc.squirrel"));
            serviceCollection.AddSingleton<IThirdPartyNotice>((x) => new ResourceBasedThirdPartyNotice("Newtonsoft.Json", "https://www.newtonsoft.com/json", "Orc.Squirrel", "Orc.Squirrel", "Resources.ThirdPartyNotices.newtonsoft.json.txt"));
            serviceCollection.AddSingleton<IThirdPartyNotice>((x) => new ResourceBasedThirdPartyNotice("Velopack", "https://github.com/velopack/velopack", "Orc.Squirrel", "Orc.Squirrel", "Resources.ThirdPartyNotices.velopack.txt"));

            return serviceCollection;
        }
    }
}
