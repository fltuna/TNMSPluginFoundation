using Sharp.Shared.Enums;
using Sharp.Shared.GameEntities;
using Sharp.Shared.Objects;
using Sharp.Shared.Types;
using TnmsPluginFoundation.Managers;

namespace TnmsPluginFoundation.Extensions.Client;

public static class PlayerControllerExtension
{
    /// <summary>
    /// print chat to client
    /// </summary>
    /// <param name="client">Client to print</param>
    /// <param name="message">message</param>
    /// <param name="param1"></param>
    /// <param name="param2"></param>
    /// <param name="param3"></param>
    /// <param name="param4"></param>
    public static void PrintToChat(this IPlayerController client, string message, string? param1 = null, string? param2 = null, string? param3 = null, string? param4 = null)
    {
        client.Print(HudPrintChannel.Chat, message, param1, param2, param3, param4);
    }

    /// <summary>
    /// I'm not sure what say text2 is
    /// </summary>
    /// <param name="client">Client to print</param>
    /// <param name="message">message</param>
    /// <param name="param1"></param>
    /// <param name="param2"></param>
    /// <param name="param3"></param>
    /// <param name="param4"></param>
    public static void PrintToSayText2(this IPlayerController client, string message, string? param1 = null, string? param2 = null, string? param3 = null, string? param4 = null)
    {
        client.Print(HudPrintChannel.SayText2, message, param1, param2, param3, param4);
    }

    /// <summary>
    /// print center message to client (maybe HTML?)
    /// </summary>
    /// <param name="client">Client to print</param>
    /// <param name="message">message</param>
    /// <param name="param1"></param>
    /// <param name="param2"></param>
    /// <param name="param3"></param>
    /// <param name="param4"></param>
    public static void PrintToCenter(this IPlayerController client, string message, string? param1 = null, string? param2 = null, string? param3 = null, string? param4 = null)
    {
        client.Print(HudPrintChannel.Center, message, param1, param2, param3, param4);
    }
    
    /// <summary>
    /// print hint message to client
    /// </summary>
    /// <param name="client">Client to print</param>
    /// <param name="message">message</param>
    /// <param name="param1"></param>
    /// <param name="param2"></param>
    /// <param name="param3"></param>
    /// <param name="param4"></param>
    public static void PrintToHint(this IPlayerController client, string message, string? param1 = null, string? param2 = null, string? param3 = null, string? param4 = null)
    {
        client.Print(HudPrintChannel.Hint, message, param1, param2, param3, param4);
    }

    /// <summary>
    /// print Center HTML message to client
    /// </summary>
    /// <param name="client">Client to print</param>
    /// <param name="message">message</param>
    /// <param name="duration"></param>
    public static void PrintToCenterHtml(this IPlayerController client, string message, int duration = 5)
    {
        TnmsEventManager.PrintToCenterHtml(client, message, duration);
    }


    public static IGameClient? GetGameClient(this IPlayerController playerController)
    {
        return TnmsPlugin.StaticSharedSystem.GetClientManager().GetGameClient(playerController.SteamId);
    }
}