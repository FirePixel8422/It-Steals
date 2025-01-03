using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;


public interface ICustomUpdater
{
    public bool requireUpdate { get; }

    [BurstCompile]
    public void OnUpdate(float deltaTime);
}
