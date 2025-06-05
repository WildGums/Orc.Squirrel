namespace Orc.Squirrel
{
    using Catel.Reflection;

    public class AppMetadataProvider : IAppMetadataProvider
    {
        public AppMetadataProvider()
        {
            var entryAssembly = AssemblyHelper.GetRequiredEntryAssembly();

            AppId = entryAssembly.GetName().Name ?? string.Empty;
        }

        public string AppId { get; set; }

        public string? CurrentVersion { get; set; }
    }
}
