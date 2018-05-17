namespace Orc.Squirrel
{
    public class SquirrelResult
    {
        public bool IsUpdateInstalledOrAvailable { get; set; }

        public string CurrentVersion { get; set; }

        public string NewVersion { get; set; }
    }
}
