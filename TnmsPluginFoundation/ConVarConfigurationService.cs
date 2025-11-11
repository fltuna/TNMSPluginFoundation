using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using Sharp.Shared.Objects;
using TnmsPluginFoundation.Extensions;

namespace TnmsPluginFoundation;

/// <summary>
/// Manages plugin ConVar configuration file and execution.
/// </summary>
/// <param name="plugin">Instance of <see cref="TnmsPlugin"/></param>
public class ConVarConfigurationService(TnmsPlugin plugin)
{
    private readonly Dictionary<string, List<IConVar>> _moduleConVars = new();
    
    /// <summary>
    /// Add ConVar to the list of ConVars to be saved in the config file.
    /// </summary>
    /// <param name="moduleName">ModuleName should be unique, otherwise overriden to new one</param>
    /// <param name="conVar">FakeConVar</param>
    public void TrackConVar(IConVar conVar, string? moduleName = null)
    {
        if (moduleName == null)
            moduleName = plugin.DisplayName;
        
        if (!_moduleConVars.TryGetValue(moduleName, out var list))
        {
            list = new List<IConVar>();
            _moduleConVars[moduleName] = list;
        }
        
        list.Add(conVar);
    }
    /// <summary>
    /// Save all module ConVar config to one file.
    /// </summary>
    public void SaveAllConfigToFile()
    {
        if (string.IsNullOrEmpty(plugin.ConVarConfigPath))
            return;
        
        if(IsFileExists(plugin.ConVarConfigPath))
            return;

        using (StreamWriter writer = new StreamWriter(plugin.ConVarConfigPath))
        {
            foreach (var moduleName in _moduleConVars.Keys)
            {
                writer.WriteLine($"// ===== {moduleName} =====");
                writer.WriteLine();
                
                foreach (var conVar in _moduleConVars[moduleName])
                {
                    writer.WriteLine($"// {conVar.HelpString}");
                    
                    if (conVar.GetType().GenericTypeArguments[0] == typeof(bool))
                    {
                        bool value = conVar.GetBool();
                        writer.WriteLine($"{conVar.Name} {Convert.ToInt32(value)}");
                    }
                    else
                    {
                        writer.WriteLine($"{conVar.Name} {conVar.GetCvarValueString()}");
                    }
                    
                    writer.WriteLine();
                }
                
                writer.WriteLine();
            }
        }
    }

    /// <summary>
    /// Execute plugin's ConVar config
    /// </summary>
    public void ExecuteConfigs()
    {
        if (string.IsNullOrEmpty(plugin.ConVarConfigPath))
            return;
        
        // Check if ConVarConfigPath is not directory, and file is not exists
        if (!Directory.Exists(plugin.ConVarConfigPath) && !File.Exists(plugin.ConVarConfigPath))
        {
            plugin.Logger.LogError("We failed to find and executing the config file. This is shouldn't be happened!");
            return;
        }

        if (File.Exists(plugin.ConVarConfigPath))
        {
            plugin.Logger.LogInformation("Executing plugin ConVar config files...");
            string configPath = GetSubPathAfterPattern(plugin.ConVarConfigPath, "game/csgo/cfg");
            TnmsPlugin.StaticSharedSystem.GetModSharp().ServerCommand($"exec {configPath}");
        }
        else
        {
            plugin.Logger.LogError("We failed to find and executing the config file. This is shouldn't be happened!");
        }
        plugin.Logger.LogInformation("ConVar config execution has been done");
    }

    private bool IsFileExists(string path)
    {
        if (string.IsNullOrEmpty(path))
            return false;
        
        string directory = Path.GetDirectoryName(path)!;
        if (!Directory.Exists(directory))
        {
            plugin.Logger.LogInformation("Failed to find the config folder. Trying to generate...");
                
            Directory.CreateDirectory(directory);

            if (!Directory.Exists(directory))
            {
                plugin.Logger.LogError($"Failed to generate the Config folder! cancelling the config generation!");
                return false;
            }
        }

        return File.Exists(path);
    }
    
    /// <summary>
    /// Remove ConVar from the list of ConVars to be saved in the config file.
    /// </summary>
    /// <param name="moduleName"></param>
    public void UntrackModule(string moduleName)
    {
        _moduleConVars.Remove(moduleName);
    }

    private static string GetSubPathAfterPattern(string fullPath, string pattern)
    {
        pattern = pattern.Replace('\\', '/').TrimEnd('/') + '/';
        fullPath = fullPath.Replace('\\', '/');
        
        int patternIndex = fullPath.LastIndexOf(pattern, StringComparison.Ordinal);
        
        if (patternIndex < 0)
        {
            return string.Empty;
        }
        
        int startPos = patternIndex + pattern.Length;
        
        if (startPos >= fullPath.Length)
        {
            return string.Empty;
        }
        
        return fullPath.Substring(startPos);
    }
}