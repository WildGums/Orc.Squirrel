namespace Orc.Squirrel
{
    using System;
    using System.IO;
    using Catel;
    using Catel.Logging;
    using Catel.Reflection;
    using Orc.FileSystem;

    public class UpdateExecutableLocationService : IUpdateExecutableLocationService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private const string UpdateExe = "update.exe";

        private readonly IFileService _fileService;
        private string _updateExeLocation;
        private bool _shownWarning = false;

        public UpdateExecutableLocationService(IFileService fileService)
        {
            Argument.IsNotNull(() => fileService);

            _fileService = fileService;
        }

        public virtual string GetApplicationExecutable()
        {
            var entryAssemblyFileName = AssemblyHelper.GetEntryAssembly().Location;
            return entryAssemblyFileName;
        }

        public string FindUpdateExecutable()
        {
            if (string.IsNullOrWhiteSpace(_updateExeLocation))
            {
                var applicationExecutable = GetApplicationExecutable();
                if (!string.IsNullOrEmpty(applicationExecutable))
                {
                    var searchDirectory = Path.GetDirectoryName(applicationExecutable);

                    var safetyCounter = 2;

                    try
                    {
                        while (safetyCounter >= 0)
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

                    if (!_shownWarning && string.IsNullOrWhiteSpace(_updateExeLocation))
                    {
                        Log.Info("Could not find the update executable, updates are not supported");
                        
                        _shownWarning = true;
                    }
                }
            }

            return _updateExeLocation ?? "notfound";
        }
    }
}
