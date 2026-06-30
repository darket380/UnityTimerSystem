using UnityEngine;

/// <summary>
/// Immutable timer handle.
/// Allows querying state, remaining time, and cancelling the timer.
/// </summary>
public readonly struct TimerHandle
{
    private readonly ITimerManager manager;
    private readonly long id;

    internal TimerHandle(ITimerManager manager, long id)
    {
        this.manager = manager;
        this.id = id;
    }

    public bool IsActive => manager != null && manager.IsActive(id);
    public float RemainingTime => manager != null ? manager.GetRemainingTime(id) : 0f;
    public float StartTime => manager != null ? manager.GetStartTime(id) : 0f;
    public float Duration => manager != null ? manager.GetDuration(id) : 0f;
    public void Cancel() => manager?.CancelById(id);
}