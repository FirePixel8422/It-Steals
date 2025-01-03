using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using UnityEngine;

public static class CustomUpdaterManager
{
    public static List<ICustomUpdater> updateStack;

    public static void AddUpdater(ICustomUpdater newEntry)
    {
        updateStack.Add(newEntry);
    }



    public static void Init(int updateListPreSizeCap, bool searchSceneForTargets = false)
    {
        //search for updateInterfaces in scene and convert to updatestack list
        if (searchSceneForTargets)
        {
            updateStack = FindAllInterfacesOfType<ICustomUpdater>();
        }

        //set capacity of updatestack list even after filling it
        if (searchSceneForTargets == false || updateStack.Count < updateListPreSizeCap)
        {
            if (updateStack == null)
            {
                updateStack = new List<ICustomUpdater>(updateListPreSizeCap);
            }
            else
            {
                updateStack.Capacity = updateListPreSizeCap;
            }
        }

        GameManager.Instance.StartCoroutine(UpdateLoop());
    }

    public static List<T> FindAllInterfacesOfType<T>()
    {
        return Object
            .FindObjectsOfType<MonoBehaviour>() // Find all MonoBehaviours
            .OfType<T>()                        // Filter by interface
            .ToList();                         // Convert to an array
    }



    private static IEnumerator UpdateLoop()
    {
        while (true)
        {
            yield return null;

            Update_UpdateStack(Time.deltaTime);
        }
    }

    
    private static void Update_UpdateStack(float deltaTime)
    {
        for (int i = 0; i < updateStack.Count; i++)
        {
            if (updateStack[i].requireUpdate)
            {
                updateStack[i].OnUpdate(deltaTime);
            }
        }
    }
}
