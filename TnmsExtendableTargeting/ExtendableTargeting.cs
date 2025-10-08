using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sharp.Shared;
using Sharp.Shared.Enums;
using Sharp.Shared.Objects;
using TnmsExtendableTargeting.Shared;

namespace TnmsExtendableTargeting;

public class ExtendableTargeting: IModSharpModule, IExtendableTargeting
{
    private readonly ILogger _logger;
    private readonly ISharedSystem _sharedSystem;
    
    public ExtendableTargeting(ISharedSystem sharedSystem,
        string?                  dllPath,
        string?                  sharpPath,
        Version?                 version,
        IConfiguration?          coreConfiguration,
        bool                     hotReload)
    {
        _sharedSystem = sharedSystem;
        _logger = sharedSystem.GetLoggerFactory().CreateLogger<ExtendableTargeting>();
        
        ArgumentNullException.ThrowIfNull(dllPath);
        ArgumentNullException.ThrowIfNull(sharpPath);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(coreConfiguration);
    }
    
    public string DisplayName => "TnmsExtendableTargeting";
    public string DisplayAuthor => "faketuna";
    
    public bool Init()
    {
        _logger.LogInformation("TnmsExtendableTargeting initialized");
        
        BuiltinTargeting.SharedSystem = _sharedSystem;
        RegisterBuiltinTargets();
        return true;
    }

    public void PostInit()
    {
        _sharedSystem.GetSharpModuleManager().RegisterSharpModuleInterface(this, IExtendableTargeting.ModSharpModuleIdentity, (IExtendableTargeting)this);
    }

    public void Shutdown()
    {
        _logger.LogInformation("TnmsExtendableTargeting shutdown");
    }

    private void RegisterBuiltinTargets()
    {
        RegisterCustomTarget("@me", BuiltinTargeting.Me);
        RegisterCustomTarget("@!me", BuiltinTargeting.WithOutMe);
        RegisterCustomSingleTarget("@aim", BuiltinTargeting.Aim);
        RegisterCustomTarget("@ct", BuiltinTargeting.Ct);
        RegisterCustomTarget("@t", BuiltinTargeting.Te);
        RegisterCustomTarget("@spec", BuiltinTargeting.Spectator);
        RegisterCustomTarget("@bot", BuiltinTargeting.Bot);
        RegisterCustomTarget("@human", BuiltinTargeting.Human);
        RegisterCustomTarget("@alive", BuiltinTargeting.Alive);
        RegisterCustomTarget("@dead", BuiltinTargeting.Dead);
    }

    // This is a '#' prefixed targeting
    private bool SharpPrefixParamTargets(string param, IGameClient targetClient, IGameClient? caller)
    {
        if (!uint.TryParse(param, out var result))
            return false;

        if (targetClient.SteamId.AccountId == result)
            return true;
        
        if (targetClient.UserId.AsPrimitive() == result)
            return true;
        
        return false;
    }

     
     // Custom target
     private readonly Dictionary<string, IExtendableTargeting.TargetPredicateDelegate> _customTargets = new(StringComparer.OrdinalIgnoreCase);
     
     // Custom single target
     private readonly Dictionary<string, IExtendableTargeting.SingleTargetPredicateDelegate> _singleTargets = new(StringComparer.OrdinalIgnoreCase);
         
     // Parameterized target
     private readonly Dictionary<string, IExtendableTargeting.ParameterizedTargetPredicateDelegate> _paramTargets = new(StringComparer.OrdinalIgnoreCase);


     public void RegisterCustomSingleTarget(string prefix, IExtendableTargeting.SingleTargetPredicateDelegate predicate)
     {
         if (!prefix.StartsWith('@'))
         {
             prefix = '@' + prefix;
         }
         
         if (CheckPrefixIsRegistered(prefix))
                throw new ArgumentException("The specified prefix is already registered. please consider using another prefix");

         _singleTargets.Add(prefix, predicate);
     }

