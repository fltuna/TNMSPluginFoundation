using System.Collections.Generic;
using Sharp.Shared.Objects;

namespace TnmsExtendableTargeting.Shared;

public interface IExtendableTargeting
{
    public const string ModSharpModuleIdentity = "TnmsExtendableTargeting";
    
    /// <summary>
    /// Single target predication delegate <br/>
    /// This is useful for single target that only requires 1 user like @aim (Because @aim is only checks caller's LOS) <br/>
    /// </summary>
    /// <param name="caller">The command executor. client or console. (this is useful for tracing client's ray when adding like @aim targeting)</param>
    public delegate IGameClient? SingleTargetPredicateDelegate(IGameClient? caller);
    
    /// <summary>
    /// Target predication delegate
    /// </summary>
    /// <param name="targetClient">The target client for testing.</param>
    /// <param name="caller">The command executor. client or console. (this is useful for tracing client's ray when adding like @aim targeting)</param>
    public delegate bool TargetPredicateDelegate(IGameClient targetClient, IGameClient? caller);
     
    /// <summary>
    /// Parameterized Target predication delegate
    /// </summary>
    /// <param name="param">Any parameters for inputted to this targeting. (e.g. if @test:100 then input is "100")</param>
    /// <param name="targetClient">The target client for testing.</param>
    /// <param name="caller">The command executor. client or console. (this is useful for tracing client's ray when adding like @aim targeting)</param>
    public delegate bool ParameterizedTargetPredicateDelegate(string param, IGameClient targetClient, IGameClient? caller);
    
    // Register custom target
    /// <summary>
    /// Register custom single targeting
    /// </summary>
    /// <param name="prefix">targeting prefix (e.g. @vip, @friends)</param>
    /// <param name="predicate"></param>
    public void RegisterCustomSingleTarget(string prefix, IExtendableTargeting.SingleTargetPredicateDelegate predicate);
     
    /// <summary>
    /// Unregister custom single target from extended targeting
    /// </summary>
    /// <param name="prefix">prefix of targeting</param>
    /// <returns>true if deleted successfully, otherwise false</returns>
    public bool UnregisterCustomSingleTarget(string prefix);
    
    // Register custom target
    /// <summary>
    /// Register custom targeting
    /// </summary>
    /// <param name="prefix">targeting prefix (e.g. @vip, @friends)</param>
    /// <param name="predicate"></param>
    public void RegisterCustomTarget(string prefix, IExtendableTargeting.TargetPredicateDelegate predicate);
     
    /// <summary>
    /// Unregister custom target from extended targeting
    /// </summary>
    /// <param name="prefix">prefix of targeting</param>
    /// <returns>true if deleted successfully, otherwise false</returns>
    public bool UnregisterCustomTarget(string prefix);
     
     // Register parameterized target
     /// <summary>
     /// Registers a custom parameterized targeting
     /// </summary>
     /// <param name="prefix">targeting prefix (e.g. @vip, @friends)</param>
     /// <param name="predicate"></param>
     public void RegisterCustomParameterizedTarget(string prefix,
         IExtendableTargeting.ParameterizedTargetPredicateDelegate predicate);
     
     /// <summary>
     /// Unregister custom parameterized target from extended targeting
     /// </summary>
     /// <param name="prefix">prefix of targeting</param>
     /// <returns>true if deleted successfully, otherwise false</returns>
     public bool UnregisterCustomParameterizedTarget(string prefix);

     // Extended target resolve
     /// <summary>
     /// Resolve extended targeting. If no custom targeting matched, then it will fallback to CS#'s default targeting system.
     /// </summary>
     /// <param name="targetString">Target string for finding targets</param>
     /// <param name="caller"></param>
     /// <param name="foundTargets">Returns filled TargetResult if found, otherwise empty TargetReuslt</param>
     /// <returns>true if at least 1 player found. otherwise false</returns>
     public bool ResolveTarget(string targetString, IGameClient? caller, out List<IGameClient> foundTargets);
}