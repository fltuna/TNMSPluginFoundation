namespace TnmsCentralizedDbPlatform.Shared;

public class DbConnectionParameters
{
    public TnmsDatabaseProviderType ProviderType { get; set; }
    
    /// <summary>
    /// When provider type is SQLite, this will be file name (relative to module directory)
    /// </summary>
    public string? Host { get; set; }
    public string? Port { get; set; }
    public string? Database { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}