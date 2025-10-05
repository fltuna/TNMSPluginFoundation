using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TnmsAdministrationPlatform.Models;

[Table("tnms_admin_user_permission_server")]
public class TnmsUserPermissionServer
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("user_steam_id")]
    public ulong UserSteamId { get; set; }
    
    [Column("permission_node")]
    [MaxLength(255)]
    public string PermissionNode { get; set; } = string.Empty;
    
    [Column("server_name")]
    [MaxLength(255)]
    public string ServerName { get; set; } = string.Empty;
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual TnmsAdminUser User { get; set; } = null!;
    public virtual TnmsAdminServer Server { get; set; } = null!;
}
