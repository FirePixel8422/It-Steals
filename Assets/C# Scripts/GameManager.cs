using UnityEngine;


public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        GridManager _gridManager = GridManager.Instance;
        _gridManager.Init();

        ViewUpdater.Init(_gridManager);
        AStarPathfinder.Init(_gridManager);

        Stalker[] stalkers = this.FindObjectsOfType<Stalker>();
        StalkerManager.Init(stalkers);
    }


    private void OnDestroy()
    {
        StalkerManager.OnDestroy();
    }
}
