namespace Orc.Squirrel.Example.Services;

using FileSystem;
using Microsoft.Extensions.Logging;

public class ExampleUpdateExecutableLocationService : UpdateExecutableLocationService
{
    public ExampleUpdateExecutableLocationService(ILogger<UpdateExecutableLocationService> logger,
        IFileService fileService) 
        : base(logger, fileService)
    {
    }

    public string ExecutableFileName { get; set; }

    public override string GetApplicationExecutable()
    {
        return ExecutableFileName;
    }
}
