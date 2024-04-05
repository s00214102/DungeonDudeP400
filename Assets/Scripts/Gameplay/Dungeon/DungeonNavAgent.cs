using UnityEngine;

public class DungeonNavAgent : MonoBehaviour {
    DungeonNavigationSystem dungeonGrid;

    private void Awake() {
        dungeonGrid = GameObject.Find("DungeonNavigation").GetComponent<DungeonNavigationSystem>();
    }
    private void Update() {
        //dungeonGrid.
        // pass the agents current position to the navigation system so its grid position can be updated
        
    }
}