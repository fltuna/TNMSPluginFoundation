using Sharp.Shared.Enums;
using Sharp.Shared.GameEntities;
using Sharp.Shared.Objects;
using Sharp.Shared.Types;

namespace TnmsPluginFoundation.Extensions.Client;

public static class GameClientExtension
{
    /// <summary>
    /// print chat to client
    /// </summary>
    /// <param name="client">Client to print</param>
    /// <param name="message">message</param>
    public static void PrintToChat(this IGameClient client, string message)
    {
        TnmsPlugin.StaticSharedSystem.GetModSharp().PrintChannelFilter(HudPrintChannel.Chat, message, new RecipientFilter(client));
    }

    /// <summary>
    /// I'm not sure what say text2 is
    /// </summary>
    /// <param name="client">Client to print</param>
    /// <param name="message">message</param>
    public static void PrintToSayText2(this IGameClient client, string message)
    {
        TnmsPlugin.StaticSharedSystem.GetModSharp().PrintChannelFilter(HudPrintChannel.SayText2, message, new RecipientFilter(client));
    }

    /// <summary>
    /// print center message to client (maybe HTML?)
    /// </summary>
    /// <param name="client">Client to print</param>
    /// <param name="message">message</param>
    public static void PrintToCenter(this IGameClient client, string message)
    {
        TnmsPlugin.StaticSharedSystem.GetModSharp().PrintChannelFilter(HudPrintChannel.Center, message, new RecipientFilter(client));
    }
    
    /// <summary>
    /// print hint message to client
    /// </summary>
    /// <param name="client">Client to print</param>
    /// <param name="message">message</param>
    public static void PrintToHint(this IGameClient client, string message)
    {
        TnmsPlugin.StaticSharedSystem.GetModSharp().PrintChannelFilter(HudPrintChannel.Hint, message, new RecipientFilter(client));
    }
    
    /// <summary>
    /// Get PlayerController from IGameClient
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    public static IPlayerController? GetPlayerController(this IGameClient client)
    {
        return TnmsPlugin.StaticSharedSystem.GetEntityManager().FindPlayerControllerBySlot(client.Slot);
    }

    /// <summary>
    /// Get PlayerPawn from IGameClient
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    public static IPlayerPawn? GetPlayerPawn(this IGameClient client)
    {
        return TnmsPlugin.StaticSharedSystem.GetEntityManager().FindPlayerPawnBySlot(client.Slot);
    }
}