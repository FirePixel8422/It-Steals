using System.Collections.Generic;
using System.Diagnostics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;


public class ViewUpdater : MonoBehaviour
{
    public Camera playerCamera;
    public LayerMask wallLayer;

    public int gridWidth = 100;
    public int gridHeight = 100;

    private float3 gridOffset;


    private void Start()
    {
        gridOffset = new float3(gridWidth / 2 - 0.5f, 0, gridHeight / 2 - 0.5f);
    }

    private void Update()
    {
        sw = Stopwatch.StartNew();


        CheckVisibleTiles();

        //print(sw.ElapsedTicks);
    }

    private void CheckVisibleTiles()
    {
        // Get the camera's frustum planes
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerCamera);

        // Loop through the grid, checking which tiles are in the camera's view
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Determine the position of the tile's center in world space
                float3 tilePosition = new float3(x, 0, y) - gridOffset; // Assuming flat grid on the XZ plane

                GridManager.Instance.grid[x, y].visibleByPlayer = IsTileVisible(tilePosition, planes); 
            }
        }
    }


    private bool IsTileVisible(float3 tilePosition, Plane[] planes)
    {
        // Check if the tile is within the frustum
        // You can use the plane-based AABB check to see if the tile's bounding box intersects the frustum
        // A tile is a square with size tileSize, so we check its bounds
        Bounds tileBounds = new Bounds(tilePosition, new float3(1, 1, 1)) ; // Create a bounding box for the tile

        bool inCameraView = GeometryUtility.TestPlanesAABB(planes, tileBounds);

        if (inCameraView == false)
        {
            return false;
        }

        // Now check for obstacles between the camera and the tile
        float3 startPosition = playerCamera.transform.position;  // Start ray from the camera position

        UnityEngine.Debug.DrawRay(startPosition, tilePosition - startPosition);

        if (Physics.Raycast(startPosition, tilePosition - startPosition, out RaycastHit hit, 2.25f, wallLayer))
        {
            // If we hit something before reaching the tile, it's obstructed
            if (hit.point != (Vector3)tilePosition)
            {
                return true;
            }
        }

        return false;
    }



    private Stopwatch sw;
}
