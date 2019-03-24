using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.IO;

public class SpaceGrid : MonoBehaviourPun {

    public Transform firefighter;
    Vector2 gridWorldSize;

    public Space[,] grid;

    [SerializeField] float spaceRadius;
    float spaceDiameter;
    static int gridSizeX, gridSizeY;

    private void Start() {
        gridWorldSize = new Vector2(10, 8);
        spaceDiameter = spaceRadius * 2;
        gridSizeX = 10;
        gridSizeY = 8;
        CreateGrid();
        randomizePOI();
    }

    public Space[,] getGrid() {
        return grid;
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

    //list index: 0 top, 1 right, 2 bottom, 3 left
    public Space[] GetNeighbours(Space space) {
        Space[] neighbours = new Space[4];

        //  _________
        // |__|_x|__|
        // |_x|_c|_x| 
        // |__|_x|__| 
        //search neighbour tiles x of c
        int index = 0;
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == y) continue; //continue if its the middle and corner tile

                int checkX = space.indexX + x;
                int checkY = space.indexY + y;

                //check if neighbouring node is valid
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) { //within boundaries
                    bool isValid = false;

                    if (y == 1 && isValidNeighbour(checkX, checkY, 0)) {
                        isValid = true;
                    }
                    else if (x == 1 && isValidNeighbour(checkX, checkY, 1)) {
                        isValid = true;
                    }
                    else if (y == -1 && isValidNeighbour(checkX, checkY, 2)) {
                        isValid = true;
                    }
                    else if(x == -1 && isValidNeighbour(checkX, checkY, 3)) {
                        isValid = true;
                    }

                    if (isValid) neighbours[index] = grid[checkX, checkY];
                    index++;
                }
            }
        }

        return neighbours;
    }

    private bool isValidNeighbour(int checkX, int checkY, int wallIndex) {
        return grid[checkX, checkY].getWalls()[wallIndex] != default(Wall) &&
                            grid[checkX, checkY].getWalls()[wallIndex].getWallStatus() == WallStatus.Destroyed;
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
                if (t.kind == SpaceKind.Outdoor) {
                    Gizmos.color = Color.cyan;
                }
                Gizmos.DrawWireCube(t.worldPosition, new Vector3(spaceDiameter - 0.01f, spaceDiameter - 0.01f, 1));
            }
        }
    }

    public bool containsFiremarker(int row, int col)
    {
        if(row == 2 && col == 2 || row == 2 && col == 3 || row == 3 && col == 2 || row == 3 && col == 3 || row == 3 && col == 4 ||
        row == 3 && col == 5 || row == 4 && col == 4 || row == 5 && col == 5 || row == 5 && col == 6 || row == 6 && col == 5)
        {
            return true;
        }
        return false;
    }

    public bool alreadyPlaced(int row, int col, int[] rows, int[] cols)
    {
        for(int i = 0; i < cols.Length; i++)
        {
            if(col == cols[i] || row == rows[i])
            {
                return true;
            }
        }
        return false;
    }

    public void randomizePOI()
    {
        string[] paths = new string[3];
        int[] rows = new int[3];
        int[] cols = new int[3];
        int index = 0;

        while(index < 3)
        {
            string path = Path.Combine("PhotonPrefabs", "Prefabs", "POIs", "POI");
            //randomize between 1 and 6
            int col = Random.Range(1, 8);
            //randomize between 1 and 8
            int row = Random.Range(1, 6);

            if (containsFiremarker(row, col))
            {
                continue;
            }

            if (alreadyPlaced(row, col, rows, cols)){
                continue;
            }

            paths[index] = path;
            rows[index] = row;
            cols[index] = col;

            index++;

        }

     



        //THINGS TO PASS TO DATA, repeat 3 times
        /* 1) String path to prefab
         * 2) position. e.g.grid[1,2].worldPosition
         * FORMAT OF DATA: PATH, POSITION,PATH,POSITION,ETC.        
         */
        object[] data = new object[] {paths,rows,cols};

        Photon.Realtime.RaiseEventOptions options = new Photon.Realtime.RaiseEventOptions()
        {
            CachingOption = Photon.Realtime.EventCaching.DoNotCache,
            Receivers = Photon.Realtime.ReceiverGroup.All
        };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceGameUnit, data, options, SendOptions.SendUnreliable);

    }



    //    ================ NETWORK SYNCHRONIZATION SECTION =================
    public void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    public void OnEvent(EventData eventData)
    {
        byte evCode = eventData.Code;

        //0: placing a firefighter
        if (evCode == (byte)PhotonEventCodes.PlaceGameUnit)
        {
            object[] datas = eventData.CustomData as object[];

            string[] paths = (string[]) datas[0];
            int[] rows = (int[]) datas[1];
            int[] cols = (int[]) datas[2];

            for(int i = 0; i < paths.Length; i++)
            {
                Space space = grid[cols[i], rows[i]];

                Vector3 position = space.worldPosition;

                GameObject POI1 = PhotonNetwork.Instantiate(paths[i],
                   position,
                   Quaternion.identity, 0);

                GameUnit poi = POI1.GetComponent<POI>();

                space.addOccupant(poi);

            }

        }

    }
}
