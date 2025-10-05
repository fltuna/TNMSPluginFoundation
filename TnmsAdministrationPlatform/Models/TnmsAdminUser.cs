using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TnmsAdministrationPlatform.Models;

[Table("tnms_admin_user")]
public class TnmsAdminUser
{
    [Key]
    [Column("steam_id")]
    public ulong SteamId { get; set; }
    
    [Column("immunity")]
    public byte Immunity { get; set; } = 0;
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual IEnumerable<TnmsUserPermissionGlobal> UserPermissionGlobals { get; set; } = new List<TnmsUserPermissionGlobal>();
    public virtual IEnumerable<TnmsUserPermissionServer> UserPermissionServers { get; set; } = new List<TnmsUserPermissionServer>();
    public virtual IEnumerable<TnmsGroupRelation> GroupRelations { get; set; } = new List<TnmsGroupRelation>();
}
