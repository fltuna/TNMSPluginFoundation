using System.Collections.Generic;
using System.Linq;
using Sharp.Shared.Objects;
using TnmsAdministrationPlatform.Shared;
using TnmsPluginFoundation.Interfaces;

namespace TnmsPluginFoundation.Models.Admin;

/// <summary>
/// Adapter that wraps TnmsAdministrationPlatform's IAdminUser as ITnmsPlfdAdminUser.
/// </summary>
internal sealed class TnmsPlfdAdminUserAdapter : ITnmsPlfdAdminUser
{
    private readonly IAdminUser _user;

    public TnmsPlfdAdminUserAdapter(IAdminUser user)
    {
        _user = user;
    }

    /// <inheritdoc />
    public IGameClient Client => _user.Client;

    /// <inheritdoc />
    public IReadOnlySet<string> Permissions => _user.Permissions;

    /// <inheritdoc />
    public IReadOnlySet<ITnmsPlfdAdminGroup> Groups =>
        new HashSet<ITnmsPlfdAdminGroup>(_user.Groups.Select(g => new TnmsPlfdAdminGroupAdapter(g)));

    /// <inheritdoc />
    public byte Immunity => _user.Immunity;
}
