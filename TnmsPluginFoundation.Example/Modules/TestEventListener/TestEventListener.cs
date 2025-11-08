using Microsoft.Extensions.Logging;
using Sharp.Shared.Enums;
using Sharp.Shared.HookParams;
using Sharp.Shared.Listeners;
using Sharp.Shared.Types;
using Sharp.Shared.Units;
using TnmsPluginFoundation.Example.Modules.TestEventListener.Client;
using TnmsPluginFoundation.Models.Plugin;

namespace TnmsPluginFoundation.Example.Modules.TestEventListener;

public class TestEventListener(IServiceProvider serviceProvider, bool hotReload) : PluginModuleBase(serviceProvider, hotReload)
{
    public override string PluginModuleName => "TestEventListener";
    public override string ModuleChatPrefix => "TestEventListener";
    protected override bool UseTranslationKeyInModuleChatPrefix => false;
    
    private readonly ClientListener _clientListener = new(serviceProvider);

    protected override void OnInitialize()
    {
        SharedSystem.GetHookManager().TextMsg.InstallHookPre(OnTextMsg);
        
        
        // Duck patch try
        SharedSystem.GetHookManager().PlayerProcessMovePre.InstallForward((@params =>
        {
            @params.MaxSpeed = 500.0f;
            @params.Service.DuckSpeed = 8.0f;
        }));
        
        SharedSystem.GetHookManager().PostEventAbstract.InstallHookPre(((@params, value) =>
        {
            if (@params.MsgId == ProtobufNetMessageType.svc_Print)
            {
                Logger.LogInformation("Print");
            }
            return value;
        }));

        SharedSystem.GetClientManager().InstallClientListener(_clientListener);
    }
    
    protected override void OnUnloadModule()
    {
        SharedSystem.GetHookManager().TextMsg.RemoveHookPre(OnTextMsg);
        SharedSystem.GetClientManager().RemoveClientListener(_clientListener);
    }

    private HookReturnValue<NetworkReceiver> OnTextMsg(ITextMsgHookParams @params, HookReturnValue<NetworkReceiver> hookReturnValue)
    {
        Logger.LogInformation("Received: {param}", @params.Name);
        return hookReturnValue;
    }
}