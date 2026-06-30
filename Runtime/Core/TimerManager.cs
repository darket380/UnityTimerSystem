using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central manager for delayed actions.
/// Supports scaled/unscaled time and weak references.
/// </summary>
public class TimerManager : MonoBehaviour, ITimerManager
{
    // All pending timers, keyed by unique ID for fast lookup and removal.
    private Dictionary<long, DelayedCall> _pendingCalls = new Dictionary<long, DelayedCall>();

    // Cached lists used each frame to avoid per-frame allocations.
    // They store the keys and calls that are due this frame.
    private List<long> _readyKeys = new List<long>();
    private List<DelayedCall> _readyCalls = new List<DelayedCall>();

    // Auto-incremented to give each timer a unique identifier.
    private long _nextId;

    private void Update()
    {
        float scaledTime = Time.time;
        float unscaledTime = Time.unscaledTime;

        _readyKeys.Clear();
        _readyCalls.Clear();

        foreach (var kvp in _pendingCalls)
        {
            DelayedCall call = kvp.Value;
            float currentTime = call.UseUnscaledTime ? unscaledTime : scaledTime;
            if (currentTime >= call.InvokeTime)
            {
                _readyKeys.Add(kvp.Key);
                _readyCalls.Add(call);
            }
        }

        for (int i = 0; i < _readyCalls.Count; i++)
        {
            DelayedCall call = _readyCalls[i];
            if (call.TargetRef.TryGetTarget(out MonoBehaviour target) && target != null)
            {
                call.Action?.Invoke();
            }
        }

        for (int i = 0; i < _readyKeys.Count; i++)
        {
            _pendingCalls.Remove(_readyKeys[i]);
        }
    }

    public TimerHandle AddDelayedAction(MonoBehaviour target, Action action, float delay, bool useUnscaledTime = false)
    {
        if (target == null)
        {
            Debug.LogWarning("TimerManager: target is null, call ignored.");
            return default;
        }

        long id = _nextId++;
        float now = useUnscaledTime ? Time.unscaledTime : Time.time;

        var call = new DelayedCall
        {
            Id = id,
            StartTime = now,
            Duration = delay,
            InvokeTime = now + delay,
            UseUnscaledTime = useUnscaledTime,
            TargetRef = new WeakReference<MonoBehaviour>(target),
            Action = action
        };

        _pendingCalls.Add(id, call);
        return new TimerHandle(this, id);
    }

    public TimerHandle AddDelayedAction<T>(T target, Action<T> action, float delay, bool useUnscaledTime = false)
        where T : MonoBehaviour
    {
        return AddDelayedAction(target, () =>
        {
            if ((UnityEngine.Object)target != null)
                action(target);
        }, delay, useUnscaledTime);
    }

    public void CancelAllForTarget(MonoBehaviour target)
    {
        List<long> toRemove = new List<long>();
        foreach (var kvp in _pendingCalls)
        {
            if (kvp.Value.TargetRef.TryGetTarget(out MonoBehaviour t) && t == target)
            {
                toRemove.Add(kvp.Key);
            }
        }

        foreach (long key in toRemove)
        {
            _pendingCalls.Remove(key);
        }
    }

    public void CancelById(long id)
    {
        _pendingCalls.Remove(id);
    }

    public void CancelAll()
    {
        _pendingCalls.Clear();
    }

    public float GetRemainingTime(long id)
    {
        if (_pendingCalls.TryGetValue(id, out var call))
        {
            float current = call.UseUnscaledTime ? Time.unscaledTime : Time.time;
            return Mathf.Max(0f, call.InvokeTime - current);
        }
        return 0f;
    }

    public float GetStartTime(long id) =>
        _pendingCalls.TryGetValue(id, out var call) ? call.StartTime : 0f;

    public float GetDuration(long id) =>
        _pendingCalls.TryGetValue(id, out var call) ? call.Duration : 0f;

    public bool IsActive(long id) =>
        _pendingCalls.ContainsKey(id);

    public List<TimerInfo> GetActiveTimersInfo()
    {
        List<TimerInfo> list = new List<TimerInfo>(_pendingCalls.Count);
        foreach (var call in _pendingCalls.Values)
        {
            bool alive = call.TargetRef.TryGetTarget(out MonoBehaviour target);
            list.Add(new TimerInfo
            {
                Id = call.Id,
                TargetName = alive ? target.name : "(destroyed)",
                StartTime = call.StartTime,
                Duration = call.Duration,
                RemainingTime = Mathf.Max(0f, call.InvokeTime - (call.UseUnscaledTime ? Time.unscaledTime : Time.time)),
                IsScaled = !call.UseUnscaledTime,
                TargetAlive = alive
            });
        }
        return list;
    }

    private void OnDestroy()
    {
        if (TimerService.Instance == this)
            TimerService.Shutdown();

        CancelAll();
    }
}