using System.Collections.Generic;
using UnityEngine;


public class Stalker : MonoBehaviour
{
    [SerializeField] private StalkerAudio audioHandler;

    [SerializeField] private float updateDelay;
    [SerializeField] private float minForcedUpdateDelay;
    [SerializeField] private float minTargetMovementForUpdate;

    [SerializeField] private int visibleTilesExtraMoveCost;


    [SerializeField] private float baseMoveSpeed;
    [SerializeField] private float cMoveSpeed;

    [SerializeField] private float ravagingMoveSpeed;
    [SerializeField] private float ravageTimeMin, ravageTimeMax;
    [SerializeField] private float maxRavageSpeedAtXPercent;
    [SerializeField] private float cSprintTime;


    public StalkerState behaviourState;


    [SerializeField] private Transform target;
    [SerializeField] private Vector3 prevTargetPos;

    [SerializeField] private List<Vector3> path;

    [SerializeField] private bool drawPathGizmos = true;
    [SerializeField] private Color pathNodesColor = Color.black;

    private Vector3 roamPoint;
    private int roamPointsLeft;



    public void Init()
    {
        audioHandler.Init(this);
        //audioHandler.StartChase();

        baseMoveSpeed = cMoveSpeed;
        prevTargetPos = target.position;

        RecalculateNextPath();
    }


    [SerializeField] private bool hasActivePath;

    public bool RecalculateNextPath()
    {
        // The stalker is scared of the player while its not ravaging
        bool scaredOfPlayer = false;

        // Extra cost for pathfinder per tile that the player can see
        int _visibleTilesExtraMoveCost = 0;

        // Position to move to
        Vector3 destinationPos = target.position;


        switch (behaviourState)
        {
            case StalkerState.RoamingStart:

                scaredOfPlayer = false;
                _visibleTilesExtraMoveCost = visibleTilesExtraMoveCost;

                behaviourState = StalkerState.Roaming;
                roamPointsLeft = 3;
                roamPoint = GridManager.GetAnyWalkableNode().worldPos;

                destinationPos = roamPoint;
                break;

            case StalkerState.Roaming:

                // If stalker reached roampoint
                if (transform.position == roamPoint)
                {
                    roamPointsLeft -= 1;
                    if (roamPointsLeft == 0)
                    {
                        behaviourState = StalkerState.Stalking;
                    }
                    else
                    {
                        roamPoint = GridManager.GetAnyWalkableNode().worldPos;
                    }
                }

                scaredOfPlayer = false;
                _visibleTilesExtraMoveCost = visibleTilesExtraMoveCost;

                destinationPos = roamPoint;
                break;

            case StalkerState.Stalking:

                scaredOfPlayer = true;
                _visibleTilesExtraMoveCost = visibleTilesExtraMoveCost;

                destinationPos = target.position;
                break;

            case StalkerState.Hunting:

                scaredOfPlayer = true;
                _visibleTilesExtraMoveCost = visibleTilesExtraMoveCost;

                destinationPos = target.position;
                break;


            case StalkerState.Ravaging:

                scaredOfPlayer = false;
                _visibleTilesExtraMoveCost = 0;

                destinationPos = target.position;
                break;
        }

        hasActivePath = AStarPathfinder.TryGetPathToTarget(transform.position, destinationPos, _visibleTilesExtraMoveCost, path, scaredOfPlayer);

        return hasActivePath;
    }



    public void GetSpottedByPlayer()
    {
        if (behaviourState != StalkerState.Ravaging)
        {
            behaviourState = StalkerState.RoamingStart;
            audioHandler.OnSpotted();
        }
    }




    public float elapsedTimeSinceLastUpdate;

    public void OnUpdate(float deltaTime, bool pathUpdateQueued)
    {
        audioHandler.OnUpdate();

        elapsedTimeSinceLastUpdate += deltaTime;

        bool targetMovedEnough = Vector3.Distance(target.position, prevTargetPos) > minTargetMovementForUpdate;

        if (targetMovedEnough || elapsedTimeSinceLastUpdate > updateDelay || elapsedTimeSinceLastUpdate > minForcedUpdateDelay)
        {
            prevTargetPos = target.position;
            elapsedTimeSinceLastUpdate = 0;

            pathUpdateQueued = true;
        }



        if (hasActivePath)
        {
            MoveStalker(deltaTime, pathUpdateQueued);
        }
        else
        {
            if (pathUpdateQueued)
            {
                RecalculateNextPath();
            }

            cMoveSpeed = baseMoveSpeed;
            cSprintTime = 0;
        }
    }


    private void MoveStalker(float deltaTime, bool pathUpdateQueued)
    {
        float initialMovement = cMoveSpeed * deltaTime;
        float movementLeft = initialMovement;

        bool tileReached;


        while (movementLeft > 0)
        {
            (tileReached, movementLeft) = MoveTowardsNode(movementLeft);

            if (tileReached)
            {
                //call new path calculation if update is queued, end the movement loop
                if (pathUpdateQueued)
                {
                    RecalculateNextPath();
                    break;
                }

                //next path node
                path.RemoveAt(0);

                //if current path is completed, end the movement loop
                if (path.Count == 0)
                {
                    hasActivePath = false;
                    break;
                }
            }
        }
    }
    private (bool, float) MoveTowardsNode(float maxDistanceThisFrame)
    {
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = path[0];


        // Calculate the direction vector and its magnitude
        Vector3 vectorToTarget = targetPosition - currentPosition;

        float distanceToTarget = vectorToTarget.magnitude;



        // Check if we can reach the target in this frame
        if (distanceToTarget <= maxDistanceThisFrame)
        {
            // Move directly to the target and calculate remaining distance
            transform.position = targetPosition;

            // Calculate the remainder of speed left
            float remainder = maxDistanceThisFrame - distanceToTarget;

            return (true, remainder);
        }
        else
        {
            // Move partially towards the target with a direction to the next node
            transform.position = currentPosition + vectorToTarget.normalized * maxDistanceThisFrame;

            // No remainder since we couldn't reach the next node
            return (false, 0);
        }
    }





    private void OnDrawGizmos()
    {
        if (drawPathGizmos == true && path != null)
        {
            Gizmos.color = pathNodesColor;
            for (int i2 = 0; i2 < path.Count; i2++)
            {
                Gizmos.DrawCube(path[i2], 0.9f * GridManager.Instance.nodeSize * Vector3.one);
            }
        }
    }
}