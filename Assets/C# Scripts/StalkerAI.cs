using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkerAI : MonoBehaviour
{
    public static StalkerAI Instance;
    private void Awake()
    {
        Instance = this;
    }


    public float updateDelay;
    public float minForcedUpdateDelay;
    public float minTargetMovementForUpdate;



    public Transform target;
    public Vector3 prevTargetPos;

    public List<Vector3> path;


    private void Start()
    {
        StartCoroutine(PathUpdateLoop());
    }

    private IEnumerator PathUpdateLoop()
    {
        float elapsed = 0;

        while (true)
        {
            yield return null;

            elapsed += Time.deltaTime;

            bool targetMovedEnough = Vector3.Distance(target.position, prevTargetPos) > minTargetMovementForUpdate;

            if ((targetMovedEnough && elapsed > updateDelay) || elapsed > minForcedUpdateDelay)
            {
                prevTargetPos = target.position;
                elapsed = 0;

                UpdatePath();
            }
        }
    }


    private void UpdatePath()
    {
        if (PathFinding.TryGetPathToTarget(transform.position, target.position, out path))
        {

        }
    }
}
