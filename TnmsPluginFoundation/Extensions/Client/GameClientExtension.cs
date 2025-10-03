using Sharp.Shared.Enums;
using Sharp.Shared.GameEntities;
using Sharp.Shared.Objects;
using Sharp.Shared.Types;

namespace TnmsPluginFoundation.Extensions.Client;

public static class GameClientExtension
{
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