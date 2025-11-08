using Sharp.Shared.Enums;
using Sharp.Shared.GameEntities;
using Sharp.Shared.Objects;

namespace TnmsPluginFoundation.Utils.Entity;

/// <summary>
/// Utility class for manipulate game entity
/// </summary>
public static class EntityUtil
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns>Returns CCSGameRules instance if found. Otherwise null</returns>
    public static IGameRules GetGameRules()
    {
        return TnmsPlugin.StaticSharedSystem.GetModSharp().GetGameRules();
    }

    /// <summary>
    /// Return a specified CsTeam's CCSTeam instance.
    /// </summary>
    /// <param name="csTeam">Team to want to obtain</param>
    /// <returns>Returns CCSTeam instance if found. Otherwise null</returns>
    public static IBaseTeam? GetTeam(CStrikeTeam csTeam)
    {
        return TnmsPlugin.StaticSharedSystem.GetEntityManager().GetGlobalCStrikeTeam(csTeam);
    }
}