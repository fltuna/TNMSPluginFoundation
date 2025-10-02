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
    
    private readonly Dictionary<ulong, IAdminUser> _userPermissions = new();

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
        _userPermissions.Clear();
        _sharedSystem.GetClientManager().RemoveClientListener(this);
        _logger.LogInformation("Unloaded TnmsAdministrationPlatform");
    }
    
    public void OnClientConnected(IGameClient client)
    {
        // TODO() Add permission loading feature from cached config
        if (!_userPermissions.ContainsKey(client.SteamId.AccountId))
        {
            _userPermissions[client.SteamId.AccountId] = new AdminUser(client);
        }
    }

    public void OnClientDisconnecting(IGameClient client, NetworkDisconnectionReason reason)
    {
        _userPermissions.Remove(client.SteamId.AccountId);
    }

    public bool ClientHasPermission(IGameClient? client, string permission)
    {
        // Because Console
        if (client == null)
            return true;
        
        if (!_userPermissions.TryGetValue(client.SteamId.AccountId, out var adminUser))
            return false;
        
        if (adminUser.Permissions.Contains(IAdminManager.RootPermissionWildCard))
            return true;
        
        if (adminUser.Permissions.Contains(permission))
            return true;
        
        foreach (var userPerm in adminUser.Permissions)
        {
            if (!userPerm.EndsWith(".*"))
                continue;
                
            var prefix = userPerm.Substring(0, userPerm.Length - 1);
            if (permission.StartsWith(prefix))
                return true;
        }
        
        return false;
    }

    public bool AddPermissionToClient(IGameClient client, string permission)
    {
        return _userPermissions[client.SteamId.AccountId].Permissions.Add(permission);
    }

    public bool RemovePermissionFromClient(IGameClient client, string permission)
    {
        return _userPermissions[client.SteamId.AccountId].Permissions.Remove(permission);
    }

    public IAdminUser GetAdminInformation(IGameClient client)
    {
        return _userPermissions[client.SteamId.AccountId];
    }
}