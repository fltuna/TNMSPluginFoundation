using System;
using Sharp.Shared.Enums;
using Sharp.Shared.Objects;
using Sharp.Shared.Types;
using TnmsPluginFoundation.Extensions.Client;

namespace TnmsPluginFoundation.Models.Plugin;


/// <summary>
/// Translation Feature Base
/// </summary>
/// <param name="serviceProvider"></param>
public class PluginTranslatableFeatureBase(IServiceProvider serviceProvider) : PluginBasicFeatureBase(serviceProvider)
{
    
    /// <summary>
    /// Helper method for sending localized text to all players.
    /// </summary>
    /// <param name="localizationKey">Language localization key</param>
    /// <param name="args">Any args that can be use ToString()</param>
    protected void PrintLocalizedChatToAll(string localizationKey, params object[] args)
    {
        foreach (var client in Plugin.SharedSystem.GetModSharp().GetIServer().GetGameClients())
        {
            if (client.IsFakeClient || client.IsHltv)
                continue;
            
            Plugin.SharedSystem.GetModSharp().PrintChannelFilter(HudPrintChannel.Chat, GetTextWithPluginPrefix(client, LocalizeString(client, localizationKey, args)), new RecipientFilter(client));
        }
    }
    /// <summary>
    /// Helper method for sending localized text to all players.
    /// </summary>
    /// <param name="localizationKey">Language localization key</param>
    protected void PrintLocalizedChatToAll(string localizationKey)
    {
        foreach (var client in Plugin.SharedSystem.GetModSharp().GetIServer().GetGameClients())
        {
            if (client.IsFakeClient || client.IsHltv)
                continue;
            
            Plugin.SharedSystem.GetModSharp().PrintChannelFilter(HudPrintChannel.Chat, GetTextWithPluginPrefix(client, LocalizeString(client, localizationKey)), new RecipientFilter(client));
        }
    }

    
    /// <summary>
    /// Prints message to server or player's chat
    /// </summary>
    /// <param name="player">Player Instance. if null message will print to server console</param>
    /// <param name="message">Message text</param>
    protected void PrintMessageToServerOrPlayerChat(IGameClient? player, string message)
    {
        if (player == null)
        {
            Console.WriteLine(message);
            return;
        }

        var playerController = player.GetPlayerController();
        
        if (playerController == null)
            Console.WriteLine(message);
        else
            playerController.PrintToChat(message);
    }
    
    
    /// <summary>
    /// Helper method for obtain the localized text.
    /// </summary>
    /// <param name="player">Player instance, If null it will use server language</param>
    /// <param name="localizationKey">Language localization key</param>
    /// <param name="args">Any args that can be use ToString()</param>
    /// <returns></returns>
    protected string LocalizeWithPluginPrefix(IGameClient? player, string localizationKey, params object[] args)
    {
        return GetTextWithPluginPrefix(player, LocalizeString(player, localizationKey, args));
    }
    
    /// <summary>
    /// Helper method for obtain the localized text.
    /// </summary>
    /// <param name="player">Player instance, If null it will use server language</param>
    /// <param name="localizationKey">Language localization key</param>
    /// <returns></returns>
    protected string LocalizeWithPluginPrefix(IGameClient? player, string localizationKey)
    {
        return GetTextWithPluginPrefix(player, LocalizeString(player, localizationKey));
    }

    
    /// <summary>
    /// Get text with plugin prefix.
    /// </summary>
    /// <param name="player">Player instance, If null it will use server language</param>
    /// <param name="text">original text</param>
    /// <returns>Text combined with original text and prefix, returns translated plugin prefix if Plugin.UseTranslationKeyInPluginPrefix is true</returns>
    protected string GetTextWithPluginPrefix(IGameClient? player, string text)
    {
        if (!Plugin.UseTranslationKeyInPluginPrefix)
            return $" {Plugin.PluginPrefix} {text}";
        
        return $" {LocalizeString(player, Plugin.PluginPrefix)} {text}";
    }
}