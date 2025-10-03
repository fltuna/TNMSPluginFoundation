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
    private readonly Dictionary<string, IAdminGroup> _groupPermissions = new();

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
        // TODO() Add permission loading feature from cached config or DB
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
        
        return VerifyPermission(adminUser, permission);
    }

    public bool ClientCanTarget(IGameClient? executor, IGameClient target)
    {
        if (executor == null)
            return true;
        
        var executorInfo = GetAdminInformation(executor);
        var targetInfo = GetAdminInformation(target);
        
        bool executorIsAdmin = VerifyPermission(executorInfo, IAdminManager.AdminPermissionNode);
        bool executorIsRoot = VerifyPermission(executorInfo, IAdminManager.RootPermissionWildCard);
        
        bool targetIsAdmin = VerifyPermission(targetInfo, IAdminManager.AdminPermissionNode);
        bool targetIsRoot = VerifyPermission(targetInfo, IAdminManager.RootPermissionWildCard);

        if (targetIsAdmin && !executorIsAdmin)
            return false;

        if (targetIsRoot && !executorIsRoot)
            return false;
        
        return targetInfo.Immunity <= executorInfo.Immunity;
    }

    private bool VerifyPermission(IAdminUser user, string permission)
    {
        if (user.Permissions.Contains(IAdminManager.RootPermissionWildCard))
            return true;
        
        if (user.Permissions.Contains(permission))
            return true;
        
        foreach (var userPerm in user.Permissions)
        {
            if (!userPerm.EndsWith(".*"))
                continue;
                
            var prefix = userPerm.Substring(0, userPerm.Length - 1);
            if (permission.StartsWith(prefix))
                return true;
        }

        foreach (var group in user.Groups)
        {
            if (group.Permissions.Contains(IAdminManager.RootPermissionWildCard))
                return true;
        
            if (group.Permissions.Contains(permission))
                return true;
            
            foreach (var groupPerm in group.Permissions)
            {
                if (!groupPerm.EndsWith(".*"))
                    continue;
                
                var prefix = groupPerm.Substring(0, groupPerm.Length - 1);
                if (permission.StartsWith(prefix))
                    return true;
            }
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
    
    public bool AddClientToGroup(IGameClient client, string groupName)
    {
        if (!_groupPermissions.TryGetValue(groupName, out var group))
            return false;
        
        return _userPermissions[client.SteamId.AccountId].Groups.Add(group);
    }

    public bool RemoveClientFromGroup(IGameClient client, string groupName)
    {
        if (!_groupPermissions.TryGetValue(groupName, out var group))
            return false;
        
        return _userPermissions[client.SteamId.AccountId].Groups.Remove(group);
    }

    public IAdminUser GetAdminInformation(IGameClient client)
    {
        return _userPermissions[client.SteamId.AccountId];
    }
}