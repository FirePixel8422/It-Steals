using FirePixel.Utility;
using UnityEngine;


public class StalkerManager
{
    public static Stalker[] stalkers;

    private static bool pathUpdateQueued;

    public static void Init(Stalker[] _stalkers)
    {
        stalkers = _stalkers;

        for (int i = 0; i < stalkers.Length; i++)
        {
            stalkers[i].Init();
        }

        // Add this instance to the static list when it's created
        UpdateScheduler.RegisterUpdate(OnUpdate);
    }

    public static void QueueStalkerPathUpdates()
    {
        pathUpdateQueued = true;
    }

    private static void OnUpdate()
    {
        for (int i = 0; i < stalkers.Length; i++)
        {
            stalkers[i].OnUpdate(Time.deltaTime, pathUpdateQueued);
        }
    }
}
