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
    public void LogAdminAction(IGameClient? executor, string actionDescription)
    {
        plugin.Logger.LogInformation("[AdminAction] {ExecutorName} ({ExecutorSteamId}) performed action: {ActionDescription}", PlayerUtil.GetPlayerName(executor), executor?.SteamId.ToString() ?? "N/A", actionDescription);
        
        // TODO() Store actions to the DB
    }
}