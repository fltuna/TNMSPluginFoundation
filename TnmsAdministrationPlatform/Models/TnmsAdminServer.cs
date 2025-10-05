using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TnmsAdministrationPlatform.Models;

[Table("tnms_admin_server")]
public class TnmsAdminServer
{
    [Key]
    [Column("server_name")]
    public string ServerName { get; set; } = string.Empty;
    
    [Column("description")]
    public string? Description { get; set; }
    
    [Column("is_active")]
    public bool IsActive { get; set; } = true;
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual IEnumerable<TnmsUserPermissionServer> UserPermissionServers { get; set; } = new List<TnmsUserPermissionServer>();
    public virtual IEnumerable<TnmsGroupPermissionServer> GroupPermissionServers { get; set; } = new List<TnmsGroupPermissionServer>();
}