     public bool UnregisterCustomSingleTarget(string prefix)
     {
         return _singleTargets.Remove(prefix);
     }

     public void RegisterCustomTarget(string prefix, IExtendableTargeting.TargetPredicateDelegate predicate)
     {
         if (!prefix.StartsWith('@'))
         {
             prefix = '@' + prefix;
         }

         
         if (CheckPrefixIsRegistered(prefix))
             throw new ArgumentException("The specified prefix is already registered. please consider using another prefix");

         _customTargets.TryAdd(prefix, predicate);
     }

     public bool UnregisterCustomTarget(string prefix)
     {
         return _customTargets.Remove(prefix);
     }

     public void RegisterCustomParameterizedTarget(string prefix, IExtendableTargeting.ParameterizedTargetPredicateDelegate predicate)
     {
         if (!prefix.StartsWith('@'))
         {
             prefix = '@' + prefix;
         }
         
         if (CheckPrefixIsRegistered(prefix))
             throw new ArgumentException("The specified prefix is already registered. please consider using another prefix");

         _paramTargets.TryAdd(prefix, predicate);
     }

     public bool UnregisterCustomParameterizedTarget(string prefix)
     {
         return _paramTargets.Remove(prefix);
     }

     public bool ResolveTarget(string targetString, IGameClient? caller, out List<IGameClient> foundTargets)
     {
         
         if (targetString.StartsWith('#') && targetString.Length > 1)
         {
             var param = targetString.Substring(1);
             
             List<IGameClient> players = new();
             foreach (var client in _sharedSystem.GetModSharp().GetIServer().GetGameClients())
             {
                 if (SharpPrefixParamTargets(param, client, caller))
                     players.Add(client);
             }

             foundTargets = players;
             
             return foundTargets.Any();
         }
         
         if (_singleTargets.TryGetValue(targetString, out var singlePredicate))
         {
             foundTargets = new List<IGameClient>();
             var target = singlePredicate(caller);
             if (target != null)
                 foundTargets.Add(target);
             
             return foundTargets.Any();
         }
         
         if (_customTargets.TryGetValue(targetString, out var predicate))
         {
             List<IGameClient> players = new();
             foreach (var client in _sharedSystem.GetModSharp().GetIServer().GetGameClients())
             {
                 if (predicate(client, caller))
                     players.Add(client);
             }

             foundTargets = players;
             return foundTargets.Any();
         }

         // TODO() `=` contains can be produce bug
         if (targetString.StartsWith('@') && targetString.Contains('='))
         {
             var parts = targetString.Split('=', 2);
             var prefix = parts[0];
             var param = parts[1];
             
             if (param.Length < 1)
             {
                 foundTargets = [];
                 return false;
             }

             if (_paramTargets.TryGetValue(prefix, out var paramPredicate))
             {
                 List<IGameClient> players = new();
                 foreach (var client in _sharedSystem.GetModSharp().GetIServer().GetGameClients())
                 {
                     if (paramPredicate(param, client, caller))
                         players.Add(client);
                 }
                 
                 foundTargets = players;
                 
                 return foundTargets.Any();
             }
         }
         
         List<IGameClient> nameContainedPlayers = new();
         foreach (var client in _sharedSystem.GetModSharp().GetIServer().GetGameClients())
         {
             if (client.Name.Contains(targetString, StringComparison.OrdinalIgnoreCase))
                 nameContainedPlayers.Add(client);
         }
         
         foundTargets = nameContainedPlayers;
         return foundTargets.Any();
     }
     
     
     private bool CheckPrefixIsRegistered(string prefix)
     {
         if (!prefix.StartsWith('@'))
         {
             prefix = '@' + prefix;
         }

         return _customTargets.ContainsKey(prefix) || _singleTargets.ContainsKey(prefix) || _paramTargets.ContainsKey(prefix);
     }
}