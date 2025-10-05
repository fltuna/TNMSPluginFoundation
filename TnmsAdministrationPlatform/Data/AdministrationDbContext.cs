using Microsoft.EntityFrameworkCore;
using TnmsAdministrationPlatform.Models;

namespace TnmsAdministrationPlatform.Data;

public class AdministrationDbContext : DbContext
{
    public AdministrationDbContext(DbContextOptions<AdministrationDbContext> options) 
        : base(options)
    {
    }
    
    // DbSets
    public DbSet<TnmsAdminServer> AdminServers { get; set; }
    public DbSet<TnmsAdminUser> AdminUsers { get; set; }
    public DbSet<TnmsUserPermissionGlobal> UserPermissionGlobals { get; set; }
    public DbSet<TnmsUserPermissionServer> UserPermissionServers { get; set; }
    public DbSet<TnmsAdminGroup> AdminGroups { get; set; }
    public DbSet<TnmsGroupRelation> GroupRelations { get; set; }
    public DbSet<TnmsGroupPermissionGlobal> GroupPermissionGlobals { get; set; }
    public DbSet<TnmsGroupPermissionServer> GroupPermissionServers { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // TnmsAdminServer configuration
        modelBuilder.Entity<TnmsAdminServer>(entity =>
        {
            entity.HasKey(e => e.ServerName);
            entity.Property(e => e.ServerName).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(500);
        });
        
        // TnmsAdminUser configuration
        modelBuilder.Entity<TnmsAdminUser>(entity =>
        {
            entity.HasKey(e => e.SteamId);
        });
        
        // TnmsUserPermissionGlobal configuration
        modelBuilder.Entity<TnmsUserPermissionGlobal>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserSteamId, e.PermissionNode })
                  .IsUnique()
                  .HasDatabaseName("uk_user_perm_global");
            entity.HasIndex(e => e.UserSteamId)
                  .HasDatabaseName("idx_user_perm_global_steam_id");
            
            entity.HasOne(e => e.User)
                  .WithMany(u => u.UserPermissionGlobals)
                  .HasForeignKey(e => e.UserSteamId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        // TnmsUserPermissionServer configuration
        modelBuilder.Entity<TnmsUserPermissionServer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserSteamId, e.PermissionNode, e.ServerName })
                  .IsUnique()
                  .HasDatabaseName("uk_user_perm_server");
            entity.HasIndex(e => e.UserSteamId)
                  .HasDatabaseName("idx_user_perm_server_steam_id");
            entity.HasIndex(e => e.ServerName)
                  .HasDatabaseName("idx_user_perm_server_name");
            
            entity.HasOne(e => e.User)
                  .WithMany(u => u.UserPermissionServers)
                  .HasForeignKey(e => e.UserSteamId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Server)
                  .WithMany(s => s.UserPermissionServers)
                  .HasForeignKey(e => e.ServerName)
                  .OnDelete(DeleteBehavior.Restrict);
        });
        
        // TnmsAdminGroup configuration
        modelBuilder.Entity<TnmsAdminGroup>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.GroupName)
                  .IsUnique();
            entity.Property(e => e.GroupName).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
        });
        
        // TnmsGroupRelation configuration
        modelBuilder.Entity<TnmsGroupRelation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.GroupId, e.UserSteamId })
                  .IsUnique()
                  .HasDatabaseName("uk_group_user");
            entity.HasIndex(e => e.UserSteamId)
                  .HasDatabaseName("idx_group_relation_steam_id");
            entity.HasIndex(e => e.GroupId)
                  .HasDatabaseName("idx_group_relation_group_id");
            
            entity.HasOne(e => e.Group)
                  .WithMany(g => g.GroupRelations)
                  .HasForeignKey(e => e.GroupId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.User)
                  .WithMany(u => u.GroupRelations)
                  .HasForeignKey(e => e.UserSteamId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        // TnmsGroupPermissionGlobal configuration
        modelBuilder.Entity<TnmsGroupPermissionGlobal>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.GroupId, e.PermissionNode })
                  .IsUnique()
                  .HasDatabaseName("uk_group_perm_global");
            entity.HasIndex(e => e.GroupId)
                  .HasDatabaseName("idx_group_perm_global_group_id");
            
            entity.HasOne(e => e.Group)
                  .WithMany(g => g.GroupPermissionGlobals)
                  .HasForeignKey(e => e.GroupId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        // TnmsGroupPermissionServer configuration
        modelBuilder.Entity<TnmsGroupPermissionServer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.GroupId, e.PermissionNode, e.ServerName })
                  .IsUnique()
                  .HasDatabaseName("uk_group_perm_server");
            entity.HasIndex(e => e.GroupId)
                  .HasDatabaseName("idx_group_perm_server_group_id");
            entity.HasIndex(e => e.ServerName)
                  .HasDatabaseName("idx_group_perm_server_name");
            
            entity.HasOne(e => e.Group)
                  .WithMany(g => g.GroupPermissionServers)
                  .HasForeignKey(e => e.GroupId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Server)
                  .WithMany(s => s.GroupPermissionServers)
                  .HasForeignKey(e => e.ServerName)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
