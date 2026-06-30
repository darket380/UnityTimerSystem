using System;
using UnityEngine;

/// <summary>
/// Internal representation of a delayed call. Used only by TimerManager.
/// </summary>
internal class DelayedCall
{
    public long Id;                     // Unique timer identifier
    public float StartTime;             // Time when the timer was created
    public float Duration;              // Original delay
    public float InvokeTime;            // Time when the timer fires
    public bool UseUnscaledTime;        // Whether to use real (unscaled) time
    public WeakReference<MonoBehaviour> TargetRef;
    public Action Action;
}