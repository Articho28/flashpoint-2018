using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

    TileGrid grid;

    // Use this for initialization
    void Start() {
        grid = GetComponent<TileGrid>();
    }

    public bool isValidWalkable(Vector3 worldPosition) {
        return false;
    }

    //distance to adjacent horizontal/vertical tile is 1
    public int GetDistance(Tile tileA, Tile tileB) { //shud be private, changed it to public just for spaghetti demo code
        int distX = Mathf.Abs(tileA.gridX - tileB.gridX);
        int distY = Mathf.Abs(tileA.gridY - tileB.gridY);

        return distX + distY;
    }
}
