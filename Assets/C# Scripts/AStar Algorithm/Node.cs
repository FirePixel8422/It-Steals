using UnityEngine;
using System.Collections;
using Unity.Mathematics;

public class Node : IHeapItems<Node>
{
    public int layerId;

    public bool walkable;
    public Vector3 worldPos;

    public int2 gridPos;
    public int2 parentGridPos;

    public bool visibleByPlayer;

    public int gCost;
    public int hCost;

    private int heapIndex;

    public Node(bool _walkable, Vector3 _worldPos, int2 _gridPos, int _layerId)
    {
        walkable = _walkable;
        worldPos = _worldPos;
        gridPos = _gridPos;
        layerId = _layerId;
    }

    public int FCost => gCost + hCost;


    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = FCost.CompareTo(nodeToCompare.FCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}