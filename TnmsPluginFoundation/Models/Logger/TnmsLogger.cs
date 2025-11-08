using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sharp.Shared.Objects;
using TnmsPluginFoundation.Extensions.Client;
using TnmsPluginFoundation.Interfaces;
using TnmsPluginFoundation.Utils.Entity;

namespace TnmsPluginFoundation.Models.Logger;

public sealed class TnmsLogger(TnmsPlugin plugin)
{
    public void LogAdminActionLocalized(IGameClient? executor, string descriptionTranslationKey, params object[] descriptionParams)
    {
        string localizedActionDescription = plugin.LocalizeString(descriptionTranslationKey, descriptionParams);
        plugin.Logger.LogInformation("[AdminAction] {ExecutorName} ({ExecutorSteamId}) performed action: {LocalizedActionDescription}", PlayerUtil.GetPlayerName(executor), executor?.SteamId.ToString() ?? "N/A", localizedActionDescription);
        
        foreach (var gameClient in plugin.SharedSystem.GetModSharp().GetIServer().GetGameClients())
        {
            if (gameClient.IsFakeClient || gameClient.IsHltv)
                continue;
            
            // TODO: add permission check for manipulating details shown in admin action log
            
            var msg =
                $"{plugin.GetPluginPrefix(gameClient)} {PlayerUtil.GetPlayerName(executor)}: {plugin.LocalizeStringForPlayer(gameClient, descriptionTranslationKey, descriptionParams)}";
            
            gameClient.GetPlayerController()?.PrintToChat(msg);
        }
    }
    
    public void LogAdminAction(IGameClient? executor, string actionDescription)
    {
        plugin.Logger.LogInformation("[AdminAction] {ExecutorName} ({ExecutorSteamId}) performed action: {ActionDescription}", PlayerUtil.GetPlayerName(executor), executor?.SteamId.ToString() ?? "N/A", actionDescription);
        
        // TODO() Store actions to the DB
    }
}