using UnityEngine;

/// <summary>
/// Static entry point for the timer system.
/// Creates a DontDestroyOnLoad manager on first access.
/// </summary>
public static class TimerService
{
    private static ITimerManager _instance;

    public static ITimerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("[TimerManager]");
                Object.DontDestroyOnLoad(go);
                _instance = go.AddComponent<TimerManager>();
            }
            return _instance;
        }
    }

    /// <summary>
    /// Replace the current timer manager with a custom implementation.
    /// </summary>
    public static void Initialize(ITimerManager manager)
    {
        if (_instance is TimerManager oldManager && oldManager != null)
            Object.Destroy(oldManager.gameObject);
        _instance = manager;
    }

    /// <summary>
    /// Destroy the current manager and clear the instance.
    /// </summary>
    public static void Shutdown()
    {
        if (_instance is TimerManager manager)
            Object.Destroy(manager.gameObject);
        _instance = null;
    }
}