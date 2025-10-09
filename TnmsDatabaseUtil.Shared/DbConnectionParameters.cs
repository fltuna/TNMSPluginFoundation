using System.Collections.Generic;

namespace TnmsDatabaseUtil.Shared;

public class DbConnectionParameters
{
    public TnmsDatabaseProviderType ProviderType { get; init; }
    
    /// <summary>
    /// When provider type is SQLite, this will be file name (relative to module directory)
    /// </summary>
    public string? Host { get; init; }
    public string? Port { get; init; }
    public string? Database { get; init; }
    public string? Username { get; init; }
    public string? Password { get; init; }
    
    public Dictionary<string, string> AdditionalParameters { get; } = new();
}