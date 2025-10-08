using Sharp.Shared.Enums;
using Sharp.Shared.Objects;
using Sharp.Shared.Types;
using TnmsPluginFoundation.Extensions.Client;
using TnmsPluginFoundation.Models.Command;
using TnmsPluginFoundation.Utils.Entity;

namespace TnmsPluginFoundation.Example.Modules.UtilitiyTest;

public class UtilitiyTestCommand(IServiceProvider provider) : TnmsAbstractCommandBase(provider)
{
    // ms_ is automatically declared so we don't need to specify here
    public override string CommandName => "ut";
    public override string CommandDescription => "";
    public override TnmsCommandRegistrationType CommandRegistrationType { get; } = TnmsCommandRegistrationType.Client;

    protected override void ExecuteCommand(IGameClient? player, StringCommand commandInfo, ValidatedArguments? validatedArguments)
    {
        if (player == null)
            return;

        var controller = player.GetPlayerController();
        if (controller == null)
            return;
        
        controller.PrintToChat("Utility Test Command Start");

        controller.PrintToChat("-- Team Name Test --");
        CsTeamUtil.SetTeamName(CStrikeTeam.CT, "Test CT");
        CsTeamUtil.SetTeamName(CStrikeTeam.TE, "Test T");

        controller.PrintToChat("-- Team Score Test --");
        CsTeamUtil.SetTeamScore(CStrikeTeam.CT, 100);
        CsTeamUtil.SetTeamScore(CStrikeTeam.TE, 50);

        var gameRules = EntityUtil.GetGameRules();
        var before = gameRules.RoundTime;
        GameRulesUtil.SetRoundTime(5000);
        var after = gameRules.RoundTime;
        controller.PrintToChat($"-- Round Time Test --\nBefore: {before}\nAfter: {after}");

        controller.PrintToChat("-- Round Terminate is round ended? --");
        GameRulesUtil.TerminateRound(5.0f, RoundEndReason.BombDefused);


        controller.PrintToChat($"-- Is freeze? {GameRulesUtil.IsFreezePeriod()} --");
        
        controller.PrintToChat($"-- Is warmup? {GameRulesUtil.IsWarmup()} --");


        PlayerUtil.SetPlayerBuyZoneStatus(player, true);
    }
}