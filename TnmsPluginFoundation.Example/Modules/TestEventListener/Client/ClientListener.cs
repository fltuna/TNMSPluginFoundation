using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sharp.Shared.Enums;
using Sharp.Shared.Listeners;
using Sharp.Shared.Objects;

namespace TnmsPluginFoundation.Example.Modules.TestEventListener.Client;

public class ClientListener(IServiceProvider provider): IClientListener
{
    private TnmsPlugin _plugin = provider.GetRequiredService<TnmsPlugin>();
    
    public int ListenerVersion => 1;
    public int ListenerPriority => 0;

    public void OnClientConnected(IGameClient client)
    {
        _plugin.Logger.LogInformation("CLIENT CONNECTED {client}", client.Name);
    }

    public ECommandAction OnClientSayCommand(IGameClient client, bool teamOnly, bool isCommand, string commandName,
        string message)
    {
        _plugin.Logger.LogInformation("PLAYER SAY {client} {teamOnly} {isCommand} {commandName} {message}", client.Name, teamOnly, isCommand, commandName, message);
        return ECommandAction.Skipped;
    }

    public void OnClientPutInServer(IGameClient client)
    {
        _plugin.Logger.LogInformation("CLIENT PUT IN: {client}", client.Name);
    }
}