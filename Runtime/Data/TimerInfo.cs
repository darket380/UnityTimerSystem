/// <summary>
/// Information about an active timer for debugging/UI.
/// </summary>
public struct TimerInfo
{
    public long Id;
    public string TargetName;
    public float StartTime;
    public float Duration;
    public float RemainingTime;
    public bool IsScaled;       // true if depends on Time.timeScale
    public bool TargetAlive;    // whether the target MonoBehaviour is still alive
}