namespace TnmsAdministrationPlatform;

public class AdminGroup(string groupName) : IAdminGroup
{
    public string GroupName { get; } = groupName;
    public HashSet<string> Permissions => new();
}