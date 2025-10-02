using Sharp.Shared;
using Sharp.Shared.Enums;
using Sharp.Shared.Objects;

namespace TnmsExtendableTargeting;

internal static class BuiltinTargeting
{
    internal static ISharedSystem SharedSystem { get; set; } = null!;
    
    internal static bool Me(IGameClient targetClient, IGameClient? caller)
    {
        return caller?.Slot == targetClient.Slot;
    }

    internal static bool WithOutMe(IGameClient targetClient, IGameClient? caller)
    {
        return caller?.Slot != targetClient.Slot;
    }

    internal static bool Aim(IGameClient targetClient, IGameClient? caller)
    {
        // TODO()
        return false;
    }

    
    internal static bool Ct(IGameClient targetClient, IGameClient? caller)
    {
        return SharedSystem.GetEntityManager().FindPlayerPawnBySlot(targetClient.Slot)?.Team == CStrikeTeam.CT;
    }

    internal static bool Te(IGameClient targetClient, IGameClient? caller)
    {
        return SharedSystem.GetEntityManager().FindPlayerPawnBySlot(targetClient.Slot)?.Team == CStrikeTeam.TE;
    }
    
    internal static bool Spectator(IGameClient targetClient, IGameClient? caller)
    {
        return SharedSystem.GetEntityManager().FindPlayerPawnBySlot(targetClient.Slot)?.Team == CStrikeTeam.Spectator;
    }


    internal static bool Bot(IGameClient targetClient, IGameClient? caller)
    {
        return targetClient.IsFakeClient;
    }

    internal static bool Human(IGameClient targetClient, IGameClient? caller)
    {
        return !targetClient.IsFakeClient;
    }

    internal static bool Alive(IGameClient targetClient, IGameClient? caller)
    {
        return SharedSystem.GetEntityManager().FindPlayerPawnBySlot(targetClient.Slot)?.LifeState == LifeState.Alive;
    }

    internal static bool Dead(IGameClient targetClient, IGameClient? caller)
    {
        return SharedSystem.GetEntityManager().FindPlayerPawnBySlot(targetClient.Slot)?.LifeState != LifeState.Alive;
    }


    internal static bool SteamId(string param, IGameClient targetClient, IGameClient? caller)
    {
        if (!uint.TryParse(param, out var steamId))
            return false;
        
        return targetClient.SteamId.AccountId == steamId;
    }
    
    
    internal static bool UserSlot(string param, IGameClient targetClient, IGameClient? caller)
    {
        if (!byte.TryParse(param, out var slot))
            return false;

        return targetClient.Slot.AsPrimitive() == slot;
    }
}