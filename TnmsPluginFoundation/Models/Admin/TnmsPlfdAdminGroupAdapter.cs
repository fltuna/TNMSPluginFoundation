using System.Collections.Generic;
using TnmsAdministrationPlatform.Shared;
using TnmsPluginFoundation.Interfaces;

namespace TnmsPluginFoundation.Models.Admin;

/// <summary>
/// Adapter that wraps TnmsAdministrationPlatform's IAdminGroup as ITnmsPlfdAdminGroup.
/// </summary>
internal sealed class TnmsPlfdAdminGroupAdapter : ITnmsPlfdAdminGroup
{
    private readonly IAdminGroup _group;

    public TnmsPlfdAdminGroupAdapter(IAdminGroup group)
    {
        _group = group;
    }

    /// <inheritdoc />
    public string GroupName => _group.GroupName;

    /// <inheritdoc />
    public IReadOnlySet<string> Permissions => _group.Permissions;
}
