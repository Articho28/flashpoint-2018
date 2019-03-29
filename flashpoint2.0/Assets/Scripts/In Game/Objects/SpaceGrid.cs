using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.IO;

public class SpaceGrid : MonoBehaviourPun {

    Photon.Realtime.RaiseEventOptions options = new Photon.Realtime.RaiseEventOptions()
    {
        CachingOption = Photon.Realtime.EventCaching.DoNotCache,
        Receivers = Photon.Realtime.ReceiverGroup.All
    };

    public Transform firefighter;
    Vector2 gridWorldSize;

    public Space[,] grid;

    [SerializeField] 
    float spaceRadius;
    float spaceDiameter;
    static int gridSizeX, gridSizeY;
    static int numOfActivePOI;

    private void Start() {
        gridWorldSize = new Vector2(10, 8);
        spaceDiameter = spaceRadius * 2;
        gridSizeX = 10;
        gridSizeY = 8;
        CreateGrid();

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.BoardSetup, null, options, SendOptions.SendUnreliable);
        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceInitialFireMarker, null, options, SendOptions.SendReliable);
    }

    void Awake()
    {
        numOfActivePOI = 0;
    }

    void Update()
    {
        if(numOfActivePOI < 3)
        {
            PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlacePOI, null, options, SendOptions.SendUnreliable);
        }
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




    private void BoardSetup()     {         //INSTANTIATE WALLS         //north:0, east:1, south:2, west:3          Space currSpace;         GameObject w1 = GameObject.Find("/Board/Walls/1");
        w1.GetComponent<Wall>().setPhysicalObject(w1);         GameObject w2 = GameObject.Find("/Board/Walls/2");
        w2.GetComponent<Wall>().setPhysicalObject(w2);         GameObject w3 = GameObject.Find("/Board/Walls/3");
        w3.GetComponent<Wall>().setPhysicalObject(w3);         GameObject w4 = GameObject.Find("/Board/Walls/4");
        w4.GetComponent<Wall>().setPhysicalObject(w4);         GameObject w5 = GameObject.Find("/Board/Walls/5");
        w5.GetComponent<Wall>().setPhysicalObject(w5);         GameObject w6 = GameObject.Find("/Board/Walls/6");
        w6.GetComponent<Wall>().setPhysicalObject(w6);         GameObject w7 = GameObject.Find("/Board/Walls/7");
        w7.GetComponent<Wall>().setPhysicalObject(w7);         GameObject w8 = GameObject.Find("/Board/Walls/8");
        w8.GetComponent<Wall>().setPhysicalObject(w8);         GameObject w9 = GameObject.Find("/Board/Walls/9");
        w9.GetComponent<Wall>().setPhysicalObject(w9);         GameObject w10 = GameObject.Find("/Board/Walls/10");
        w10.GetComponent<Wall>().setPhysicalObject(w10);         GameObject w11 = GameObject.Find("/Board/Walls/11");
        w11.GetComponent<Wall>().setPhysicalObject(w11);         GameObject w12 = GameObject.Find("/Board/Walls/12");
        w12.GetComponent<Wall>().setPhysicalObject(w12);         GameObject w13 = GameObject.Find("/Board/Walls/13");
        w13.GetComponent<Wall>().setPhysicalObject(w13);         GameObject w14 = GameObject.Find("/Board/Walls/14");
        w14.GetComponent<Wall>().setPhysicalObject(w14);         GameObject w15 = GameObject.Find("/Board/Walls/15");
        w15.GetComponent<Wall>().setPhysicalObject(w15);         GameObject w16 = GameObject.Find("/Board/Walls/16");
        w16.GetComponent<Wall>().setPhysicalObject(w16);         GameObject w17 = GameObject.Find("/Board/Walls/17");
        w17.GetComponent<Wall>().setPhysicalObject(w17);         GameObject w18 = GameObject.Find("/Board/Walls/18");
        w18.GetComponent<Wall>().setPhysicalObject(w18);         GameObject w19 = GameObject.Find("/Board/Walls/19");
        w19.GetComponent<Wall>().setPhysicalObject(w19);         GameObject w20 = GameObject.Find("/Board/Walls/20");
        w20.GetComponent<Wall>().setPhysicalObject(w20);         GameObject w21 = GameObject.Find("/Board/Walls/21");
        w21.GetComponent<Wall>().setPhysicalObject(w21);         GameObject w22 = GameObject.Find("/Board/Walls/22");
        w22.GetComponent<Wall>().setPhysicalObject(w22);         GameObject w23 = GameObject.Find("/Board/Walls/23");
        w23.GetComponent<Wall>().setPhysicalObject(w23);         GameObject w24 = GameObject.Find("/Board/Walls/24");
        w24.GetComponent<Wall>().setPhysicalObject(w24);         GameObject w25 = GameObject.Find("/Board/Walls/25");
        w25.GetComponent<Wall>().setPhysicalObject(w25);         GameObject w26 = GameObject.Find("/Board/Walls/26");
        w26.GetComponent<Wall>().setPhysicalObject(w26);         GameObject w27 = GameObject.Find("/Board/Walls/27");
        w27.GetComponent<Wall>().setPhysicalObject(w27);         GameObject w28 = GameObject.Find("/Board/Walls/28");
        w28.GetComponent<Wall>().setPhysicalObject(w28);         GameObject w29 = GameObject.Find("/Board/Walls/29");
        w29.GetComponent<Wall>().setPhysicalObject(w29);         GameObject w30 = GameObject.Find("/Board/Walls/30");
        w30.GetComponent<Wall>().setPhysicalObject(w30);         GameObject w31 = GameObject.Find("/Board/Walls/31");
        w31.GetComponent<Wall>().setPhysicalObject(w31);         GameObject w32 = GameObject.Find("/Board/Walls/32");
        w32.GetComponent<Wall>().setPhysicalObject(w32);         GameObject w33 = GameObject.Find("/Board/Walls/33");
        w33.GetComponent<Wall>().setPhysicalObject(w33);         GameObject w34 = GameObject.Find("/Board/Walls/34");
        w34.GetComponent<Wall>().setPhysicalObject(w34);         GameObject w35 = GameObject.Find("/Board/Walls/35");
        w35.GetComponent<Wall>().setPhysicalObject(w35);         GameObject w36 = GameObject.Find("/Board/Walls/36");
        w36.GetComponent<Wall>().setPhysicalObject(w36);         GameObject w37 = GameObject.Find("/Board/Walls/37");
        w37.GetComponent<Wall>().setPhysicalObject(w37);         GameObject w38 = GameObject.Find("/Board/Walls/38");
        w38.GetComponent<Wall>().setPhysicalObject(w38);         GameObject w39 = GameObject.Find("/Board/Walls/39");
        w39.GetComponent<Wall>().setPhysicalObject(w39);         GameObject w40 = GameObject.Find("/Board/Walls/40");
        w40.GetComponent<Wall>().setPhysicalObject(w40);         GameObject w41 = GameObject.Find("/Board/Walls/41");
        w41.GetComponent<Wall>().setPhysicalObject(w41);         GameObject w42 = GameObject.Find("/Board/Walls/42");
        w42.GetComponent<Wall>().setPhysicalObject(w42);         GameObject w43 = GameObject.Find("/Board/Walls/43");
        w43.GetComponent<Wall>().setPhysicalObject(w43);         GameObject w44 = GameObject.Find("/Board/Walls/44");
        w44.GetComponent<Wall>().setPhysicalObject(w44);         GameObject w45 = GameObject.Find("/Board/Walls/45");
        w45.GetComponent<Wall>().setPhysicalObject(w45);         GameObject w46 = GameObject.Find("/Board/Walls/46");
        w46.GetComponent<Wall>().setPhysicalObject(w46);         GameObject w47 = GameObject.Find("/Board/Walls/47");
        w47.GetComponent<Wall>().setPhysicalObject(w47);         GameObject w48 = GameObject.Find("/Board/Walls/48");
        w48.GetComponent<Wall>().setPhysicalObject(w48);         GameObject w49 = GameObject.Find("/Board/Walls/49");
        w49.GetComponent<Wall>().setPhysicalObject(w49);         GameObject w50 = GameObject.Find("/Board/Walls/50");
        w50.GetComponent<Wall>().setPhysicalObject(w50);         GameObject w51 = GameObject.Find("/Board/Walls/51");
        w51.GetComponent<Wall>().setPhysicalObject(w51);         GameObject w52 = GameObject.Find("/Board/Walls/52");
        w52.GetComponent<Wall>().setPhysicalObject(w52);         GameObject w53 = GameObject.Find("/Board/Walls/53");
        w53.GetComponent<Wall>().setPhysicalObject(w53);         GameObject w54 = GameObject.Find("/Board/Walls/54");
        w54.GetComponent<Wall>().setPhysicalObject(w54);          GameObject d1 = GameObject.Find("/Board/Doors/doorCol45");
        d1.GetComponent<Door>().setPhysicalObject(d1);
        d1.GetComponent<Door>().setDoorStatus(DoorStatus.Closed);         GameObject d2 = GameObject.Find("/Board/Doors/Door top");
        d2.GetComponent<Door>().setPhysicalObject(d2);
        d2.GetComponent<Door>().setDoorStatus(DoorStatus.Open);         GameObject d3 = GameObject.Find("/Board/Doors/Door bottom");
        d3.GetComponent<Door>().setPhysicalObject(d3);
        d3.GetComponent<Door>().setDoorStatus(DoorStatus.Open);         GameObject d4 = GameObject.Find("/Board/Doors/Door right");
        d4.GetComponent<Door>().setPhysicalObject(d4);
        d4.GetComponent<Door>().setDoorStatus(DoorStatus.Open);         GameObject d5 = GameObject.Find("/Board/Doors/Door left");
        d5.GetComponent<Door>().setPhysicalObject(d5);
        d5.GetComponent<Door>().setDoorStatus(DoorStatus.Open);         GameObject d6 = GameObject.Find("/Board/Doors/doorCol78");
        d6.GetComponent<Door>().setPhysicalObject(d6);
        d6.GetComponent<Door>().setDoorStatus(DoorStatus.Closed);         GameObject d7 = GameObject.Find("/Board/Doors/doorCol56");
        d7.GetComponent<Door>().setPhysicalObject(d7);
        d7.GetComponent<Door>().setDoorStatus(DoorStatus.Closed);         GameObject d8 = GameObject.Find("/Board/Doors/doorCol67");
        d8.GetComponent<Door>().setPhysicalObject(d8);
        d8.GetComponent<Door>().setDoorStatus(DoorStatus.Closed);         GameObject d9 = GameObject.Find("/Board/Doors/doorRow56");
        d9.GetComponent<Door>().setPhysicalObject(d9);
        d9.GetComponent<Door>().setDoorStatus(DoorStatus.Closed);         GameObject d10 = GameObject.Find("/Board/Doors/doorCol89");
        d10.GetComponent<Door>().setPhysicalObject(d10);
        d10.GetComponent<Door>().setDoorStatus(DoorStatus.Closed);         GameObject d11 = GameObject.Find("/Board/Doors/doorRow34");
        d11.GetComponent<Door>().setPhysicalObject(d11);
        d11.GetComponent<Door>().setDoorStatus(DoorStatus.Closed);         GameObject d12 = GameObject.Find("/Board/Doors/doorCol34");
        d12.GetComponent<Door>().setPhysicalObject(d12);
        d12.GetComponent<Door>().setDoorStatus(DoorStatus.Closed);

          for (int j = 0; j < gridSizeY; j++) //rows
        {             for (int i = 0; i < gridSizeX; i++) //columns
            {                 currSpace = grid[i, j];                  //DO OUTDOORS SPACE FIRST                  if (j == 0) //first row                 {                     currSpace.setSpaceKind(SpaceKind.Outdoor);                     currSpace.setSpaceStatus(SpaceStatus.Safe);                     switch (i)                     {                         case 0:                             currSpace.setWalls(new Wall[4] { null, null, null, null });                             currSpace.setDoors(new Door[4] { null, null, null, null });                             break;                         case 1:                             currSpace.setWalls(new Wall[4] { null, null, w1.GetComponent<Wall>(), null });                             currSpace.setDoors(new Door[4] { null, null, null, null });                             break;                         case 2:                             currSpace.setWalls(new Wall[] { null, null, w2.GetComponent<Wall>(), null });                             currSpace.setDoors(new Door[4] { null, null, null, null });                             break;                         case 3:                             currSpace.setWalls(new Wall[4] { null, null, w3.GetComponent<Wall>(), null });                             currSpace.setDoors(new Door[4] { null, null, null, null });                             break;                         case 4:                             currSpace.setWalls(new Wall[4] { null, null, w4.GetComponent<Wall>(), null });                             currSpace.setDoors(new Door[4] { null, null, null, null });                             break;                         case 5:                             currSpace.setWalls(new Wall[4] { null, null, w5.GetComponent<Wall>(), null });                             currSpace.setDoors(new Door[4] { null, null, null, null });                             break;                         case 6:                             currSpace.setWalls(new Wall[4] { null, null, w6.GetComponent<Wall>(), null });                             currSpace.setDoors(new Door[4] { null, null, d2.GetComponent<Door>(), null });                             break;                         case 7:                             currSpace.setWalls(new Wall[4] { null, null, w7.GetComponent<Wall>(), null });                             currSpace.setDoors(new Door[4] { null, null, null, null });                             break;                         case 8:                             currSpace.setWalls(new Wall[4] { null, null, w8.GetComponent<Wall>(), null });                             currSpace.setDoors(new Door[4] { null, null, null, null });                             break;                         case 9:                             currSpace.setWalls(new Wall[4] { null, null, null, null });                             currSpace.setDoors(new Door[4] { null, null, null, null });                             break;                     }                 }                  else if (j == 1)                 {                      if (i == 0)                     {                         currSpace.setSpaceKind(SpaceKind.Outdoor);                         currSpace.setSpaceStatus(SpaceStatus.Safe);                         currSpace.setWalls(new Wall[4] { null, w40.GetComponent<Wall>(), null, null });                         currSpace.setDoors(new Door[4] { null, null, null, null });                     }                     else if (i == 9)                     {                         currSpace.setSpaceKind(SpaceKind.Outdoor);                         currSpace.setSpaceStatus(SpaceStatus.Safe);                         currSpace.setWalls(new Wall[4] { null, null, null, w21.GetComponent<Wall>() });                         currSpace.setDoors(new Door[4] { null, null, null, null });                     }                     else                     {                         currSpace.setSpaceKind(SpaceKind.Indoor);                         currSpace.setSpaceStatus(SpaceStatus.Safe);                         switch (i)                         {                             case 1:                                 currSpace.setWalls(new Wall[4] { w1.GetComponent<Wall>(), null, null, w40.GetComponent<Wall>() });                                 currSpace.setDoors(new Door[4] { null, null, null, null });                                 break;                             case 2:                                 currSpace.setWalls(new Wall[] { w2.GetComponent<Wall>(), null, null, null });                                 currSpace.setDoors(new Door[4] { null, null, null, null });                                 break;                             case 3:                                 currSpace.setWalls(new Wall[4] { w3.GetComponent<Wall>(), w43.GetComponent<Wall>(), null, null });                                 currSpace.setDoors(new Door[4] { null, d1.GetComponent<Door>(), null, null });                                 break;                             case 4:                                 currSpace.setWalls(new Wall[4] { w4.GetComponent<Wall>(), null, null, w43.GetComponent<Wall>() });                                 currSpace.setDoors(new Door[4] { null, null, null, d1.GetComponent<Door>() });                                 break;                             case 5:                                 currSpace.setWalls(new Wall[4] { w5.GetComponent<Wall>(), w41.GetComponent<Wall>(), null, null });                                 currSpace.setDoors(new Door[4] { null, null, null, null });                                 break;                             case 6:                                 currSpace.setWalls(new Wall[4] { w6.GetComponent<Wall>(), null, null, w41.GetComponent<Wall>() });                                 currSpace.setDoors(new Door[4] { d2.GetComponent<Door>(), null, null, null });                                 break;                             case 7:                                 currSpace.setWalls(new Wall[4] { w7.GetComponent<Wall>(), null, null, null });                                 currSpace.setDoors(new Door[4] { null, null, null, null });                                 break;                             case 8:                                 currSpace.setWalls(new Wall[4] { w8.GetComponent<Wall>(), w21.GetComponent<Wall>(), null, null });                                 currSpace.setDoors(new Door[4] { null, null, null, null });                                 break;                          }                     }                 }                  else if (j == 2)                 {                      if (i == 0)                     {                         currSpace.setSpaceKind(SpaceKind.Outdoor);                         currSpace.setSpaceStatus(SpaceStatus.Safe);                         currSpace.setWalls(new Wall[4] { null, w39.GetComponent<Wall>(), null, null });                         currSpace.setDoors(new Door[4] { null, null, null, null });                     }                     else if (i == 9)                     {                         currSpace.setSpaceKind(SpaceKind.Outdoor);                         currSpace.setSpaceStatus(SpaceStatus.Safe);                         currSpace.setWalls(new Wall[4] { null, null, null, w22.GetComponent<Wall>() });                         currSpace.setDoors(new Door[4] { null, null, null, null });                     }                     else                     {                         currSpace.setSpaceKind(SpaceKind.Indoor);                         currSpace.setSpaceStatus(SpaceStatus.Safe);                         switch (i)                         {                             case 1:                                 currSpace.setWalls(new Wall[4] { null, null, null, w39.GetComponent<Wall>() });                                 currSpace.setDoors(new Door[4] { null, null, null, null });                                 break;                             case 2:                                 currSpace.setWalls(new Wall[4] { null, null, null, null });                                 currSpace.setDoors(new Door[4] { null, null, null, null });                                 break;                             case 3:                                 currSpace.setWalls(new Wall[4] { null, w44.GetComponent<Wall>(), w9.GetComponent<Wall>(), null });                                 currSpace.setDoors(new Door[4] { null, null, null, null });                                 break;                             case 4:                                 currSpace.setWalls(new Wall[4] { null, null, w10.GetComponent<Wall>(), w44.GetComponent<Wall>() });                                 currSpace.setDoors(new Door[4] { null, null, null, null });                                 break;                             case 5:                                 currSpace.setWalls(new Wall[4] { null, w42.GetComponent<Wall>(), w11.GetComponent<Wall>(), null });                                 currSpace.setDoors(new Door[4] { null, d8.GetComponent<Door>(), null, null });                                 break;                             case 6:                                 currSpace.setWalls(new Wall[4] { null, null, w12.GetComponent<Wall>(), w42.GetComponent<Wall>() });                                 currSpace.setDoors(new Door[4] { null, null, null, d8.GetComponent<Door>() });                                 break;                             case 7:                                 currSpace.setWalls(new Wall[4] { null, null, w13.GetComponent<Wall>(), null });                                 currSpace.setDoors(new Door[4] { null, null, null, null });                                 break;                             case 8:                                 currSpace.setWalls(new Wall[4] { null, w22.GetComponent<Wall>(), w14.GetComponent<Wall>(), null });                                 currSpace.setDoors(new Door[4] { null, null, d11.GetComponent<Door>(), null });                                 break;                          }                     }                 }                  else if (j == 3)                 {                      if (i == 0)                     {                         currSpace.setSpaceKind(SpaceKind.Outdoor);                         currSpace.setSpaceStatus(SpaceStatus.Safe);                         currSpace.setWalls(new Wall[4] { null, w38.GetComponent<Wall>(), null, null });                         currSpace.setDoors(new Door[4] { null, d5.GetComponent<Door>(), null, null });                     }                     else if (i == 9)                     {                         currSpace.setSpaceKind(SpaceKind.Outdoor);                         currSpace.setSpaceStatus(SpaceStatus.Safe);                         currSpace.setWalls(new Wall[4] { null, null, null, w23.GetComponent<Wall>() });                         currSpace.setDoors(new Door[4] { null, null, null, null });                     }                     else                     {                         currSpace.setSpaceKind(SpaceKind.Indoor);                         currSpace.setSpaceStatus(SpaceStatus.Safe);                         switch (i)                         {                             case 1:                                 currSpace.setWalls(new Wall[4] { null, null, null, w38.GetComponent<Wall>() });                                 currSpace.setDoors(new Door[4] { null, null, null, d5.GetComponent<Door>() });                                 break;                             case 2:                                 currSpace.setWalls(new Wall[4] { null, w45.GetComponent<Wall>(), null, null });                                 currSpace.setDoors(new Door[4] { null, d12.GetComponent<Door>(), null, null });                                 break;                             case 3:                                 currSpace.setWalls(new Wall[4] { w9.GetComponent<Wall>(), null, null, w45.GetComponent<Wall>() });                                 currSpace.setDoors(new Door[4] { null, null, null, d12.GetComponent<Door>() });                                 break;                             case 4:                                 currSpace.setWalls(new Wall[4] { w10.GetComponent<Wall>(), null, null, null });                                 currSpace.setDoors(new Door[4] { null, null, null, null });                                 break;                             case 5:                                 currSpace.setWalls(new Wall[4] { w11.GetComponent<Wall>(), null, null, null });                                 currSpace.setDoors(new Door[4] { null, null, null, null });                                 break;                             case 6:                                 currSpace.setWalls(new Wall[4] { w12.GetComponent<Wall>(), w53.GetComponent<Wall>(), null, null });                                 currSpace.setDoors(new Door[4] { null, null, null, null });                                 break;                             case 7:                                 currSpace.setWalls(new Wall[4] { w13.GetComponent<Wall>(), null, null, w53.GetComponent<Wall>() });                                 currSpace.setDoors(new Door[4] { null, null, null, null });                                 break;                             case 8:                                 currSpace.setWalls(new Wall[4] { w14.GetComponent<Wall>(), w23.GetComponent<Wall>(), null, null });                                 currSpace.setDoors(new Door[4] { d11.GetComponent<Door>(), null, null, null });                                 break;                          }                     }                 }                  else if (j == 4)                 {                      if (i == 0)                     {                         currSpace.setSpaceKind(SpaceKind.Outdoor);                         currSpace.setSpaceStatus(SpaceStatus.Safe);                         currSpace.setWalls(new Wall[4] { null, w37.GetComponent<Wall>(), null, null });                         currSpace.setDoors(new Door[4] { null, null, null, null });                     }                     else if (i == 9)                     {                         currSpace.setSpaceKind(SpaceKind.Outdoor);                         currSpace.setSpaceStatus(SpaceStatus.Safe);                         currSpace.setWalls(new Wall[4] { null, null, null, w24.GetComponent<Wall>() });                         currSpace.setDoors(new Door[4] { null, null, null, d4.GetComponent<Door>() });                     }                     else                     {                         currSpace.setSpaceKind(SpaceKind.Indoor);                         currSpace.setSpaceStatus(SpaceStatus.Safe);                         switch (i)                         {                             case 1:                                 currSpace.setWalls(new Wall[4] { null, null, w50.GetComponent<Wall>(), w37.GetComponent<Wall>() });                                 currSpace.setDoors(new Door[4] { null, null, null, null });                                 break;                             case 2:                                 currSpace.setWalls(new Wall[4] { null, w46.GetComponent<Wall>(), w49.GetComponent<Wall>(), null });                                 currSpace.setDoors(new Door[4] { null, null, null, null });                                 break;                             case 3:                                 currSpace.setWalls(new Wall[4] { null, null, w15.GetComponent<Wall>(), w46.GetComponent<Wall>() });                                 currSpace.setDoors(new Door[4] { null, null, null, null });                                 break;                             case 4:                                 currSpace.setWalls(new Wall[4] { null, null, w16.GetComponent<Wall>(), null });                                 currSpace.setDoors(new Door[4] { null, null, d9.GetComponent<Door>(), null });                                 break;                             case 5:                                 currSpace.setWalls(new Wall[4] { null, null, w17.GetComponent<Wall>(), null });                                 currSpace.setDoors(new Door[4] { null, null, null, null });                                 break;                             case 6:                                 currSpace.setWalls(new Wall[4] { null, w54.GetComponent<Wall>(), w18.GetComponent<Wall>(), null });                                 currSpace.setDoors(new Door[4] { null, d6.GetComponent<Door>(), null, null });                                 break;                             case 7:                                 currSpace.setWalls(new Wall[4] { null, null, w19.GetComponent<Wall>(), w54.GetComponent<Wall>() });                                 currSpace.setDoors(new Door[4] { null, null, null, d6.GetComponent<Door>() });                                 break;                             case 8:                                 currSpace.setWalls(new Wall[4] { null, w24.GetComponent<Wall>(), w20.GetComponent<Wall>(), null });                                 currSpace.setDoors(new Door[4] { null, d4.GetComponent<Door>(), null, null });                                 break;                          }                     }                 }                  else if (j == 5)                 {                      if (i == 0)                     {                         currSpace.setSpaceKind(SpaceKind.Outdoor);                         currSpace.setSpaceStatus(SpaceStatus.Safe);                         currSpace.setWalls(new Wall[4] { null, w36.GetComponent<Wall>(), null, null });                         currSpace.setDoors(new Door[4] { null, null, null, null });                     }                     else if (i == 9)                     {                         currSpace.setSpaceKind(SpaceKind.Outdoor);                         currSpace.setSpaceStatus(SpaceStatus.Safe);                         currSpace.setWalls(new Wall[4] { null, null, null, w25.GetComponent<Wall>() });                         currSpace.setDoors(new Door[4] { null, null, null, null });                     }                     else                     {                         currSpace.setSpaceKind(SpaceKind.Indoor);                         currSpace.setSpaceStatus(SpaceStatus.Safe);                         switch (i)                         {                             case 1:                                 currSpace.setWalls(new Wall[4] { w50.GetComponent<Wall>(), null, null, w36.GetComponent<Wall>() });                                 currSpace.setDoors(new Door[4] { null, null, null, null });                                 break;                             case 2:                                 currSpace.setWalls(new Wall[4] { w49.GetComponent<Wall>(), null, null, null });                                 currSpace.setDoors(new Door[4] { null, null, null, null });                                 break;                             case 3:                                 currSpace.setWalls(new Wall[4] { w15.GetComponent<Wall>(), null, null, null });                                 currSpace.setDoors(new Door[4] { null, null, null, null });                                 break;                             case 4:                                 currSpace.setWalls(new Wall[4] { w16.GetComponent<Wall>(), w47.GetComponent<Wall>(), null, null });                                 currSpace.setDoors(new Door[4] { d9.GetComponent<Door>(), null, null, null });                                 break;                             case 5:                                 currSpace.setWalls(new Wall[4] { w17.GetComponent<Wall>(), null, null, w47.GetComponent<Wall>() });                                 currSpace.setDoors(new Door[4] { null, null, null, null });                                 break;                             case 6:                                 currSpace.setWalls(new Wall[4] { w18.GetComponent<Wall>(), null, null, null });                                 currSpace.setDoors(new Door[4] { null, null, null, null });                                 break;                             case 7:                                 currSpace.setWalls(new Wall[4] { w19.GetComponent<Wall>(), w51.GetComponent<Wall>(), null, null });                                 currSpace.setDoors(new Door[4] { null, null, null, null });                                 break;                             case 8:                                 currSpace.setWalls(new Wall[4] { w20.GetComponent<Wall>(), w25.GetComponent<Wall>(), null, w51.GetComponent<Wall>() });                                 currSpace.setDoors(new Door[4] { null, null, null, null });                                 break;                          }                     }                 }                  else if (j == 6)                 {                      if (i == 0)                     {                         currSpace.setSpaceKind(SpaceKind.Outdoor);                         currSpace.setSpaceStatus(SpaceStatus.Safe);                         currSpace.setWalls(new Wall[4] { null, w35.GetComponent<Wall>(), null, null });                         currSpace.setDoors(new Door[4] { null, null, null, null });                     }                     else if (i == 9)                     {                         currSpace.setSpaceKind(SpaceKind.Outdoor);                         currSpace.setSpaceStatus(SpaceStatus.Safe);                         currSpace.setWalls(new Wall[4] { null, null, null, w26.GetComponent<Wall>() });                         currSpace.setDoors(new Door[4] { null, null, null, null });                     }                     else                     {                         currSpace.setSpaceKind(SpaceKind.Indoor);                         currSpace.setSpaceStatus(SpaceStatus.Safe);                         switch (i)                         {                             case 1:                                 currSpace.setWalls(new Wall[4] { null, null, w34.GetComponent<Wall>(), w35.GetComponent<Wall>() });                                 currSpace.setDoors(new Door[4] { null, null, null, null });                                 break;                             case 2:                                 currSpace.setWalls(new Wall[4] { null, null, w33.GetComponent<Wall>(), null });                                 currSpace.setDoors(new Door[4] { null, null, null, null });                                 break;                             case 3:                                 currSpace.setWalls(new Wall[4] { null, null, w32.GetComponent<Wall>(), null });                                 currSpace.setDoors(new Door[4] { null, null, d3.GetComponent<Door>(), null });                                 break;                             case 4:                                 currSpace.setWalls(new Wall[4] { null, w48.GetComponent<Wall>(), w31.GetComponent<Wall>(), null });                                 currSpace.setDoors(new Door[4] { null, d7.GetComponent<Door>(), null, null });                                 break;                             case 5:                                 currSpace.setWalls(new Wall[4] { null, null, w30.GetComponent<Wall>(), w48.GetComponent<Wall>() });                                 currSpace.setDoors(new Door[4] { null, null, null, d7.GetComponent<Door>() });                                 break;                             case 6:                                 currSpace.setWalls(new Wall[4] { null, null, w29.GetComponent<Wall>(), null });                                 currSpace.setDoors(new Door[4] { null, null, null, null });                                 break;                             case 7:                                 currSpace.setWalls(new Wall[4] { null, w52.GetComponent<Wall>(), w28.GetComponent<Wall>(), null });                                 currSpace.setDoors(new Door[4] { null, d10.GetComponent<Door>(), null, null });                                 break;                             case 8:                                 currSpace.setWalls(new Wall[4] { null, w26.GetComponent<Wall>(), w27.GetComponent<Wall>(), w52.GetComponent<Wall>() });                                 currSpace.setDoors(new Door[4] { null, null, null, d10.GetComponent<Door>() });                                 break;                          }                     }                 }                  else if (j == 7)                 {                     currSpace.setSpaceKind(SpaceKind.Outdoor);                     currSpace.setSpaceStatus(SpaceStatus.Safe);                     switch (i)                     {                         case 0:                             currSpace.setWalls(new Wall[4] { null, null, null, null });                             currSpace.setDoors(new Door[4] { null, null, null, null });                             break;                         case 1:                             currSpace.setWalls(new Wall[4] { w34.GetComponent<Wall>(), null, null, null });                             currSpace.setDoors(new Door[4] { null, null, null, null });                             break;                         case 2:                             currSpace.setWalls(new Wall[4] { w33.GetComponent<Wall>(), null, null, null });                             currSpace.setDoors(new Door[4] { null, null, null, null });                             break;                         case 3:                             currSpace.setWalls(new Wall[4] { w32.GetComponent<Wall>(), null, null, null });                             currSpace.setDoors(new Door[4] { d3.GetComponent<Door>(), null, null, null });                             break;                         case 4:                             currSpace.setWalls(new Wall[4] { w31.GetComponent<Wall>(), null, null, null });                             currSpace.setDoors(new Door[4] { null, null, null, null });                             break;                         case 5:                             currSpace.setWalls(new Wall[4] { w30.GetComponent<Wall>(), null, null, null });                             currSpace.setDoors(new Door[4] { null, null, null, null });                             break;                         case 6:                             currSpace.setWalls(new Wall[4] { w29.GetComponent<Wall>(), null, null, null });                             currSpace.setDoors(new Door[4] { null, null, null, null });                             break;                         case 7:                             currSpace.setWalls(new Wall[4] { w28.GetComponent<Wall>(), null, null, null });                             currSpace.setDoors(new Door[4] { null, null, null, null });                             break;                         case 8:                             currSpace.setWalls(new Wall[4] { w27.GetComponent<Wall>(), null, null, null });                             currSpace.setDoors(new Door[4] { null, null, null, null });                             break;                         case 9:                             currSpace.setWalls(new Wall[4] { null, null, null, null });                             currSpace.setDoors(new Door[4] { null, null, null, null });                             break;                     }                 }             }         }     } 






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

        //return grid[checkX, checkY].getWalls()[wallIndex] != default(Wall) &&
        //                  grid[checkX, checkY].getWalls()[wallIndex].getWallStatus() == WallStatus.Destroyed





        return true;


        //bool openSpace = (grid[checkX, checkY].getWalls()[wallIndex] == null);
        //if (openSpace) {
        //    return true;
        //}
        //else //if there's a wall
        //{
        //    bool openDoor = (grid[checkX, checkY].getDoors()[wallIndex] == null);
        //    bool destroyedWall = (grid[checkX, checkY].getWalls()[wallIndex].getWallStatus() == WallStatus.Destroyed);
        //    if(openDoor || destroyedWall)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }

        //}

    }

    //TODO Don't use
    public Space getNeighborInDirection(Space currentLocation, int direction)
    {
        int currentX = currentLocation.indexX;
        int currentY = currentLocation.indexY;

        if (direction == 0)
        {
            if (grid[currentX, currentY - 1] != null)
            {
                return grid[currentX, currentY - 1];
            }
           
        }
        else if (direction == 1)
        {
            if (grid[currentX + 1, currentY] != null)
            {
                return grid[currentX + 1, currentY];
            }
        }
        else if (direction == 2)
        {
            if (grid[currentX, currentY + 1] != null)
            {
                return grid[currentX, currentY + 1];
            }
        }
        else
        {
            if(grid[currentX-1, currentY] != null)
            {
                return grid[currentX - 1, currentY];
            }
        }

        return null;
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

    public bool containsFireORSmoke(int col, int row)
    {
        if( grid[row,col].getSpaceStatus() == SpaceStatus.Fire || grid[row,col].getSpaceStatus() == SpaceStatus.Safe)
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

        int[] rows = new int[] { 2, 2, 3, 3, 3, 3, 4, 5, 5, 6 };
        int[] cols = new int[] { 2, 3, 2, 3, 4, 5, 4, 5, 6, 5 };

        for (int i = 0; i < rows.Length; i++)
        {
            Space currentSpace = grid[cols[i], rows[i]];
            Vector3 position = currentSpace.worldPosition;
            GameObject newFireMarker = Instantiate(Resources.Load("PhotonPrefabs/Prefabs/FireMarker/FireMarker")) as GameObject;
            Vector3 newPosition = new Vector3(position.x, position.y, -5);

            newFireMarker.GetComponent<Transform>().position = newPosition;
            newFireMarker.GetComponent<GameUnit>().setCurrentSpace(currentSpace);
            newFireMarker.GetComponent<GameUnit>().setType("FireMarker");
            newFireMarker.GetComponent<GameUnit>().setPhysicalObject(newFireMarker);
            currentSpace.addOccupant(newFireMarker.GetComponent<GameUnit>());
            currentSpace.setSpaceStatus(SpaceStatus.Fire);


        }


    }

    public void randomizePOI()
    {

          
        //randomize between 1 and 6
        int col = Random.Range(1, 8);
        //randomize between 1 and 8
        int row = Random.Range(1, 6);

        if (containsFireORSmoke(col, row))
        {
            return;
        }

        if (alreadyPlaced(col, row))
        {
            return;
        }

        

        Space currentSpace = grid[col, row];
        Vector3 position = currentSpace.worldPosition;
        GameObject POI = Instantiate(Resources.Load("PhotonPrefabs/Prefabs/POIs/POI")) as GameObject;
        Vector3 newPosition = new Vector3(position.x, position.y, -5);

        POI.GetComponent<Transform>().position = newPosition;
        POI.GetComponent<GameUnit>().setCurrentSpace(currentSpace);
        POI.GetComponent<GameUnit>().setType(FlashPointGameConstants.GAMEUNIT_TYPE_POI);
        POI.GetComponent<GameUnit>().setPhysicalObject(POI);
        currentSpace.addOccupant(POI.GetComponent<POI>());
        numOfActivePOI++;

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

            randomizePOI();

        }
        else if (evCode == (byte)PhotonEventCodes.PlaceInitialFireMarker)
        {

            placeFireMarker();
                
        }
        else if (evCode == (byte)PhotonEventCodes.BoardSetup)
        {
            BoardSetup();
        }
    }
}
