namespace Orc.Squirrel;

using System;
using System.IO;
using Catel.Logging;
using Catel.Reflection;
using FileSystem;

public class UpdateExecutableLocationService : IUpdateExecutableLocationService
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    private const string UpdateExe = "update.exe";
    private const string NotFoundFindResult = "notfound";

    private readonly IFileService _fileService;
    private string? _updateExeLocation;
    private bool _shownWarning = false;

    public UpdateExecutableLocationService(IFileService fileService)
    {
        ArgumentNullException.ThrowIfNull(fileService);

        _fileService = fileService;
    }

    public virtual string GetApplicationExecutable()
    {
        var entryAssemblyFileName = AssemblyHelper.GetRequiredEntryAssembly().Location;
        return entryAssemblyFileName;
    }

    public string FindUpdateExecutable()
    {
        if (!string.IsNullOrWhiteSpace(_updateExeLocation))
        {
            return _updateExeLocation ?? NotFoundFindResult;
        }

        var applicationExecutable = GetApplicationExecutable();
        if (string.IsNullOrEmpty(applicationExecutable))
        {
            return _updateExeLocation ?? NotFoundFindResult;
        }

        var searchDirectory = Path.GetDirectoryName(applicationExecutable);
        if (searchDirectory is null)
        {
            return _updateExeLocation ?? NotFoundFindResult;
        }

        var safetyCounter = 2;

        try
        {
            while (safetyCounter >= 0 && searchDirectory is not null)
            {
                var potentialUpdateExe = Catel.IO.Path.GetFullPath(UpdateExe, searchDirectory);

                if (_fileService.Exists(potentialUpdateExe))
                {
                    _updateExeLocation = potentialUpdateExe;

                    Log.Debug($"Determined update executable path '{_updateExeLocation}'");

                    break;
                }

                searchDirectory = Path.GetDirectoryName(searchDirectory);
                safetyCounter--;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while searching for the update executable");
        }

        if (_shownWarning || !string.IsNullOrWhiteSpace(_updateExeLocation))
        {
            return _updateExeLocation ?? NotFoundFindResult;
        }

        Log.Info("Could not find the update executable, updates are not supported");

        _shownWarning = true;

        return _updateExeLocation ?? NotFoundFindResult;
    }
}
