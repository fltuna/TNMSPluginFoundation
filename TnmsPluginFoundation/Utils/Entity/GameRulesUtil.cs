using Sharp.Shared.Enums;
using Sharp.Shared.Types;

namespace TnmsPluginFoundation.Utils.Entity;

/// <summary>
/// Utility class for manipulate GameRules entity
/// </summary>
public class GameRulesUtil
{
    /// <summary>
    /// Get current round time
    /// </summary>
    /// <returns>Current round time</returns>
    public static int GetRoundTime()
    {
        return EntityUtil.GetGameRules().RoundTime;
    }

    /// <summary>
    /// Set current round time
    /// </summary>
    /// <param name="newRoundTime">The time to be updated</param>
    /// <returns>Updated time, if update failed it returns -1</returns>
    public static int SetRoundTime(int newRoundTime)
    {
        var gameRules = EntityUtil.GetGameRules();
        gameRules.RoundTime = newRoundTime;
        return gameRules.RoundTime;
    }

    /// <summary>
    /// Check current game state is warmup or not
    /// </summary>
    /// <returns>Returns true if in warmup. Otherwise false</returns>
    public static bool IsWarmup()
    {
        return EntityUtil.GetGameRules().IsWarmupPeriod;
    }

    /// <summary>
    /// Check current state is freeze period (e.g. freeze time to buy weapons) or not
    /// </summary>
    /// <returns>Returns true if in freeze period. Otherwise false</returns>
    public static bool IsFreezePeriod()
    {
        return EntityUtil.GetGameRules().IsFreezePeriod;
    }

    
    /// <summary>
    /// Terminates current round
    /// </summary>
    /// <returns></returns>
    public static void TerminateRound(float delay, RoundEndReason reason, bool bypassHook = false, TeamRewardInfo[]? info = null)
    {
        EntityUtil.GetGameRules().TerminateRound(delay, reason);
    }
}