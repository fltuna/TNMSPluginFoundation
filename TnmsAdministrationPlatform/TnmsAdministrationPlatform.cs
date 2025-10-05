using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sharp.Shared;
using Sharp.Shared.Enums;
using Sharp.Shared.Listeners;
using Sharp.Shared.Objects;
using TnmsAdministrationPlatform.Data;
using TnmsAdministrationPlatform.Shared;
using TnmsCentralizedDbPlatform.Shared;

namespace TnmsAdministrationPlatform;

public class TnmsAdministrationPlatform: IModSharpModule, IAdminManager, IClientListener
{
    
    private readonly ILogger _logger;
    private readonly ISharedSystem _sharedSystem;
    
    private ITnmsCentralizedDbPlatform? _dbPlatform;
    private AdministrationDbContext? _dbContext;
    
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
    
    // TODO() Make this configurable
    // private TnmsDatabaseProviderType _dbProviderType = TnmsDatabaseProviderType.Sqlite;

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

    public void OnAllModulesLoaded()
    {
        if (!InitializeDatabase())
        {
            _logger.LogError("Failed to initialize database in OnAllModulesLoaded. Plugin may not work correctly");
        }
    }

    public void Shutdown()
    {
        _userPermissions.Clear();
        _dbContext?.Dispose();
        _sharedSystem.GetClientManager().RemoveClientListener(this);
        _logger.LogInformation("Unloaded TnmsAdministrationPlatform");
    }
    
    private bool InitializeDatabase()
    {
        try
        {
            _dbPlatform = _sharedSystem.GetSharpModuleManager()
                .GetRequiredSharpModuleInterface<ITnmsCentralizedDbPlatform>(
                    ITnmsCentralizedDbPlatform.ModSharpModuleIdentity).Instance;

            if (_dbPlatform == null)
            {
                _logger.LogWarning("TnmsCentralizedDbPlatform not found. Database features will be disabled.");
                return false;
            }

            var dbParams = new DbConnectionParameters
            {
                ProviderType = TnmsDatabaseProviderType.Sqlite,
                Host = "administration.db"
            };

            var options = _dbPlatform.ConfigureDbContext<AdministrationDbContext>(dbParams, "TnmsAdministrationPlatform");
            _dbContext = new AdministrationDbContext(options.Options);
            
            if (!ApplyDatabaseMigrations())
            {
                _logger.LogError("Failed to apply database migrations");
                return false;
            }

            _logger.LogInformation("Database initialized successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize database");
            return false;
        }
    }

    private bool ApplyDatabaseMigrations()
    {
        try
        {
            if (_dbContext == null)
            {
                _logger.LogError("DbContext is null");
                return false;
            }

            var pendingMigrations = _dbContext.Database.GetPendingMigrations().ToList();
            
            if (pendingMigrations.Any())
            {
                _logger.LogInformation("Found {Count} pending migration(s): {Migrations}", 
                    pendingMigrations.Count, string.Join(", ", pendingMigrations));

                if (IsAutoMigrationEnabled())
                {
                    _logger.LogInformation("Auto-applying database migrations...");
                    _dbContext.Database.Migrate();
                    _logger.LogInformation("Database migrations applied successfully");
                }
                else
                {
                    _logger.LogWarning("Pending migrations detected but auto-migration is disabled.");
                    _logger.LogWarning("Please run 'dotnet ef database update' manually to apply migrations.");
                }
            }
            else
            {
                _logger.LogInformation("Database is up to date, no pending migrations");
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during migration process");
            return false;
        }
    }

    private bool IsAutoMigrationEnabled()
    {
        // TODO() Load this from config
        return true;
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

    public PermissionSaveResult AddPermissionToClient(IGameClient client, string permission)
    {
        return _userPermissions[client.SteamId.AccountId].Permissions.Add(permission) ? PermissionSaveResult.Success : PermissionSaveResult.FailureDuplicatePermission;
    }

    public PermissionSaveResult RemovePermissionFromClient(IGameClient client, string permission)
    {
        return _userPermissions[client.SteamId.AccountId].Permissions.Remove(permission) ? PermissionSaveResult.Success : PermissionSaveResult.FailureDontHavePermission;
    }

    public PermissionSaveResult AddPermissionToGroup(string groupName, string permission)
    {
        if (!_groupPermissions.TryGetValue(groupName, out var group))
            return PermissionSaveResult.GroupNotFound;
        
        return group.Permissions.Add(permission) ? PermissionSaveResult.Success : PermissionSaveResult.FailureDuplicatePermission;
    }

    public PermissionSaveResult RemovePermissionFromGroup(string groupName, string permission)
    {
        if (!_groupPermissions.TryGetValue(groupName, out var group))
            return PermissionSaveResult.GroupNotFound;
        
        return group.Permissions.Remove(permission) ? PermissionSaveResult.Success : PermissionSaveResult.FailureDontHavePermission;
    }

    public PermissionSaveResult AddClientToGroup(IGameClient client, string groupName)
    {
        if (!_groupPermissions.TryGetValue(groupName, out var group))
            return PermissionSaveResult.GroupNotFound;
        
        return _userPermissions[client.SteamId.AccountId].Groups.Add(group) ? PermissionSaveResult.Success : PermissionSaveResult.FailureClientAlreadyInGroup;
    }

    public PermissionSaveResult RemoveClientFromGroup(IGameClient client, string groupName)
    {
        if (!_groupPermissions.TryGetValue(groupName, out var group))
            return PermissionSaveResult.GroupNotFound;
        
        return _userPermissions[client.SteamId.AccountId].Groups.Remove(group) ? PermissionSaveResult.Success : PermissionSaveResult.FailureClientDontHaveGroup;
    }

    public IAdminUser GetAdminInformation(IGameClient client)
    {
        return _userPermissions[client.SteamId.AccountId];
    }
}