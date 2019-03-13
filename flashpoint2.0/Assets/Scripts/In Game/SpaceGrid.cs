using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceGrid : MonoBehaviour {

    public Transform firefighter;
    Vector2 gridWorldSize;
    static Space[,] grid;

    [SerializeField] float spaceRadius;
    float spaceDiameter;
    static int gridSizeX, gridSizeY;

    private void Start() {
        gridWorldSize = new Vector2(10, 8);
        spaceDiameter = spaceRadius * 2;
        gridSizeX = 10;
        gridSizeY = 8;
        CreateGrid();
    }

    //updates every frame
    void Update()
    {

    }

    private void CreateGrid() {
        grid = new Space[gridSizeX, gridSizeY];

        Vector2 worldBottomLeft = new Vector2(transform.position.x, transform.position.y) -
            Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;

        //creating outside spaces
        createFirstRowOutsideSpace();
        createLastRowOutsideSpace();
        createFirstColumnOutsideSpace();
        createLastColumnOutsideSpace();

        //create inside spaces
        createFirstRowInsideSpace();
        createSecondRowInsideSpace();
        createThirdRowInsideSpace();
        createFourthRowInsideSpace();
        createFifthRowInsideSpace();
        createSixthRowInsideSpace();

        //corners
        Vector2 worldPoint00 = worldBottomLeft + Vector2.right * (0 * spaceDiameter + spaceRadius)
        + Vector2.up * (0 * spaceDiameter + spaceRadius);
        grid[0, 0] = new Space(worldPoint00, true, 0, 0);

        Vector2 worldPoint90 = worldBottomLeft + Vector2.right * (9 * spaceDiameter + spaceRadius)
        + Vector2.up * (0 * spaceDiameter + spaceRadius);
        grid[9, 0] = new Space(worldPoint90, true, 9, 0);

        Vector2 worldPoint07 = worldBottomLeft + Vector2.right * (0 * spaceDiameter + spaceRadius)
        + Vector2.up * (7 * spaceDiameter + spaceRadius);
        grid[0, 7] = new Space(worldPoint07, true, 0, 7);

        Vector2 worldPoint97 = worldBottomLeft + Vector2.right * (9 * spaceDiameter + spaceRadius)
        + Vector2.up * (7 * spaceDiameter + spaceRadius);
        grid[9, 7] = new Space(worldPoint97, true, 9, 7);




    }

    public static List<Space> GetNeighbours(Space tile) {
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
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        if (grid != null) {
            foreach (Space t in grid) {
                Gizmos.color = Color.red;
                if (t.isOutside == true) {
                    Gizmos.color = Color.cyan;
                }
                Gizmos.DrawWireCube(t.worldPosition, new Vector3(spaceDiameter - 0.01f, spaceDiameter - 0.01f, 1));
            }
        }
    }

    public void createFirstRowOutsideSpace()
    {
        Vector2 worldBottomLeft = new Vector2(transform.position.x, transform.position.y) -
            Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;

        for(int x = 1; x < gridSizeX - 1; x++)
        {
            Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * spaceDiameter + spaceRadius) + Vector2.up * (0 * spaceDiameter + spaceRadius);
            Space space = new Space(worldPoint, true, x, 0);
            if (x == 6)
            {
                space.addDoor(new Door(DoorStatus.Open), 2);
            }
            else
            {
                space.addWall(new Wall(), 2);
            }
            grid[x, 0] = space;
        }
    }

    public void createLastRowOutsideSpace()
    {
        Vector2 worldBottomLeft = new Vector2(transform.position.x, transform.position.y) -
            Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;

        for (int x = 1; x < gridSizeX - 1; x++)
        {
            Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * spaceDiameter + spaceRadius) + Vector2.up * (7 * spaceDiameter + spaceRadius);
            Space space = new Space(worldPoint, true, x, 7);
            if (x == 3)
            {
                space.addDoor(new Door(DoorStatus.Open), 0);
            }
            else
            {
                space.addWall(new Wall(), 0);
            }
            grid[x, 7] = space;
        }
    }

    public void createFirstColumnOutsideSpace()
    {
        Vector2 worldBottomLeft = new Vector2(transform.position.x, transform.position.y) -
            Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;

        for (int y = 1; y < gridSizeY - 1; y++)
        {
            Vector2 worldPoint = worldBottomLeft + Vector2.right * (0 * spaceDiameter + spaceRadius) + Vector2.up * (y * spaceDiameter + spaceRadius);
            Space space = new Space(worldPoint, true, 0, y);
            if (y == 3)
            {
                space.addDoor(new Door(DoorStatus.Open), 1);
            }
            else
            {
                space.addWall(new Wall(), 1);
            }
            grid[0, y] = space;
        }
    }

    public void createLastColumnOutsideSpace()
    {
        Vector2 worldBottomLeft = new Vector2(transform.position.x, transform.position.y) -
            Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;

        for (int y = 1; y < gridSizeY - 1; y++)
        {
            Vector2 worldPoint = worldBottomLeft + Vector2.right * (9 * spaceDiameter + spaceRadius) + Vector2.up * (y * spaceDiameter + spaceRadius);
            Space space = new Space(worldPoint, true, 9, y);
            if(y == 4)
            {
                space.addDoor(new Door(DoorStatus.Open), 3);
            }
            else
            {
                space.addWall(new Wall(), 3);
            }
            grid[9, y] = space;
        }
    }

    public void createFirstRowInsideSpace()
    {
        Vector2 worldBottomLeft = new Vector2(transform.position.x, transform.position.y) -
            Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;

        for (int x = 1; x < gridSizeX - 1; x++)
        {
            Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * spaceDiameter + spaceRadius) + Vector2.up * (1 * spaceDiameter + spaceRadius);
            Space space = new Space(worldPoint, false, x, 1);
            if (x == 1)
            {
                space.addWall(new Wall(), 0);
                space.addWall(new Wall(), 3);
            }
            else if(x == 2)
            {
                space.addWall(new Wall(), 0);
            }
            else if(x == 3)
            {
                space.addWall(new Wall(), 0);
                space.addDoor(new Door(DoorStatus.Closed), 1);
            }
            else if(x == 4)
            {
                space.addWall(new Wall(), 0);
                space.addDoor(new Door(DoorStatus.Closed), 3);
            }
            else if(x == 5)
            {
                space.addWall(new Wall(), 0);
                space.addWall(new Wall(), 1);
            }
            else if(x == 6)
            {
                space.addWall(new Wall(), 3);
                space.addDoor(new Door(DoorStatus.Open), 0);
            }
            else if(x == 7)
            {
                space.addWall(new Wall(), 0);
            }
            else if(x == 8)
            {
                space.addWall(new Wall(), 0);
                space.addWall(new Wall(), 1);
            }
            grid[x, 1] = space;
        }
    }

    public void createSecondRowInsideSpace()
    {
        Vector2 worldBottomLeft = new Vector2(transform.position.x, transform.position.y) -
            Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;

        for (int x = 1; x < gridSizeX - 1; x++)
        {
            Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * spaceDiameter + spaceRadius) + Vector2.up * (2 * spaceDiameter + spaceRadius);
            Space space = new Space(worldPoint, false, x, 2);
            if (x == 1)
            {
                space.addWall(new Wall(), 0);
            }
            else if (x == 3)
            {
                space.addWall(new Wall(), 1);
                space.addWall(new Wall(), 2);
            }
            else if (x == 4)
            {
                space.addWall(new Wall(), 2);
                space.addWall(new Wall(), 3);
            }
            else if (x == 5)
            {
                space.addWall(new Wall(), 2);
                space.addDoor(new Door(DoorStatus.Closed), 1);
            }
            else if (x == 6)
            {
                space.addWall(new Wall(), 2);
                space.addDoor(new Door(DoorStatus.Closed), 3);
            }
            else if (x == 7)
            {
                space.addWall(new Wall(), 2);
            }
            else if (x == 8)
            {
                space.addWall(new Wall(), 1);
                space.addDoor(new Door(DoorStatus.Closed), 2);
            }
            grid[x, 2] = space;
        }
    }

    public void createThirdRowInsideSpace()
    {
        Vector2 worldBottomLeft = new Vector2(transform.position.x, transform.position.y) -
            Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;

        for (int x = 1; x < gridSizeX - 1; x++)
        {
            Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * spaceDiameter + spaceRadius) + Vector2.up * (3 * spaceDiameter + spaceRadius);
            Space space = new Space(worldPoint, false, x, 3);
            if (x == 1)
            {
                space.addDoor(new Door(DoorStatus.Open), 3);
            }
            else if (x == 2)
            {
                space.addDoor(new Door(DoorStatus.Closed), 1);
            }
            else if (x == 3)
            {
                space.addWall(new Wall(), 0);
                space.addDoor(new Door(DoorStatus.Closed), 3);
            }
            else if (x == 4 || x == 5)
            {
                space.addWall(new Wall(), 0);
            }
            else if (x == 6)
            {
                space.addWall(new Wall(), 0);
                space.addWall(new Wall(), 1);
            }
            else if (x == 7)
            {
                space.addWall(new Wall(), 0);
                space.addWall(new Wall(), 3);
            }
            else if (x == 8)
            {
                space.addWall(new Wall(), 1);
                space.addDoor(new Door(DoorStatus.Closed), 0);
            }
            grid[x, 3] = space;
        }
    }

    public void createFourthRowInsideSpace()
    {
        Vector2 worldBottomLeft = new Vector2(transform.position.x, transform.position.y) -
            Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;

        for (int x = 1; x < gridSizeX - 1; x++)
        {
            Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * spaceDiameter + spaceRadius) + Vector2.up * (4 * spaceDiameter + spaceRadius);
            Space space = new Space(worldPoint, false, x, 4);
            if (x == 1)
            {
                space.addWall(new Wall(), 3);
                space.addWall(new Wall(), 2);
            }
            else if (x == 2)
            {
                space.addWall(new Wall(), 1);
                space.addWall(new Wall(), 2);
            }
            else if (x == 3)
            {
                space.addWall(new Wall(), 3);
                space.addWall(new Wall(), 2);
            }
            else if (x == 4)
            {
                space.addDoor(new Door(DoorStatus.Closed), 2);
            }
            else if(x == 5)
            {
                space.addWall(new Wall(), 2);
            }
            else if (x == 6)
            {
                space.addWall(new Wall(), 2);
                space.addDoor(new Door(DoorStatus.Closed), 1);
            }
            else if (x == 7)
            {
                space.addWall(new Wall(), 2);
                space.addDoor(new Door(DoorStatus.Closed), 3);
            }
            else if (x == 8)
            {
                space.addWall(new Wall(), 2);
                space.addDoor(new Door(DoorStatus.Open), 1);
            }
            grid[x, 4] = space;
        }
    }

    public void createFifthRowInsideSpace()
    {
        Vector2 worldBottomLeft = new Vector2(transform.position.x, transform.position.y) -
            Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;

        for (int x = 1; x < gridSizeX - 1; x++)
        {
            Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * spaceDiameter + spaceRadius) + Vector2.up * (5 * spaceDiameter + spaceRadius);
            Space space = new Space(worldPoint, false, x, 5);
            if (x == 1)
            {
                space.addWall(new Wall(), 3);
                space.addWall(new Wall(), 0);
            }
            else if (x == 2 || x == 3)
            {
                space.addWall(new Wall(), 0);
            }
            else if (x == 4)
            {
                space.addDoor(new Door(DoorStatus.Closed), 0);
            }
            else if (x == 5 || x == 7)
            {
                space.addWall(new Wall(), 0);
                space.addWall(new Wall(), 1);
            }
            else if (x == 6)
            {
                space.addWall(new Wall(), 0);
                space.addWall(new Wall(), 3);
            }
            else if (x == 8)
            {
                space.addWall(new Wall(), 0);
                space.addWall(new Wall(), 1);
                space.addWall(new Wall(), 3);

            }
            grid[x, 5] = space;
        }
    }

    public void createSixthRowInsideSpace()
    {
        Vector2 worldBottomLeft = new Vector2(transform.position.x, transform.position.y) -
            Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;

        for (int x = 1; x < gridSizeX - 1; x++)
        {
            Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * spaceDiameter + spaceRadius) + Vector2.up * (6 * spaceDiameter + spaceRadius);
            Space space = new Space(worldPoint, false, x, 6);
            if (x == 1)
            {
                space.addWall(new Wall(), 3);
                space.addWall(new Wall(), 2);
            }
            else if (x == 2 || x == 4)
            {
                space.addWall(new Wall(), 2);
            }
            else if(x == 3)
            {
                space.addDoor(new Door(DoorStatus.Open), 2);
            }
            else if (x == 5 || x == 7)
            {
                space.addWall(new Wall(), 2);
                space.addDoor(new Door(DoorStatus.Closed), 1);
            }
            else if (x == 6)
            {
                space.addWall(new Wall(), 2);
                space.addDoor(new Door(DoorStatus.Closed), 3);
            }
            else if (x == 8)
            {
                space.addWall(new Wall(), 1);
                space.addWall(new Wall(), 2);
                space.addDoor(new Door(DoorStatus.Closed), 3);

            }
            grid[x, 6] = space;
        }
    }


}
