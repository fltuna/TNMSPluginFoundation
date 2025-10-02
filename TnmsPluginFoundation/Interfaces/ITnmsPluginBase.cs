﻿namespace TnmsPluginFoundation.Interfaces;

/// <summary>
/// Required basic feature of TnmsPlugin base.
/// </summary>
internal interface ITnmsPluginBase
{
    /// <summary>
    /// ConVarConfigurationService for managing ConVar config.
    /// </summary>
    public ConVarConfigurationService ConVarConfigurationService { get; }
    
    /// <summary>
    /// DebugLogger instance
    /// </summary>
    public IDebugLogger? DebugLogger { get; }
    
    /// <summary>
    /// Base config directory path
    /// </summary>
    public string BaseCfgDirectoryPath { get; }
    
    /// <summary>
    /// ConVar configuration path.
    /// </summary>
    public string ConVarConfigPath { get; }
    
    /// <summary>
    /// This prefix used for printing to chat.
    /// </summary>
    public string PluginPrefix { get; }
    
    /// <summary>
    /// Localize language key and format args with plugin prefix
    /// </summary>
    /// <param name="languageKey">string key</param>
    /// <param name="args">Any args that supports ToString()</param>
    /// <returns>Formatted message</returns>
    public string LocalizeStringWithPluginPrefix(string languageKey, params object[] args);
    
    /// <summary>
    /// Simply localize string, same as AnyPlugin.Localizer[languageKey, args]
    /// </summary>
    /// <param name="languageKey">string key</param>
    /// <param name="args">Any args that supports ToString()</param>
    /// <returns>Translated message</returns>
    public string LocalizeString(string languageKey, params object[] args);
}