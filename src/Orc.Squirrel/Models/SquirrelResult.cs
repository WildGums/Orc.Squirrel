namespace Orc.Squirrel
{
    public class SquirrelResult
    {
        public SquirrelResult()
        {
            CurrentVersion = string.Empty;
            NewVersion = string.Empty;
        }

        public bool IsUpdateInstalledOrAvailable { get; set; }

        public string CurrentVersion { get; set; }

        public string NewVersion { get; set; }
    }
}
