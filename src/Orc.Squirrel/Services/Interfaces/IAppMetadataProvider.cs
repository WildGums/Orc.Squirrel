namespace Orc.Squirrel
{
    public interface IAppMetadataProvider
    {
        string AppId { get; set; }
        string? CurrentVersion { get; set; }
    }
}
