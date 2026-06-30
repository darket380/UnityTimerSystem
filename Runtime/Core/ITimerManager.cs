using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Public contract for the timer manager.
/// Provides delayed actions, cancellation, and querying of timer state.
/// </summary>
public interface ITimerManager
{
    /// <summary>
    /// Adds a delayed action.
    /// </summary>
    /// <returns>Timer handle for checking status and canceling.</returns>
    TimerHandle AddDelayedAction(MonoBehaviour target, Action action, float delay, bool useUnscaledTime = false);

    /// <summary>
    /// Typed overload that captures a reference to the target in the closure.
    /// </summary>
    TimerHandle AddDelayedAction<T>(T target, Action<T> action, float delay, bool useUnscaledTime = false) where T : MonoBehaviour;

    /// <summary>
    /// Removes all timers associated with the specified target.
    /// </summary>
    void CancelAllForTarget(MonoBehaviour target);

    /// <summary>
    /// Cancels a timer by its identifier.
    /// </summary>
    void CancelById(long id);

    /// <summary>
    /// Cancels all pending timers.
    /// </summary>
    void CancelAll();

    /// <summary>
    /// Returns a list with information about all active timers.
    /// Useful for debugging, editor or UI.
    /// </summary>
    List<TimerInfo> GetActiveTimersInfo();

    /// <summary>
    /// Returns the time remaining until the timer fires. If the timer doesn't exist, returns 0.
    /// </summary>
    float GetRemainingTime(long id);

    /// <summary>
    /// Returns the time when the timer was created. If the timer doesn't exist, returns 0.
    /// </summary>
    float GetStartTime(long id);

    /// <summary>
    /// Returns the original delay of the timer. If the timer doesn't exist, returns 0.
    /// </summary>
    float GetDuration(long id);

    /// <summary>
    /// Returns true if a timer with the given id is currently active.
    /// </summary>
    bool IsActive(long id);
}