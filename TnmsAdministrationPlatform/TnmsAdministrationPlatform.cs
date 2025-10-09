using System;
using System.Collections.Generic;
using System.IO;
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
using TnmsAdministrationPlatform.Services;
using TnmsAdministrationPlatform.Shared;
using TnmsDatabaseUtil.Shared;

namespace TnmsAdministrationPlatform;

public class TnmsAdministrationPlatform: IModSharpModule, IAdminManager, IClientListener
{
    private readonly ILogger _logger;
    private readonly ISharedSystem _sharedSystem;
    private readonly string _dllPath;
    
    private AdministrationDbContext? _dbContext;
    
    private UserPermissionService? _userPermissionService;
    private GroupPermissionService? _groupPermissionService;
    private GroupRelationService? _groupRelationService;
    private AdminGroupService? _adminGroupService;
    private AdminUserService? _adminUserService;

    public TnmsAdministrationPlatform(ISharedSystem sharedSystem,
        string?                  dllPath,
        string?                  sharpPath,
        Version?                 version,
        IConfiguration?          coreConfiguration,
        bool                     hotReload)
    {
        
        ArgumentNullException.ThrowIfNull(dllPath);
        ArgumentNullException.ThrowIfNull(sharpPath);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(coreConfiguration);
        
        _dllPath = dllPath;
        _sharedSystem = sharedSystem;
        _logger = sharedSystem.GetLoggerFactory().CreateLogger<TnmsAdministrationPlatform>();
    }
    
    private readonly Dictionary<ulong, IAdminUser> _userPermissions = new();
    private readonly Dictionary<string, IAdminGroup> _groupPermissions = new();
    
    // TODO() Make this configurable
    private readonly TnmsDatabaseProviderType _dbProviderType = TnmsDatabaseProviderType.Sqlite;

    public int ListenerVersion => 1;
    public int ListenerPriority => 20;
    
    
    public string DisplayName => "TnmsAdministrationPlatform";
    public string DisplayAuthor => "faketuna";
    
    public bool Init()
    {
        _sharedSystem.GetClientManager().InstallClientListener(this);
        _logger.LogInformation("TnmsAdministrationPlatform initialized");
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
        _logger.LogInformation("TnmsAdministrationPlatform shutdown");
    }
    
    private bool InitializeDatabase()
    {
        try
        {
            var dbParams = new DbConnectionParameters
            {
                ProviderType = _dbProviderType,
                Host = Path.Combine(_dllPath, "administration.db")
            };

            var options = ConnectionStringUtil.ConfigureDbContext<AdministrationDbContext>(dbParams);
            _dbContext = new AdministrationDbContext(options.Options);
            
            if (!ApplyDatabaseMigrations())
            {
                _logger.LogError("Failed to apply database migrations");
                return false;
            }

            InitializeServices();

            _logger.LogInformation("Database initialized successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize database");
            return false;
        }
    }

