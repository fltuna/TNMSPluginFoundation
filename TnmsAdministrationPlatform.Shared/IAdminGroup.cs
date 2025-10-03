namespace TnmsAdministrationPlatform;

public interface IAdminGroup
{
    public HashSet<string> Permissions { get; }
}