using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TnmsAdministrationPlatform.Models;

[Table("tnms_admin_group_relation")]
public class TnmsGroupRelation
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("group_id")]
    public int GroupId { get; set; }
    
    [Column("user_steam_id")]
    public ulong UserSteamId { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual TnmsAdminGroup Group { get; set; } = null!;
    public virtual TnmsAdminUser User { get; set; } = null!;
}
