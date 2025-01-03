using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public class StalkerManager : ICustomUpdater
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
        CustomUpdaterManager.AddUpdater(new StalkerManager());
    }


    public static void QueueStalkerPathUpdates()
    {
        pathUpdateQueued = true;
    }



    //only call update if there are stalkers to update
    public bool requireUpdate => stalkers.Length > 0;


    [BurstCompile]
    public void OnUpdate(float deltaTime)
    {
        for (int i = 0; i < stalkers.Length; i++)
        {
            stalkers[i].OnUpdate(deltaTime, pathUpdateQueued);
        }
    }
}
