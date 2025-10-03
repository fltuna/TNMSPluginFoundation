namespace TnmsAdministrationPlatform;

public interface IAdminGroup
{
    public string GroupName { get; }
    public HashSet<string> Permissions { get; }
}