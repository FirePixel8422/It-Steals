using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class StalkerAI : MonoBehaviour
{
    public static StalkerAI Instance;
    private void Awake()
    {
        Instance = this;
    }


    [SerializeField] private float updateDelay;
    [SerializeField] private float minForcedUpdateDelay;
    [SerializeField] private float minTargetMovementForUpdate;

    [SerializeField] private int visibleExtraMoveTileCost;


    private float baseMoveSpeed;
    [SerializeField] private float cMoveSpeed;
    [SerializeField] private float sprintMoveSpeed;
    [SerializeField] private float timeForFullSprint;
    [SerializeField] private float cSprintTime;



    [SerializeField] private Transform target;
    [SerializeField] private Vector3 prevTargetPos;

    [SerializeField] private List<Vector3> path;



    [SerializeField] private bool drawPathGizmos = true;
    [SerializeField] private Color pathNodesColor = Color.black;




    public void Init()
    {
        baseMoveSpeed = cMoveSpeed;
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


    public void UpdatePath()
    {
        if (PathFinding.TryGetPathToTarget(transform.position, target.position, visibleExtraMoveTileCost, path))
        {

        }
    }


    private void Update()
    {
        if (path != null && path.Count > 0)
        {
            float initialMovement = cMoveSpeed * Time.deltaTime;
            float movementLeft = initialMovement;

            bool movedInStraightLine;


            while (true)
            {
                (movementLeft, movedInStraightLine) = MoveToNewTile(movementLeft);

                cSprintTime += (movedInStraightLine ? Time.deltaTime : -Time.deltaTime) * (initialMovement - movementLeft);

                cMoveSpeed = math.lerp(baseMoveSpeed, sprintMoveSpeed, math.clamp(cSprintTime, 0, 1) / timeForFullSprint);

                if (movementLeft == 0)
                {
                    break;
                }
            }
        }
    }



    [SerializeField] private Vector3 prevMoveDirection;

    /// <returns>The distance left to move this frame</returns>
    public (float, bool) MoveToNewTile(float maxDistanceThisFrame)
    {
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = path[0];


        // Calculate the direction vector and its magnitude
        Vector3 vectorToTarget = targetPosition - currentPosition;

        bool movingInStraightLine = vectorToTarget.normalized == prevMoveDirection;
        prevMoveDirection = vectorToTarget.normalized;


        float distanceToTarget = vectorToTarget.magnitude;


        // Check if we can reach the target in this frame
        if (distanceToTarget <= maxDistanceThisFrame)
        {
            // Move directly to the target and calculate remaining distance
            transform.position = targetPosition;

            // Calculate the remainder of speed left
            float remainder = maxDistanceThisFrame - distanceToTarget;

            //next path node
            path.RemoveAt(0);


            //reached target, call return in updateloop
            if (path.Count == 0)
            {
                return (0, movingInStraightLine);
            }

            return (remainder, movingInStraightLine);
        }
        else
        {
            // Move partially towards the target
            transform.position = currentPosition + vectorToTarget.normalized * maxDistanceThisFrame;

            // No remainder since we couldn't reach the target
            return (0, movingInStraightLine);
        }
    }



    private void OnDrawGizmos()
    {
        if (drawPathGizmos == true && path != null)
        {
            Gizmos.color = pathNodesColor;
            for (int i2 = 0; i2 < path.Count; i2++)
            {
                Gizmos.DrawCube(path[i2], Vector3.one * GridManager.Instance.nodeSize * 0.9f);
            }
        }
    }
}
