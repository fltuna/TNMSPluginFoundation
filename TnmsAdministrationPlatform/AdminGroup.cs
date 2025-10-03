namespace TnmsAdministrationPlatform;

public class AdminGroup: IAdminGroup
{
    public HashSet<string> Permissions => new();
}