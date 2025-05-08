namespace Orc.Squirrel.Tests.Services
{
    using System.Linq;
    using System.Threading.Tasks;
    using Catel.Configuration;
    using Moq;
    using NUnit.Framework;
    using Orc.FileSystem;
    using Orc.Squirrel.Velopack;

    public partial class UpdateServiceFacts
    {
        [TestFixture]
        public class The_CheckForUpdatesAsync_Method
        {
            [TestCase]
            public async Task Returns_No_Update_When_None_Available_Async()
            {
                var updateChannels = new[]
                {
                    new UpdateChannel("stable", @".\Resources\Files\Velopack\")
                };

                var configurationServiceMock = new Mock<IConfigurationService>();
                configurationServiceMock.Setup(x => x.GetValue(It.IsAny<ConfigurationContainer>(), It.Is<string>(y => y == Settings.Application.AutomaticUpdates.CheckForUpdates), It.IsAny<bool>()))
                    .Returns(true);
                configurationServiceMock.Setup(x => x.GetValue(It.IsAny<ConfigurationContainer>(), It.Is<string>(y => y == Settings.Application.AutomaticUpdates.UpdateChannel), It.IsAny<string>()))
                    .Returns("stable");
                configurationServiceMock.Setup(x => x.GetValue(It.IsAny<ConfigurationContainer>(), It.Is<string>(y => y == "AutomaticUpdates.Channels.stable"), It.IsAny<string>()))
                    .Returns(@".\Resources\Files\Velopack\");

                var fileServiceMock = new Mock<IFileService>();
                var updateExecutableServiceMock = new Mock<IUpdateExecutableLocationService>();

                var appMetadataProviderMock = new Mock<IAppMetadataProvider>();
                appMetadataProviderMock.Setup(x => x.AppId)
                    .Returns("TestApp");
                appMetadataProviderMock.Setup(x => x.CurrentVersion)
                    .Returns("1.4.0-alpha1143");

                var updateService = new UpdateService(configurationServiceMock.Object, fileServiceMock.Object, 
                    updateExecutableServiceMock.Object, appMetadataProviderMock.Object, new SquirrelVelopackLocator());

                var context = new SquirrelContext
                {
                    ChannelName = "stable"
                };

                // Act
                await updateService.InitializeAsync(updateChannels, updateChannels.First(), true);

                var result = await updateService.CheckForUpdatesAsync(context);

                // Assert
                Assert.That(result.IsUpdateInstalledOrAvailable, Is.False);
            }

            [TestCase]
            public async Task Returns_No_Update_When_Velopack_Is_Unavailable_Async()
            {
                var updateChannels = new[]
                {
                    new UpdateChannel("stable", @".\Resources\Files\Velopack\")
                };

                var configurationServiceMock = new Mock<IConfigurationService>();
                configurationServiceMock.Setup(x => x.GetValue(It.IsAny<ConfigurationContainer>(), It.Is<string>(y => y == Settings.Application.AutomaticUpdates.CheckForUpdates), It.IsAny<bool>()))
                    .Returns(true);
                configurationServiceMock.Setup(x => x.GetValue(It.IsAny<ConfigurationContainer>(), It.Is<string>(y => y == Settings.Application.AutomaticUpdates.UpdateChannel), It.IsAny<string>()))
                    .Returns("stable");
                configurationServiceMock.Setup(x => x.GetValue(It.IsAny<ConfigurationContainer>(), It.Is<string>(y => y == "AutomaticUpdates.Channels.stable"), It.IsAny<string>()))
                    .Returns(@".\Resources\Files\Velopack-non-existing\");

                var fileServiceMock = new Mock<IFileService>();
                var updateExecutableServiceMock = new Mock<IUpdateExecutableLocationService>();

                var appMetadataProviderMock = new Mock<IAppMetadataProvider>();
                appMetadataProviderMock.Setup(x => x.AppId)
                    .Returns("TestApp");
                appMetadataProviderMock.Setup(x => x.CurrentVersion)
                    .Returns("1.4.0-alpha1143");

                var updateService = new UpdateService(configurationServiceMock.Object, fileServiceMock.Object,
                    updateExecutableServiceMock.Object, appMetadataProviderMock.Object, new SquirrelVelopackLocator());

                var context = new SquirrelContext
                {
                    ChannelName = "stable"
                };

                // Act
                await updateService.InitializeAsync(updateChannels, updateChannels.First(), true);

                var result = await updateService.CheckForUpdatesAsync(context);

                // Assert
                Assert.That(result.IsUpdateInstalledOrAvailable, Is.False);
            }

            [TestCase]
            public async Task Returns_Update_When_Velopack_Finds_One_Async()
            {
                var updateChannels = new[]
                {
                    new UpdateChannel("stable", @".\Resources\Files\Velopack\")
                };

                var configurationServiceMock = new Mock<IConfigurationService>();
                configurationServiceMock.Setup(x => x.GetValue(It.IsAny<ConfigurationContainer>(), It.Is<string>(y => y == Settings.Application.AutomaticUpdates.CheckForUpdates), It.IsAny<bool>()))
                    .Returns(true);
                configurationServiceMock.Setup(x => x.GetValue(It.IsAny<ConfigurationContainer>(), It.Is<string>(y => y == Settings.Application.AutomaticUpdates.UpdateChannel), It.IsAny<string>()))
                    .Returns("stable");
                configurationServiceMock.Setup(x => x.GetValue(It.IsAny<ConfigurationContainer>(), It.Is<string>(y => y == "AutomaticUpdates.Channels.stable"), It.IsAny<string>()))
                    .Returns(@".\Resources\Files\Velopack\");

                var fileServiceMock = new Mock<IFileService>();
                var updateExecutableServiceMock = new Mock<IUpdateExecutableLocationService>();

                var appMetadataProviderMock = new Mock<IAppMetadataProvider>();
                appMetadataProviderMock.Setup(x => x.AppId)
                    .Returns("TestApp");
                appMetadataProviderMock.Setup(x => x.CurrentVersion)
                    .Returns("1.0.0");

                var updateService = new UpdateService(configurationServiceMock.Object, fileServiceMock.Object, 
                    updateExecutableServiceMock.Object, appMetadataProviderMock.Object, new SquirrelVelopackLocator());

                var context = new SquirrelContext
                {
                    ChannelName = "stable"
                };

                // Act
                await updateService.InitializeAsync(updateChannels, updateChannels.First(), true);

                var result = await updateService.CheckForUpdatesAsync(context);

                // Assert
                Assert.That(result.IsUpdateInstalledOrAvailable, Is.True);
                Assert.That(result.NewVersion, Is.EqualTo("1.4.0-alpha1143"));
            }

            [TestCase, Explicit]
            public async Task Returns_Update_When_Squirrel_Finds_One_Async()
            {
                var configurationServiceMock = new Mock<IConfigurationService>();
                var fileServiceMock = new Mock<IFileService>();
                var updateExecutableServiceMock = new Mock<IUpdateExecutableLocationService>();

                var appMetadataProviderMock = new Mock<IAppMetadataProvider>();
                appMetadataProviderMock.Setup(x => x.AppId)
                    .Returns("TestApp");
                appMetadataProviderMock.Setup(x => x.CurrentVersion)
                    .Returns("1.0.0");

                var updateService = new UpdateService(configurationServiceMock.Object, fileServiceMock.Object, 
                    updateExecutableServiceMock.Object, appMetadataProviderMock.Object, new SquirrelVelopackLocator());

                var context = new SquirrelContext
                {
                    ChannelName = "stable"
                };

                // Act
                var result = await updateService.CheckForUpdatesAsync(context);

                // Assert
                Assert.That(result.IsUpdateInstalledOrAvailable, Is.True);
                Assert.That(result.NewVersion, Is.EqualTo("2.0.0"));
            }
        }
    }
}
