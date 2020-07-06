namespace Orc.Squirrel.Example.Services
{
    using Orc.FileSystem;

    public class ExampleUpdateExecutableLocationService : UpdateExecutableLocationService
    {
        public ExampleUpdateExecutableLocationService(IFileService fileService) 
            : base(fileService)
        {
        }

        public string ExecutableFileName { get; set; }

        public override string GetApplicationExecutable()
        {
            return ExecutableFileName;
        }
    }
}
