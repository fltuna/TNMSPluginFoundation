using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sharp.Shared;
using TnmsPluginFoundation.Example.Modules;
using TnmsPluginFoundation.Example.Modules.LocalizationTest;
using TnmsPluginFoundation.Example.Modules.PermissionTest;
using TnmsPluginFoundation.Example.Modules.TargetingTest;
using TnmsPluginFoundation.Example.Modules.TestCommand;
using TnmsPluginFoundation.Example.Modules.TestEventListener;

namespace TnmsPluginFoundation.Example;

public class TnmsPluginFoundationExample(
    ISharedSystem sharedSystem,
    string dllPath,
    string sharpPath,
    Version? version,
    IConfiguration coreConfiguration,
    bool hotReload)
    : TnmsPlugin(sharedSystem, dllPath, sharpPath, version, coreConfiguration, hotReload)
{
    public override string DisplayName => "TnmsPluginFoundation.Example";
    public override string DisplayAuthor => "faketuna";
    public override string BaseCfgDirectoryPath => "unused";
    public override string ConVarConfigPath => Path.Combine(SharedSystem.GetModSharp().GetGamePath(), "cfg/TNMSExamplePlugin/TNMSExamplePlugin.cfg");
    public override string PluginPrefix => "TNMSPF";
    public override bool UseTranslationKeyInPluginPrefix => false;

    protected override void TnmsOnPluginLoad(bool hotReload)
    {
        RegisterModule<TestModule>(hotReload);
        RegisterModule<TestCommand>();
        RegisterModule<TnmsPermissionTest>();
        RegisterModule<TestEventListener>();
        RegisterModule<TnmsTargetingTest>();
        RegisterModule<LocalizationTest>();
        Logger.LogInformation("Hello from the TnmsPluginFoundation.Example at TnmsOnPluginLoad()");
    }

    protected override void TnmsAllPluginsLoaded(bool hotReload)
    {
        Logger.LogInformation("Hello from the TnmsPluginFoundation.Example at TnmsAllPluginsLoaded()");
    }

    protected override void TnmsOnPluginUnload(bool hotReload)
    {
        Logger.LogInformation("Hello from the TnmsPluginFoundation.Example from TnmsOnPluginUnload()");
    }
}