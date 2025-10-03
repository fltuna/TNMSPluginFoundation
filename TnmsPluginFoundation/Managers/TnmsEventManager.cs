using Sharp.Shared.GameEntities;
using Sharp.Shared.Objects;
using TnmsPluginFoundation.Extensions.Client;

namespace TnmsPluginFoundation.Managers;

public static class TnmsEventManager
{
    public static void PrintToCenterHtml(IPlayerController controller, string message, int duration = 5)
    {
        if (TnmsPlugin.StaticSharedSystem.GetEventManager().CreateEvent("show_survival_respawn_status", true) is not { } e)
        {
            return;
        }

        if (controller.GetGameClient() is not
            {
                IsValid: true,
                IsFakeClient: false
            }
            client)
        {
            return;
        }

        e.SetString("loc_token", message);
        e.SetInt("duration", duration);
        e.FireToClient(client);
        e.Dispose();
    }
}