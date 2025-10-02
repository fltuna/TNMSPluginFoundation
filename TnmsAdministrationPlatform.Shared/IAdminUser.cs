using Sharp.Shared.Objects;

namespace TnmsAdministrationPlatform;

public interface IAdminUser
{
    public IGameClient Client { get; }
    public HashSet<string> Permissions { get; }
    public HashSet<string> Groups { get; }
    public byte Immunity { get; set; }
}