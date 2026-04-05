using Microsoft.Extensions.Configuration;
using Sharp.Shared;

namespace TnmsPluginFoundation.Example;

public class TestClass(
    ISharedSystem sharedSystem,
    string dllPath,
    string sharpPath,
    Version? version,
    IConfiguration coreConfiguration,
    bool hotReload,
    string displayName,
    string displayAuthor,
    string baseCfgDirectoryPath,
    string conVarConfigPath,
    string pluginPrefix,
    bool useTranslationKeyInPluginPrefix)
    : TnmsPlugin(sharedSystem, dllPath, sharpPath, version, coreConfiguration, hotReload)
{
    public override string DisplayName { get; } = displayName;
    public override string DisplayAuthor { get; } = displayAuthor;
    public override string BaseCfgDirectoryPath { get; } = baseCfgDirectoryPath;
    public override string ConVarConfigPath { get; } = conVarConfigPath;
    public override string PluginPrefix { get; } = pluginPrefix;
    public override bool UseTranslationKeyInPluginPrefix { get; } = useTranslationKeyInPluginPrefix;
}