using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Sharp.Shared.Enums;

namespace TnmsPluginFoundation;

public abstract partial class TnmsPlugin
{
    private readonly Dictionary<Guid, TimerInfo> _timers = new();
    
    public Guid CreateTimer(double interval, Action callback, GameTimerFlags flags = GameTimerFlags.None)
    {
        var timerId = SharedSystem.GetModSharp().PushTimer(() =>
        {
            try
            {
                callback();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in timer callback {callbackClass}:{callerMethod}", callback.Method.DeclaringType?.Namespace, callback.Method.Name);
            }
        }, interval, flags);

        _timers[timerId] = new TimerInfo(timerId, interval, flags, SharedSystem.GetModSharp().EngineTime());

        return timerId;
    }

    public Guid CreateTimer(double interval, Func<TimerAction> callback, GameTimerFlags flags = GameTimerFlags.None)
    {
        var timerId = SharedSystem.GetModSharp().PushTimer(() =>
        {
            try
            {
                return callback();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in timer callback {callbackClass}:{callerMethod}", callback.Method.DeclaringType?.Namespace, callback.Method.Name);
                return TimerAction.Stop;
            }
        }, interval, flags);

        _timers[timerId] = new TimerInfo(timerId, interval, flags, SharedSystem.GetModSharp().EngineTime());

        return timerId;
    }
    
    public void StopTimer(Guid timerId)
    {
        if (_timers.Remove(timerId))
        {
            SharedSystem.GetModSharp().StopTimer(timerId);
        }
    }
    
    public bool IsTimerValid(Guid timerId)
    {
        return _timers.ContainsKey(timerId) && SharedSystem.GetModSharp().IsValidTimer(timerId);
    }

    public void StopAllTimers()
    {
        foreach (var timer in _timers.Values.ToArray())
        {
            StopTimer(timer.Id);
        }
        _timers.Clear();
    }

    public double GetTimersRemaining(Guid timerId)
    {
        if (!_timers.TryGetValue(timerId, out var info))
        {
            return 0;
        }

        var elapsed = SharedSystem.GetModSharp().EngineTime() - info.StartTime;
        var remaining = info.Interval - elapsed % info.Interval;

        return remaining > 0 ? remaining : 0;
    }
    
    private record TimerInfo(Guid Id, double Interval, GameTimerFlags Flags, double StartTime);
}