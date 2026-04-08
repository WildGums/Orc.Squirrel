namespace Orc.Squirrel.Tests;

using Catel;
using Microsoft.Extensions.DependencyInjection;
using Orc.Squirrel;
using Orc.Theming;

internal static class ServiceCollectionHelper
{
    public static IServiceCollection CreateServiceCollection()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddLogging();
        serviceCollection.AddCatelCore();
        serviceCollection.AddCatelMvvm();
        serviceCollection.AddOrcTheming();
        serviceCollection.AddOrcSquirrel();
        serviceCollection.AddOrcSquirrelXaml();

        return serviceCollection;
    }
}
