namespace Orc.Squirrel.Tests.Example;

using System.Globalization;
using System.Resources;
using Catel.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Orc.Squirrel.Example.ViewModels;

public class MainViewModelFacts
{
    [TestFixture]
    public class The_Constructor
    {
        [TestCase]
        public void Uses_The_Injected_Language_Service_For_Title()
        {
            var expectedTitle = "Localized example title";
            using var serviceProvider = ServiceCollectionHelper.CreateServiceCollection().BuildServiceProvider();

            var languageServiceMock = new Mock<ILanguageService>();
            languageServiceMock
                .Setup(x => x.GetRequiredString("Orc_Squirrel_Example_MainViewModel_Title"))
                .Returns(expectedTitle);

            var viewModel = new MainViewModel(
                Mock.Of<IUIVisualizerService>(),
                Mock.Of<IDispatcherService>(),
                Mock.Of<IUpdateService>(),
                Mock.Of<IUpdateExecutableLocationService>(),
                languageServiceMock.Object,
                serviceProvider);

            Assert.That(viewModel.Title, Is.EqualTo(expectedTitle));
        }
    }

    [TestFixture]
    public class The_Example_Resources
    {
        [TestCase("Orc_Squirrel_Example_MainWindow_ExecutableDirectoryOptional", "Executable directory (optional)")]
        [TestCase("Orc_Squirrel_Example_MainWindow_UpdateUrl", "Update url")]
        [TestCase("Orc_Squirrel_Example_MainWindow_CheckForUpdates", "Check for updates")]
        [TestCase("Orc_Squirrel_Example_MainWindow_Update", "Update")]
        [TestCase("Orc_Squirrel_Example_MainWindow_ShowInstallationWindow", "Show installation window")]
        public void Contain_The_Localized_Main_Window_Strings(string resourceName, string expectedValue)
        {
            var resourceManager = new ResourceManager("Orc.Squirrel.Example.Properties.Resources", typeof(Orc.Squirrel.Example.App).Assembly);

            var value = resourceManager.GetString(resourceName, CultureInfo.GetCultureInfo("en-US"));

            Assert.That(value, Is.EqualTo(expectedValue));
        }
    }
}
