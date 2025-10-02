using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sharp.Shared;
using Sharp.Shared.Enums;
using Sharp.Shared.Listeners;
using Sharp.Shared.Objects;
using TnmsLocalizationPlatform.Internal;
using TnmsLocalizationPlatform.Shared;

namespace TnmsLocalizationPlatform;

public class TnmsLocalizationPlatform: IModSharpModule, ITnmsLocalizationPlatform, IClientListener
{
    internal readonly ILogger Logger;
    internal readonly ISharedSystem SharedSystem;
    
    public TnmsLocalizationPlatform(ISharedSystem sharedSystem,
        string?                  dllPath,
        string?                  sharpPath,
        Version?                 version,
        IConfiguration?          coreConfiguration,
        bool                     hotReload)
    {
        SharedSystem = sharedSystem;
        Logger = sharedSystem.GetLoggerFactory().CreateLogger<TnmsLocalizationPlatform>();
        
        ArgumentNullException.ThrowIfNull(dllPath);
        ArgumentNullException.ThrowIfNull(sharpPath);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(coreConfiguration);
    }




    public int ListenerVersion => 1;
    public int ListenerPriority => 10;
    
    public string DisplayName => "TnmsLocalizationPlatform";
    public string DisplayAuthor => "faketuna";
    
    internal static TnmsLocalizationPlatform Instance { get; private set; } = null!;
    internal readonly Dictionary<byte, CultureInfo> ClientCultures = new();
    // TODO() Get ServerDefault culture from config
    internal CultureInfo ServerDefaultCulture { get; set; }  = new CultureInfo("en-US");
    
    public bool Init()
    {
        Instance = this;
        Logger.LogInformation("TnmsLocalizationPlatform initialized");
        
        return true;
    }

    public void PostInit()
    {
        SharedSystem.GetSharpModuleManager().RegisterSharpModuleInterface(this, ITnmsLocalizationPlatform.ModSharpModuleIdentity, (ITnmsLocalizationPlatform)this);
    }

    public void Shutdown()
    {
        Logger.LogInformation("TnmsLocalizationPlatform shutdown");
    }

    public ITnmsLocalizer CreateStringLocalizer(ILocalizableModule module)
    {
        // TODO() Add language data loading feature
        return new CustomStringLocalizer(new Dictionary<string, Dictionary<string, string>>());
    }


    public void OnClientConnected(IGameClient client)
    {
        SharedSystem.GetClientManager().QueryConVar(client, "cl_language", (gameClient, status, name, value) =>
        {
            if (status != QueryConVarValueStatus.ValueIntact)
                throw new InvalidOperationException("Failed to get client language");
            
            ClientCultures[gameClient.Slot] = CultureInfo.GetCultureInfo(value);
        });
    }

    public void OnClientDisconnected(IGameClient client, NetworkDisconnectionReason reason)
    {
        ClientCultures.Remove(client.Slot);
    }
    
    
}