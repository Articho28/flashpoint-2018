using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateManager : MonoBehaviour {
    #region Singleton
    public static StateManager instance;

    private void Awake() {
        instance = this;
    }

    #endregion

    [SerializeField] GameObject tileGridObject;
    TileGrid grid;
    Pathfinding pathfinder;

    // Use this for initialization
    void Start () {
        grid = tileGridObject.GetComponent<TileGrid>();
        pathfinder = tileGridObject.GetComponent<Pathfinding>();
	}

    // THIS LAST MINUTE CODE IS FOR DEMO ONLY(DELETE THIS GARBAGE AFTER), THESE STUFF SHUD BE ELSEWHERE
    Firefighter firefighter;
	void Update () {
        if (FirefighterManager.instance.firefighters.Count == 0) {
            if (Input.GetMouseButtonDown(0)) {
                Debug.Log("mouse clicked");
                Vector3 cameraPos = Camera.main.transform.position;
                Vector3 mousePos = Input.mousePosition + new Vector3(0, 0, -cameraPos.z);
                Vector3 mousePosInWorldPoint = Camera.main.ScreenToWorldPoint(mousePos);
                Tile tile = grid.WorldPointToTile(mousePosInWorldPoint);

                if (tile.isOutside) {
                    FirefighterManager.instance.AddFirefighter(tile.worldPosition);
                }
            }
        }
        else {
            if (Input.GetMouseButtonDown(0)) {
                Debug.Log("mouse clicked");
                Vector3 cameraPos = Camera.main.transform.position;
                Vector3 mousePos = Input.mousePosition + new Vector3(0, 0, -cameraPos.z);
                Vector3 mousePosInWorldPoint = Camera.main.ScreenToWorldPoint(mousePos);
                Tile targetTile = grid.WorldPointToTile(mousePosInWorldPoint);

                firefighter = FirefighterManager.instance.firefighters[0];
                Vector3 firefighterPos = firefighter.transform.position;
                Tile currentTile = grid.WorldPointToTile(firefighterPos);

                int dist = pathfinder.GetDistance(currentTile, targetTile);
                if (firefighter.actionPoints != 0 && firefighter.actionPoints >= dist) {
                    firefighter.transform.position = targetTile.worldPosition;
                    firefighter.actionPoints -= dist;
                    Debug.Log("remaining AP: " + firefighter.actionPoints);
                }
            }
        }

      }

}
