using Sharp.Shared.Enums;

namespace TnmsPluginFoundation.Utils.Entity;

/// <summary>
/// Utility class for manipulate CCSTeam entity
/// </summary>
public static class CsTeamUtil
{
    
    /// <summary>
    /// Set team name to specified string
    /// </summary>
    /// <param name="team">Team ID</param>
    /// <param name="teamName">The name of team</param>
    /// <returns>Returns true if successfully set. Otherwise false</returns>
    public static bool SetTeamName(CStrikeTeam team, string teamName)
    {
        if (team != CStrikeTeam.TE && team != CStrikeTeam.CT)
            return false;

        string cmd;
        if (team == CStrikeTeam.TE)
        {
            cmd = $"mp_teamname_1 {teamName}";
        }
        else
        {
            cmd = $"mp_teamname_2 {teamName}";
        }

        TnmsPlugin.StaticSharedSystem.GetModSharp().ServerCommand(cmd);
        return true;
    }


    /// <summary>
    /// Set team score to specified value
    /// </summary>
    /// <param name="team">Team ID</param>
    /// <param name="score">The score of team</param>
    /// <returns>Returns true if successfully set. Otherwise false</returns>
    public static bool SetTeamScore(CStrikeTeam team, int score)
    {
        if (team != CStrikeTeam.TE && team != CStrikeTeam.CT)
            return false;
        
        var teamEntity = EntityUtil.GetTeam(team);
        if (teamEntity == null)
            return false;
        
        teamEntity.Score = score;
        return true;
    }

    
    /// <summary>
    /// Set team logo to specified string
    /// </summary>
    /// <param name="team">Team ID</param>
    /// <param name="logo">The name of logo</param>
    /// <returns>Returns true if successfully set. Otherwise false</returns>
    public static bool SetTeamLogo(CStrikeTeam team, string logo)
    {
        if (team != CStrikeTeam.TE && team != CStrikeTeam.CT)
            return false;

        string cmd;
        if (team == CStrikeTeam.CT)
        {
            cmd = $"mp_teamlogo_1 {logo}";
        }
        else
        {
            cmd = $"mp_teamlogo_2 {logo}";
        }
        
        TnmsPlugin.StaticSharedSystem.GetModSharp().ServerCommand(cmd);
        return true;
    }
}