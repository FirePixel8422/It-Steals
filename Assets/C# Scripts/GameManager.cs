using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private void Awake()
    {
        Instance = this;
    }


    [Header("Start Capicity of ICustomUpdater List")]
    public int updateListPreSizeCap;

    [Header("Auto Find all Scene Object with ICustomUpdaters")]
    public bool autoFillUpdateList;




    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        GridManager _gridManager = GridManager.Instance;
        _gridManager.Init();

        ViewUpdater.Init(_gridManager);
        AStarPathfinder.Init(_gridManager);


        Stalker[] stalkers = this.FindObjectsOfType<Stalker>();

        StalkerManager.Init(stalkers);
    }
}
