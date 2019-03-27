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

    [SerializeField] 
    float spaceRadius;
    float spaceDiameter;
    static int gridSizeX, gridSizeY;

    private void Start() {
        gridWorldSize = new Vector2(10, 8);
        spaceDiameter = spaceRadius * 2;
        gridSizeX = 10;
        gridSizeY = 8;
        CreateGrid();
        placeFireMarker();
        randomizePOI();
        randomizePOI();
        randomizePOI();
    }

    public Space[,] getGrid() {
        return grid;
    }

    private void CreateGrid() {
        grid = new Space[gridSizeX, gridSizeY];

        Vector2 worldTopLeft = new Vector2(transform.position.x, transform.position.y) -
            Vector2.right * gridWorldSize.x / 2 + Vector2.up * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++) {
            for (int y = 0; y < gridSizeY; y++) {
                Vector2 worldPoint = worldTopLeft + Vector2.right * (x * spaceDiameter + spaceRadius)
                                                              - Vector2.up * (y * spaceDiameter + spaceRadius);
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
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {

                if (x == y || x == -y)
                {
                    continue; //continue if its the middle and corner tile
                }

                int checkX = space.indexX + x;
                int checkY = space.indexY + y;

                //check if neighbouring node is valid
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) { //within boundaries

                    if (y == -1 && isValidNeighbour(checkX, checkY, 0)) {
                        neighbours[0] = grid[checkX, checkY];
                    }
                    else if (x == 1 && isValidNeighbour(checkX, checkY, 1)) {
                        neighbours[1] = grid[checkX, checkY];
                    }
                    else if (y == 1 && isValidNeighbour(checkX, checkY, 2)) {
                        neighbours[2] = grid[checkX, checkY];
                    }
                    else if(x == -1 && isValidNeighbour(checkX, checkY, 3)) {
                        neighbours[3] = grid[checkX, checkY];
                    }
                }
            }
        }

        return neighbours;
    }

    private bool isValidNeighbour(int checkX, int checkY, int wallIndex) {
        return true;
        //return grid[checkX, checkY].getWalls()[wallIndex] != default(Wall) &&
          //                  grid[checkX, checkY].getWalls()[wallIndex].getWallStatus() == WallStatus.Destroyed;
    }

    public Space WorldPointToSpace(Vector3 worldPosition) {
        float posX = ((worldPosition.x - transform.position.x) + gridWorldSize.x * 0.5f) / spaceDiameter;
        float posY = ((transform.position.y - worldPosition.y) + gridWorldSize.y * 0.5f) / spaceDiameter;
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
        if( grid[col,row].getSpaceStatus() == SpaceStatus.Fire)
        {
            return true;
        }
        return false;
    }

    public bool alreadyPlaced(int row, int col)
    {
        List<GameUnit> occupants = grid[col, row].getOccupants();
        foreach(GameUnit gu in occupants)
        {
            if (gu.GetType() == typeof(POI))
            {
                return true;
            }
        }
        return false;
    }

    public void placeFireMarker()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        int[] rows = new int[] { 2, 2, 3, 3, 3, 3, 4, 5, 5, 6 };
        int[] cols = new int[] { 2, 3, 2, 3, 4, 5, 4, 5, 6, 5 };

        object[] data = new object[] { rows, cols };

        Photon.Realtime.RaiseEventOptions options = new Photon.Realtime.RaiseEventOptions()
        {
            CachingOption = Photon.Realtime.EventCaching.DoNotCache,
            Receivers = Photon.Realtime.ReceiverGroup.All
        };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceFireMarker, data, options, SendOptions.SendUnreliable);
    }

    public void randomizePOI()
    {

        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        int col;
        int row;

        do
        {   
            //randomize between 1 and 6
            col = Random.Range(1, 8);
            //randomize between 1 and 8
            row = Random.Range(1, 6);

            if (containsFiremarker(row, col))
            {
                continue;
            }

            if (alreadyPlaced(row, col))
            {
                continue;
            }

        } while (false);

     



        //THINGS TO PASS TO DATA, repeat 3 times
        /* 1) String path to prefab
         * 2) position. e.g.grid[1,2].worldPosition
         * FORMAT OF DATA: PATH, POSITION,PATH,POSITION,ETC.        
         */
        object[] data = new object[] {row,col};

        Photon.Realtime.RaiseEventOptions options = new Photon.Realtime.RaiseEventOptions()
        {
            CachingOption = Photon.Realtime.EventCaching.DoNotCache,
            Receivers = Photon.Realtime.ReceiverGroup.All
        };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlacePOI, data, options, SendOptions.SendUnreliable);

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
        if (evCode == (byte)PhotonEventCodes.PlacePOI)
        {
            object[] datas = eventData.CustomData as object[];

            int row = (int) datas[0];
            int col = (int) datas[1];


            Space space = grid[col, row];

            Vector3 position = space.worldPosition;

            GameObject POI = Instantiate(Resources.Load("PhotonPrefabs/Prefabs/POIs/POI")) as GameObject;
            Vector3 newPosition = new Vector3(position.x, position.y, -5);

            POI.GetComponent<Transform>().position = newPosition;

            POI poi = POI.GetComponent<POI>();

            space.addOccupant(poi);
        }
        else if (evCode == (byte)PhotonEventCodes.PlaceFireMarker)
        {
            object[] datas = eventData.CustomData as object[];

            int[] rows = (int[])datas[0];
            int[] cols = (int[])datas[1];

            for(int i = 0; i < rows.Length; i++)
            {
                Vector3 position = grid[cols[i], rows[i]].worldPosition;
                GameObject newFireMarker = Instantiate(Resources.Load("PhotonPrefabs/Prefabs/FireMarker")) as GameObject;
                Vector3 newPosition = new Vector3(position.x, position.y, -5);

                newFireMarker.GetComponent<Transform>().position = newPosition;

                grid[cols[i], rows[i]].setSpaceStatus(SpaceStatus.Fire);
            }
        }

    }
}
