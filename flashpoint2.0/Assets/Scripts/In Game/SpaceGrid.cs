using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceGrid : MonoBehaviour {

    public Transform firefighter;
    public Vector2 gridWorldSize;
    Space[,] grid;

    [SerializeField] float spaceRadius;
    float spaceDiameter;
    int gridSizeX, gridSizeY;

    private void Start() {
        spaceDiameter = spaceRadius * 2;
        gridSizeX = 10;
        gridSizeY = 8;
        CreateGrid();
    }

    private void CreateGrid() {
        grid = new Space[gridSizeX, gridSizeY];

        Vector2 worldBottomLeft = new Vector2(transform.position.x, transform.position.y) -
            Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++) {
            for (int y = 0; y < gridSizeY; y++) {
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * spaceDiameter + spaceRadius)
                                                              + Vector2.up * (y * spaceDiameter + spaceRadius);
                bool isOutsideSpace = false;

                if (x == 0 || x == gridSizeX - 1 || y == 0 || y == gridSizeY - 1) {
                    isOutsideSpace = true;
                }
                grid[x, y] = new Space(worldPoint, isOutsideSpace, x, y);
            }
        }
    }

    public List<Space> GetNeighbours(Space tile) {
        List<Space> neighbours = new List<Space>();

        //  _________
        // |__|__|__|
        // |__|_c|__| 
        // |__|__|__| 
        //search neighbour tiles of c
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0) continue; //continue if its the middle tile

                int checkX = tile.indexX + x;
                int checkY = tile.indexY + y;

                //check if neighbouring node is valid
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Space WorldPointToSpace(Vector3 worldPosition) {
        float posX = ((worldPosition.x - transform.position.x) + gridWorldSize.x * 0.5f) / spaceDiameter;
        float posY = ((worldPosition.y - transform.position.y) + gridWorldSize.y * 0.5f) / spaceDiameter;
        posX = Mathf.Clamp(posX, 0, gridSizeX);
        posY = Mathf.Clamp(posY, 0, gridSizeY);

        int x = Mathf.FloorToInt(posX);
        int y = Mathf.FloorToInt(posY);

        return grid[x, y];
    }

    private void OnDrawGizmos() {
        //Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        if (grid != null) {
            foreach (Space t in grid) {
                Gizmos.color = Color.red;
                if (t.isOutside == true) {
                    Gizmos.color = Color.cyan;
                }
                print("worldpos: " + t.worldPosition);
                Gizmos.DrawWireCube(t.worldPosition, new Vector3(spaceDiameter - 0.01f, spaceDiameter - 0.01f, 1));
            }
        }
    }
}
