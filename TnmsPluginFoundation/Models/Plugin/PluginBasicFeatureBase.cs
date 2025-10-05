using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sharp.Shared;
using Sharp.Shared.Objects;
using TnmsPluginFoundation.Interfaces;

namespace TnmsPluginFoundation.Models.Plugin;

/// <summary>
/// Provides basic plugin feature. you can also make custom module class from this.
/// </summary>
/// <param name="serviceProvider">Microsoft.Extensions.DependencyInjection</param>
public abstract class PluginBasicFeatureBase(IServiceProvider serviceProvider)
{
    
    /// <summary>
    /// Main plugin instance, for registering the commands, listeners, etc...
    /// </summary>
    protected readonly TnmsPlugin Plugin = serviceProvider.GetRequiredService<TnmsPlugin>();
    
    /// <summary>
    /// ModSharp Shared system
    /// </summary>
    protected readonly ISharedSystem SharedSystem = serviceProvider.GetRequiredService<TnmsPlugin>().SharedSystem;
    
    /// <summary>
    /// Logger from main plugin instance.
    /// </summary>
    protected readonly ILogger Logger = serviceProvider.GetRequiredService<TnmsPlugin>().Logger;
    
    /// <summary>
    /// Custom debug logger for simple logging. If you didn't make custom implementation of IDebugLogger, it will do nothing.
    /// </summary>
    protected readonly IDebugLogger DebugLogger = serviceProvider.GetRequiredService<IDebugLogger>();
    
    /// <summary>
    /// DI container, you can get any service that you registered in main class from this.
    /// </summary>
    protected IServiceProvider ServiceProvider { get; set; } = serviceProvider;

    /// <summary>
    /// Simple wrapper method for AbstractTnmsPluginBase::LocalizeString()
    /// </summary>
    /// <param name="player">Player instance, If null it will use server language</param>
    /// <param name="localizationKey">Localization Key</param>
    /// <param name="args">Any args that can be use ToString()</param>
    /// <returns>Translated result</returns>
    protected string LocalizeString(IGameClient? player, string localizationKey, params object[] args)
    {
        if (player == null)
            return Plugin.LocalizeString(localizationKey, args);

        return Plugin.LocalizeStringForPlayer(player, localizationKey, args);
    }
}