using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sharp.Shared;
using Sharp.Shared.Enums;
using Sharp.Shared.Listeners;
using Sharp.Shared.Objects;

namespace TnmsAdministrationPlatform;

public class TnmsAdministrationPlatform: IModSharpModule, IAdminManager, IClientListener
{
    
    private readonly ILogger _logger;
    private readonly ISharedSystem _sharedSystem;
    
    public TnmsAdministrationPlatform(ISharedSystem sharedSystem,
        string?                  dllPath,
        string?                  sharpPath,
        Version?                 version,
        IConfiguration?          coreConfiguration,
        bool                     hotReload)
    {
        _sharedSystem = sharedSystem;
        _logger = sharedSystem.GetLoggerFactory().CreateLogger<TnmsAdministrationPlatform>();
        
        ArgumentNullException.ThrowIfNull(dllPath);
        ArgumentNullException.ThrowIfNull(sharpPath);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(coreConfiguration);
    }
    
    private readonly Dictionary<ulong, HashSet<string>> _userPermissions = new();

    public int ListenerVersion => 1;
    public int ListenerPriority => 20;
    
    
    public string DisplayName => "TnmsAdministrationPlatform";
    public string DisplayAuthor => "faketuna";
    
    public bool Init()
    {
        // TODO() Admin config loading
        _logger.LogInformation("Loaded TnmsAdministrationPlatform");
        _sharedSystem.GetClientManager().InstallClientListener(this);
        return true;
    }

    public void PostInit()
    {
        _sharedSystem.GetSharpModuleManager().RegisterSharpModuleInterface(this, IAdminManager.ModSharpModuleIdentity, (IAdminManager)this);
    }

    public void Shutdown()
    {
        foreach (var (_, value) in _userPermissions)
        {
            value.Clear();
        }
        _userPermissions.Clear();
        _sharedSystem.GetClientManager().RemoveClientListener(this);
        _logger.LogInformation("Unloaded TnmsAdministrationPlatform");
    }
    
    public void OnClientConnected(IGameClient client)
    {
        // TODO() Add permission loading feature from cached config
        if (!_userPermissions.ContainsKey(client.SteamId.AccountId))
        {
            _userPermissions[client.SteamId.AccountId] = new HashSet<string>();
        }
    }

    public void OnClientDisconnecting(IGameClient client, NetworkDisconnectionReason reason)
    {
        _userPermissions.Remove(client.SteamId.AccountId, out var set);
        set?.Clear();
    }

    public bool ClientHasPermission(IGameClient? client, string permission)
    {
        // Because Console
        if (client == null)
            return true;
        
        if (!_userPermissions.TryGetValue(client.SteamId.AccountId, out var set))
            return false;
        
        if (set.Contains(IAdminManager.RootPermissionWildCard))
            return true;
        
        // TODO()
        // Implement wildcard matching system
        // if test.command.A, test.command.B is exists and player have test.command.*
        // Then player can execute both.
        
        return set.Contains(permission);
    }

    public bool AddPermissionToClient(IGameClient client, string permission)
    {
        return _userPermissions[client.SteamId.AccountId].Add(permission);
    }

    public bool RemovePermissionFromClient(IGameClient client, string permission)
    {
        return _userPermissions[client.SteamId.AccountId].Remove(permission);
    }
}