    private void InitializeServices()
    {
        if (_dbContext == null)
        {
            _logger.LogError("Cannot initialize services without DbContext");
            return;
        }

        var userPermissionRepo = new UserPermissionRepository(_dbContext);
        var groupPermissionRepo = new GroupPermissionRepository(_dbContext);
        var groupRelationRepo = new GroupRelationRepository(_dbContext);
        var adminGroupRepo = new AdminGroupRepository(_dbContext);
        var adminUserRepo = new AdminUserRepository(_dbContext);

        _userPermissionService = new UserPermissionService(userPermissionRepo);
        _groupPermissionService = new GroupPermissionService(groupPermissionRepo);
        _groupRelationService = new GroupRelationService(groupRelationRepo);
        _adminGroupService = new AdminGroupService(adminGroupRepo);
        _adminUserService = new AdminUserService(adminUserRepo);

        _logger.LogInformation("Services initialized successfully");
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

    private async Task LoadGroupCacheAsync()
    {
        try
        {
            _logger.LogDebug("Loading groups into cache...");
            
            if (_adminGroupService == null || _groupPermissionService == null)
            {
                _logger.LogWarning("Required services not available for loading groups");
                return;
            }

            _groupPermissions.Clear();

            var allGroups = await _adminGroupService.GetAllAdminGroupsAsync();
            
            foreach (var dbGroup in allGroups)
            {
                var adminGroup = new AdminGroup(dbGroup.GroupName)
                {
                    Id = dbGroup.Id
                };
                
                var globalPerms = await _groupPermissionService.GetGroupGlobalPermissionsAsync(dbGroup.Id);
                foreach (var perm in globalPerms)
                {
                    adminGroup.Permissions.Add(perm.PermissionNode);
                }
                
                var serverPerms = await _groupPermissionService.GetGroupServerPermissionsAsync(dbGroup.Id);
                foreach (var perm in serverPerms)
                {
                    adminGroup.Permissions.Add($"{perm.PermissionNode}@{perm.ServerName}");
                }

                _groupPermissions[dbGroup.GroupName] = adminGroup;
            }

            _logger.LogDebug("Loaded {Count} groups into cache", _groupPermissions.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load groups into cache");
        }
    }

    private async Task EnsureUserExistsInDatabaseAsync(ulong steamId)
    {
        try
        {
            if (_adminUserService == null)
            {
                _logger.LogWarning("AdminUserService not available for ensuring user exists for {SteamId}", steamId);
                return;
            }

            var existingUser = await _adminUserService.GetAdminUserAsync(steamId);
            if (existingUser == null)
            {
                await _adminUserService.CreateOrUpdateAdminUserAsync(steamId, immunity: 0);
                _logger.LogDebug("Created new admin user record for {SteamId}", steamId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to ensure user exists in database for {SteamId}", steamId);
        }
    }

    private async Task<AdminUser> LoadUserDataFromDatabaseAsync(ulong steamId, IGameClient client)
    {
        var adminUser = new AdminUser(client);
        
        try
        {
            if (_userPermissionService == null || _groupRelationService == null)
            {
                _logger.LogWarning("Required services not available for loading user data for {SteamId}", steamId);
                return adminUser;
            }

            var globalPermissions = await _userPermissionService.GetUserGlobalPermissionsAsync(steamId);
            foreach (var perm in globalPermissions)
            {
                adminUser.Permissions.Add(perm.PermissionNode);
            }

            var serverPermissions = await _userPermissionService.GetUserServerPermissionsAsync(steamId);
            foreach (var perm in serverPermissions)
            {
                adminUser.Permissions.Add($"{perm.PermissionNode}@{perm.ServerName}");
            }

            var userGroups = await _groupRelationService.GetUserGroupsAsync(steamId);
            foreach (var groupRelation in userGroups)
            {
                var cachedGroup = _groupPermissions.Values
                    .FirstOrDefault(g => g is AdminGroup ag && ag.Id == groupRelation.GroupId);
                
                if (cachedGroup != null)
                {
                    adminUser.Groups.Add(cachedGroup);
                }
            }

            _logger.LogDebug("Loaded user data for {SteamId}: {PermissionCount} permissions, {GroupCount} groups", 
                steamId, adminUser.Permissions.Count, adminUser.Groups.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load user data from database for {SteamId}", steamId);
        }

        return adminUser;
    }

    public void OnClientPostAdminCheck(IGameClient client)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                if (_userPermissions.ContainsKey(client.SteamId.AccountId))
                {
                    return;
                }

                if (!_groupPermissions.Any())
                {
                    await LoadGroupCacheAsync();
                }

                await EnsureUserExistsInDatabaseAsync(client.SteamId.AccountId);

                var adminUser = await LoadUserDataFromDatabaseAsync(client.SteamId.AccountId, client);
                _userPermissions[client.SteamId.AccountId] = adminUser;

                _logger.LogInformation(
                    "Loaded admin data for user {SteamId} ({PlayerName}): {PermissionCount} permissions, {GroupCount} groups, Immunity {Immunity}",
                    client.SteamId.AccountId, client.Name, adminUser.Permissions.Count, adminUser.Groups.Count, adminUser.Immunity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error loading admin data for user {SteamId} ({PlayerName})",
                    client.SteamId.AccountId, client.Name);
            }
        });
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
        
        if (executorInfo == null || targetInfo == null)
            return true;
        
        bool executorIsAdmin = VerifyPermission(executorInfo, IAdminManager.AdminPermissionNode);
        bool executorIsRoot = VerifyPermission(executorInfo, IAdminManager.RootPermissionWildCard);
        
        bool targetIsAdmin = VerifyPermission(targetInfo, IAdminManager.AdminPermissionNode);
        bool targetIsRoot = VerifyPermission(targetInfo, IAdminManager.RootPermissionWildCard);
        
        if (!executorIsAdmin && !targetIsAdmin)
            return true;

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

    public PermissionSaveResult AddPermissionToClient(IGameClient client, string permission, string? serverName = null)
    {
        var steamId = client.SteamId.AccountId;
        
        if (serverName == null)
        {
            var success = _userPermissions[steamId].Permissions.Add(permission);
            if (success)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        if (_userPermissionService != null)
                        {
                            await _userPermissionService.AddGlobalPermissionAsync(steamId, permission);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to persist global permission {Permission} for user {SteamId}", permission, steamId);
                    }
                });
            }
            return success ? PermissionSaveResult.Success : PermissionSaveResult.FailureDuplicatePermission;
        }
        else
        {
            var success = _userPermissions[steamId].Permissions.Add($"{permission}@{serverName}");
            if (success)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        if (_userPermissionService != null)
                        {
                            await _userPermissionService.AddServerPermissionAsync(steamId, permission, serverName);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to persist server permission {Permission} for user {SteamId} on server {ServerName}", permission, steamId, serverName);
                    }
                });
            }
            return success ? PermissionSaveResult.Success : PermissionSaveResult.FailureDuplicatePermission;
        }
    }

    public PermissionSaveResult RemovePermissionFromClient(IGameClient client, string permission, string? serverName = null)
    {
        var steamId = client.SteamId.AccountId;
        
        if (serverName == null)
        {
            var success = _userPermissions[steamId].Permissions.Remove(permission);
            if (success)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        if (_userPermissionService != null)
                        {
                            await _userPermissionService.RemoveGlobalPermissionAsync(steamId, permission);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to remove global permission {Permission} for user {SteamId}", permission, steamId);
                    }
                });
            }
            return success ? PermissionSaveResult.Success : PermissionSaveResult.FailureDontHavePermission;
        }
        else
        {
            var success = _userPermissions[steamId].Permissions.Remove($"{permission}@{serverName}");
            if (success)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        if (_userPermissionService != null)
                        {
                            await _userPermissionService.RemoveServerPermissionAsync(steamId, permission, serverName);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to remove server permission {Permission} for user {SteamId} on server {ServerName}", permission, steamId, serverName);
                    }
                });
            }
            return success ? PermissionSaveResult.Success : PermissionSaveResult.FailureDontHavePermission;
        }
    }

    public PermissionSaveResult AddPermissionToGroup(string groupName, string permission, string? serverName = null)
    {
        if (!_groupPermissions.TryGetValue(groupName, out var group))
            return PermissionSaveResult.GroupNotFound;
        
        if (serverName == null)
        {
            var success = group.Permissions.Add(permission);
            if (success)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        if (_adminGroupService != null && _groupPermissionService != null)
                        {
                            var dbGroup = await _adminGroupService.GetAdminGroupByNameAsync(groupName);
                            {
                                if (dbGroup != null)
                                    await _groupPermissionService.AddGlobalPermissionAsync(dbGroup.Id, permission);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to persist global permission {Permission} for group {GroupName}", permission, groupName);
                    }
                });
            }
            return success ? PermissionSaveResult.Success : PermissionSaveResult.FailureDuplicatePermission;
        }
        else
        {
            var success = group.Permissions.Add($"{permission}@{serverName}");
            if (success)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        if (_adminGroupService != null && _groupPermissionService != null)
                        {
                            var dbGroup = await _adminGroupService.GetAdminGroupByNameAsync(groupName);
                            if (dbGroup != null)
                            {
                                await _groupPermissionService.AddServerPermissionAsync(dbGroup.Id, permission, serverName);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to persist server permission {Permission} for group {GroupName} on server {ServerName}", permission, groupName, serverName);
                    }
                });
            }
            return success ? PermissionSaveResult.Success : PermissionSaveResult.FailureDuplicatePermission;
        }
    }

    public PermissionSaveResult RemovePermissionFromGroup(string groupName, string permission, string? serverName = null)
    {
        if (!_groupPermissions.TryGetValue(groupName, out var group))
            return PermissionSaveResult.GroupNotFound;
        
        if (serverName == null)
        {
            var success = group.Permissions.Remove(permission);
            if (success)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        if (_adminGroupService != null && _groupPermissionService != null)
                        {
                            var dbGroup = await _adminGroupService.GetAdminGroupByNameAsync(groupName);
                            if (dbGroup != null)
                            {
                                await _groupPermissionService.RemoveGlobalPermissionAsync(dbGroup.Id, permission);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to remove global permission {Permission} for group {GroupName}", permission, groupName);
                    }
                });
            }
            return success ? PermissionSaveResult.Success : PermissionSaveResult.FailureDontHavePermission;
        }
        else
        {
            var success = group.Permissions.Remove($"{permission}@{serverName}");
            if (success)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        if (_adminGroupService != null && _groupPermissionService != null)
                        {
                            var dbGroup = await _adminGroupService.GetAdminGroupByNameAsync(groupName);
                            if (dbGroup != null)
                            {
                                await _groupPermissionService.RemoveServerPermissionAsync(dbGroup.Id, permission, serverName);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to remove server permission {Permission} for group {GroupName} on server {ServerName}", permission, groupName, serverName);
                    }
                });
            }
            return success ? PermissionSaveResult.Success : PermissionSaveResult.FailureDontHavePermission;
        }
    }

    public PermissionSaveResult AddClientToGroup(IGameClient client, string groupName)
    {
        if (!_groupPermissions.TryGetValue(groupName, out var group))
            return PermissionSaveResult.GroupNotFound;
        
        var adminUser = _userPermissions[client.SteamId.AccountId];
        var success = adminUser.Groups.Add(group);
        
        if (success)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    if (_adminGroupService != null && _groupRelationService != null)
                    {
                        var dbGroup = await _adminGroupService.GetAdminGroupByNameAsync(groupName);
                        if (dbGroup != null)
                        {
                            await _groupRelationService.AddUserToGroupAsync(client.SteamId.AccountId, dbGroup.Id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to persist group relation for user {SteamId} to group {GroupName}", client.SteamId.AccountId, groupName);
                }
            });
        }
        
        return success ? PermissionSaveResult.Success : PermissionSaveResult.FailureClientAlreadyInGroup;
    }

    public PermissionSaveResult RemoveClientFromGroup(IGameClient client, string groupName)
    {
        if (!_groupPermissions.TryGetValue(groupName, out var group))
            return PermissionSaveResult.GroupNotFound;
        
        var adminUser = _userPermissions[client.SteamId.AccountId];
        var success = adminUser.Groups.Remove(group);
        
        if (success)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    if (_adminGroupService != null && _groupRelationService != null)
                    {
                        var dbGroup = await _adminGroupService.GetAdminGroupByNameAsync(groupName);
                        if (dbGroup != null)
                        {
                            await _groupRelationService.RemoveUserFromGroupAsync(client.SteamId.AccountId, dbGroup.Id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to remove group relation for user {SteamId} from group {GroupName}", client.SteamId.AccountId, groupName);
                }
            });
        }
        
        return success ? PermissionSaveResult.Success : PermissionSaveResult.FailureClientDontHaveGroup;
    }

    public IAdminUser? GetAdminInformation(IGameClient client)
    {
        if (!_userPermissions.TryGetValue(client.SteamId.AccountId, out var user))
            return null;
        
        return user;
    }

    public byte GetClientImmunity(IGameClient client)
    {
        var adminUser = _userPermissions[client.SteamId.AccountId];
        return adminUser.Immunity;
    }

    public PermissionSaveResult SetClientImmunity(IGameClient client, byte immunity)
    {
        var steamId = client.SteamId.AccountId;
        var adminUser = _userPermissions[steamId];
        adminUser.Immunity = immunity;

        _ = Task.Run(async () =>
        {
            try
            {
                if (_adminUserService != null)
                {
                    await _adminUserService.CreateOrUpdateAdminUserAsync(steamId, immunity);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist immunity {Immunity} for user {SteamId}", immunity, steamId);
            }
        });

        return PermissionSaveResult.Success;
    }
}
