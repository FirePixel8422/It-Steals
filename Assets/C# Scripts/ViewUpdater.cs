using Unity.Mathematics;
using UnityEngine;


public class ViewUpdater : MonoBehaviour, ICustomUpdater
{
    public Camera playerCamera;
    public LayerMask wallLayer;

    private static int gridWidth;
    private static int gridHeight;
    private static float nodeSize;

    private static float3 gridOffset;



    public static void Init(GridManager gridManager)
    {
        gridWidth = gridManager.gridSizeX;
        gridHeight = gridManager.gridSizeZ;
        nodeSize = gridManager.nodeSize;

        gridOffset = new float3(
            gridWidth * nodeSize / 2 - (nodeSize / 2),
            0,
            gridHeight * nodeSize / 2 - (nodeSize / 2)
        );
    }





    public bool requireUpdate => true;

    public void OnUpdate(float deltaTime)
    {
        // Get the camera's frustum planes
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerCamera);

        //mark tiles that are visible as visible
        UpdateVisibleTiles(planes);

        //mark spotted stalkers as seen
        //UpdateVisibleStalkers(planes);
    }


    private void UpdateVisibleTiles(Plane[] planes)
    {
        // Loop through the grid, checking which tiles are in the camera's view
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Determine the position of the tile's center in world space
                float3 tilePosition = new float3(x * nodeSize, 0, y * nodeSize) - gridOffset;

                GridManager.SetNodeVisibilityState(x, y, IsObjectVisible(tilePosition, planes)); 
            }
        }
    }


    private void UpdateVisibleStalkers(Plane[] planes)
    {
        Stalker[] stalkers = StalkerManager.stalkers;

        for (int i = 0; i < stalkers.Length; i++)
        {
            bool stalkerIsSpotted = IsObjectVisible(stalkers[i].transform.position, planes);

            if (stalkerIsSpotted)
            {
                stalkers[i].behaviourState = StalkerState.Hiding;
            }
        }
    }



    private bool IsObjectVisible(float3 objectPosition, Plane[] planes)
    {
        // Check if the tile is within the frustum
        // You can use the plane-based AABB check to see if the tile's bounding box intersects the frustum
        // A tile is a square with size tileSize, so we check its bounds
        Bounds tileBounds = new Bounds(objectPosition, new float3(nodeSize, 3, nodeSize)) ; // Create a bounding box for the tile

        bool inCameraView = GeometryUtility.TestPlanesAABB(planes, tileBounds);

        if (inCameraView == false)
        {
            return false;
        }

        // Now check for obstacles between the camera and the tile
        float3 startPosition = playerCamera.transform.position;  // Start ray from the camera position


        if (Physics.Raycast(startPosition, objectPosition - startPosition, out RaycastHit hit, 1000f, wallLayer))
        {

#if UNITY_EDITOR
            if (showObstructedRays)
            {
                Debug.DrawLine(startPosition, hit.point, Color.red);
            }
#endif

            // If we hit something before reaching the tile, it's obstructed
            if (hit.point != (Vector3)objectPosition)
            {
                return false;
            }
        }

#if UNITY_EDITOR
        else
        {
            if (showRays)
            {
                Debug.DrawRay(startPosition, objectPosition - startPosition, Color.green);
            }
        }
#endif

        return true;
    }






#if UNITY_EDITOR
    [SerializeField] private bool showRays;
    [SerializeField] private bool showObstructedRays;

    private void OnValidate()
    {
        if (showRays == false)
        {
            showObstructedRays = false;
        }
    }
#endif
}
