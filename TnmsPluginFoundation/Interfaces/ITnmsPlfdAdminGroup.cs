using System.Collections.Generic;

namespace TnmsPluginFoundation.Interfaces;

/// <summary>
/// Abstraction layer for admin group information.
/// </summary>
public interface ITnmsPlfdAdminGroup
{
    /// <summary>
    /// Name of the admin group
    /// </summary>
    string GroupName { get; }

    /// <summary>
    /// Permissions assigned to this group
    /// </summary>
    IReadOnlySet<string> Permissions { get; }
}
