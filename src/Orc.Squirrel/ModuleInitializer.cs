using Catel.IoC;
using Catel.Services;
using Orc.Squirrel;

/// <summary>
/// Used by the ModuleInit. All code inside the Initialize method is ran as soon as the assembly is loaded.
/// </summary>
public static class ModuleInitializer
{
    /// <summary>
    /// Initializes the module.
    /// </summary>
    public static void Initialize()
    {
        var serviceLocator = ServiceLocator.Default;

        serviceLocator.RegisterType<IUpdateExecutableLocationService, UpdateExecutableLocationService>();
        serviceLocator.RegisterType<IUpdateService, UpdateService>();
        serviceLocator.RegisterType<IAppMetadataProvider, AppMetadataProvider>();

        var languageService = serviceLocator.ResolveRequiredType<ILanguageService>();
        languageService.RegisterLanguageSource(new LanguageResourceSource("Orc.Squirrel", "Orc.Squirrel.Properties", "Resources"));
    }
}
