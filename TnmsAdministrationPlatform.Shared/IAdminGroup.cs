using System.Collections.Generic;

namespace TnmsAdministrationPlatform.Shared;

public interface IAdminGroup
{
    public string GroupName { get; }
    public HashSet<string> Permissions { get; }
    
    public int Id { get; set; }
}