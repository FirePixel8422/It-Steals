using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    private void Awake()
    {
        Instance = this;
    }




    public Node[,] grid;

    public LayerMask obstructionLayer;

    public Vector3 gridSize;
    public Vector3 gridPosition;

    [Range(0.1f, 5)]
    public float nodeSize;

    [HideInInspector]
    public int gridSizeX, gridSizeZ;
    public int MaxSize => gridSizeX * gridSizeZ;


    public StalkerAI stalkerAI;


    public bool drawPathGizmos = true;
    public bool drawNodeColorGizmos = false;

    public Color pathNodesColor = Color.black;
    public Color[] nodeLayerColors;


    public void Init()
    {
        stalkerAI = StalkerAI.Instance;

        CreateGrid();
    }

    public void CreateGrid()
    {
        gridSizeX = Mathf.RoundToInt(gridSize.x / nodeSize);
        gridSizeZ = Mathf.RoundToInt(gridSize.z / nodeSize);

        grid = new Node[gridSizeX, gridSizeZ];
        Vector3 worldBottomLeft = gridPosition - Vector3.right * gridSize.x / 2 - Vector3.forward * gridSize.z / 2;


        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeSize + nodeSize / 2) + Vector3.forward * (z * nodeSize + nodeSize / 2);

                bool hit = Physics.Raycast(worldPoint + Vector3.up * 10, Vector3.down, 100, obstructionLayer);

                //hit, not walkable
                if (hit)
                {
                    grid[x, z] = new Node(false, worldPoint, new int2(x, z), -10);
                }
                //no hit, walkable
                else
                {
                    grid[x, z] = new Node(true, worldPoint, new int2(x, z), 0);
                }
            }
        }
    }

    public List<Node> GetNeigbours(Node node)
    {
        List<Node> neigbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                //skip corners and 0,0
                if (math.abs(x) == math.abs(z))
                {
                    continue;
                }
                int checkX = node.gridPos.x + x;
                int checkZ = node.gridPos.y + z;

                if (checkX >= 0 && checkX < gridSizeX && checkZ >= 0 && checkZ < gridSizeZ)
                {
                    neigbours.Add(grid[checkX, checkZ]);
                }
            }
        }
        return neigbours;
    }


    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridSize.x / 2) / gridSize.x;
        float percentZ = (worldPosition.z + gridSize.z / 2) / gridSize.z;
        percentX = Mathf.Clamp01(percentX);
        percentZ = Mathf.Clamp01(percentZ);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int z = Mathf.RoundToInt((gridSizeZ - 1) * percentZ);
        return grid[x, z];
    }



    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if (drawNodeColorGizmos == true)
            {
                for (int i2 = 0; i2 < gridSizeX; i2++)
                {
                    for (int i3 = 0; i3 < gridSizeZ; i3++)
                    {
                        //Gizmos.color = Color.red;

                        //if (grid[i2, i3].layerId / 10 != -1)
                        //{
                        //    Gizmos.color = nodeLayerColors[grid[i2, i3].layerId / 10];
                        //}

                        Gizmos.color = Color.red;
                        if (grid[i2, i3].visibleByPlayer)
                        {
                            Gizmos.color = Color.green;
                        }

                        Gizmos.DrawCube(grid[i2, i3].worldPos, Vector3.one * nodeSize * 0.9f);
                    }
                }
            }

            if (drawPathGizmos == true && stalkerAI.path != null)
            {
                Gizmos.color = pathNodesColor;
                for (int i2 = 0; i2 < stalkerAI.path.Count; i2++)
                {
                    Gizmos.DrawCube(stalkerAI.path[i2], Vector3.one * nodeSize * 0.9f);
                }
            }
        }

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(gridPosition, new Vector3(gridSize.x, 0.5f, gridSize.z));
    }
}
