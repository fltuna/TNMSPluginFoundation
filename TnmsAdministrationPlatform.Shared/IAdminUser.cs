using System.Collections.Generic;
using Sharp.Shared.Objects;

namespace TnmsAdministrationPlatform.Shared;

public interface IAdminUser
{
    public IGameClient Client { get; }
    public HashSet<string> Permissions { get; }
    public HashSet<IAdminGroup> Groups { get; }
    public byte Immunity { get; set; }
}