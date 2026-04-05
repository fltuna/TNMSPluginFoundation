using System.Collections.Generic;
using Sharp.Shared.Objects;

namespace TnmsPluginFoundation.Interfaces;

/// <summary>
/// Abstraction layer for admin user information.
/// </summary>
public interface ITnmsPlfdAdminUser
{
    /// <summary>
    /// The game client associated with this admin user
    /// </summary>
    IGameClient Client { get; }

    /// <summary>
    /// Permissions assigned to this user
    /// </summary>
    IReadOnlySet<string> Permissions { get; }

    /// <summary>
    /// Groups this user belongs to
    /// </summary>
    IReadOnlySet<ITnmsPlfdAdminGroup> Groups { get; }

    /// <summary>
    /// Immunity level (0-255)
    /// </summary>
    byte Immunity { get; }
}
