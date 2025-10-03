using Sharp.Shared.Objects;

namespace TnmsAdministrationPlatform;

public class AdminUser(IGameClient client) : IAdminUser
{
    public IGameClient Client { get; } = client;
    public HashSet<string> Permissions { get; } = new ();
    public HashSet<IAdminGroup> Groups { get; } = new();
    public byte Immunity { get; set; } = 0;
}