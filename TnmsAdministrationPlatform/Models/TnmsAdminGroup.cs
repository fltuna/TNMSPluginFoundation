using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TnmsAdministrationPlatform.Models;

[Table("tnms_admin_group")]
public class TnmsAdminGroup
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("group_name")]
    [MaxLength(255)]
    public string GroupName { get; set; } = string.Empty;
    
    [Column("description")]
    public string? Description { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual IEnumerable<TnmsGroupRelation> GroupRelations { get; set; } = new List<TnmsGroupRelation>();
    public virtual IEnumerable<TnmsGroupPermissionGlobal> GroupPermissionGlobals { get; set; } = new List<TnmsGroupPermissionGlobal>();
    public virtual IEnumerable<TnmsGroupPermissionServer> GroupPermissionServers { get; set; } = new List<TnmsGroupPermissionServer>();
}
