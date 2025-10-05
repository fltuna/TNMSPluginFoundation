using System;
using System.Collections.Generic;
using Sharp.Shared.Definition;

namespace TnmsLocalizationPlatform.Util;

public static class ChatColorUtil
{
    private static readonly Dictionary<string, string> ColorMappings = new(StringComparer.OrdinalIgnoreCase)
    {
        { "{WHITE}", ChatColor.White },
        { "{DEFAULT}", ChatColor.White },
        { "{DARKRED}", ChatColor.DarkRed },
        { "{PINK}", ChatColor.Pink },
        { "{GREEN}", ChatColor.Green },
        { "{LIGHTGREEN}", ChatColor.LightGreen },
        { "{LIME}", ChatColor.Lime },
        { "{RED}", ChatColor.Red },
        { "{GREY}", ChatColor.Grey },
        { "{GRAY}", ChatColor.Grey },
        { "{YELLOW}", ChatColor.Yellow },
        { "{GOLD}", ChatColor.Gold },
        { "{SILVER}", ChatColor.Silver },
        { "{BLUE}", ChatColor.Blue },
        { "{DARKBLUE}", ChatColor.DarkBlue },
        { "{PURPLE}", ChatColor.Purple },
        { "{LIGHTRED}", ChatColor.LightRed },
        { "{MUTED}", ChatColor.Muted },
        { "{HEAD}", ChatColor.Head }
    };

    public static string FormatChatMessage(string message)
    {
        return ProcessColorCodes(message);
    }

    private static string ProcessColorCodes(string message)
    {
        if (!message.Contains('{')) return message;
    
        var result = message;
        foreach (var mapping in ColorMappings)
        {
            result = result.Replace(mapping.Key, mapping.Value, StringComparison.OrdinalIgnoreCase);
        }
        return result;
    }
}
