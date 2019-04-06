using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.IO;
using System;


public class GameManager : MonoBehaviourPun
{
    //Initialize Singleton.
    public static GameManager GM;
    public GameObject GameLostUIPrefab;
    public GameObject GameWonUIPrefab;

    //Variables for game status and turn.
    public static string GameStatus;
    public int Turn = 1;

    //Local store of Players.
    public static int NumberOfPlayers;
    public bool isFirstReset;
    public bool isPickSpecialist;
    ArrayList playersListNameCache;

    //Game relevant variables
    public List<Specialist> availableSpecialists = new List<Specialist> 
    { 
        Specialist.Paramedic,
        Specialist.FireCaptain,
        Specialist.ImagingTechnician,
        Specialist.CAFSFirefighter,
        Specialist.HazmatTechinician,
        Specialist.Generalist,
        Specialist.RescueSpecialist,
        Specialist.DriverOperator
    };
    public List<int> freeSpecialistIndex = new List <int> //all specilaists are free at first
    { 
        0,1,2,3,4,5,6,7
    };
    public int buildingDamage;
    static int blackDice;
    static int redDice;
    public static int numOfActivePOI;
    public bool isFamilyGame; //true if family game, false if experienced
    public static Difficulty difficulty; //Recruit, Veteran, Heroic
    public static int savedVictims;
    public static int lostVictims;
    public static int totalPOIs = 15;
    public static int NumFA = 5;
    public static int numVictim = 10;
    public static bool isDestroyingVictim;

    //Network Options

    public static Photon.Realtime.RaiseEventOptions sendToAllOptions = new Photon.Realtime.RaiseEventOptions()
    {
        CachingOption = Photon.Realtime.EventCaching.DoNotCache,
        Receivers = Photon.Realtime.ReceiverGroup.All
    };



    public void Awake()
    {
        if (GM == null)
        {
            GM = this;
            GameStatus = FlashPointGameConstants.GAME_STATUS_SPAWNING_PREFABS;
            NumberOfPlayers = PhotonNetwork.PlayerList.Length;
            isFirstReset = false;
            buildingDamage = 0;
            Turn = 1;
            numOfActivePOI = 0;
            savedVictims = 0;
            lostVictims = 0;
            isPickSpecialist = true;
            playersListNameCache = new ArrayList();
            isFamilyGame = true;
            isDestroyingVictim = false;

        }
        else
        {
            if (GM != this)
            {
                Destroy(GM);
                GM = this;
            }
        }
    }

    void Start()
    {
        initialSetup();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void initialSetup()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            placeInitialFireMarker();

            if (!isFamilyGame)
            {
                placeInitialFireMarkerExperienced();
                placeInitialHotSpot();
                placeInitialAmbulance();
                placeInitialEngine();

                if (difficulty == Difficulty.Recruit) //3 hazmats + 3 initial explosions
                {
                    placeHazmat();
                    placeHazmat();
                    placeHazmat();
                    //resolveExplosion(Space targetSpace);
                    //resolveExplosion(Space targetSpace);
                    //resolveExplosion(Space targetSpace);
                }
                else if (difficulty == Difficulty.Veteran) //4 hazmats + 3 initial explosions
                {
                    placeHazmat();
                    placeHazmat();
                    placeHazmat();
                    placeHazmat();
                    //resolveExplosion(Space targetSpace);
                    //resolveExplosion(Space targetSpace);
                    //resolveExplosion(Space targetSpace);
                }
                else if (difficulty == Difficulty.Heroic) //5 hazmats + 4 initial explosions
                {
                    placeHazmat();
                    placeHazmat();
                    placeHazmat();
                    placeHazmat();
                    placeHazmat();
                    //resolveExplosion(Space targetSpace);
                    //resolveExplosion(Space targetSpace);
                    //resolveExplosion(Space targetSpace);
                    //resolveExplosion(Space targetSpace);
                }
            }
        }
    }

    public void OnAllPrefabsSpawned()
    {
        //TODO Cache the playerList.
        object[] data = new object[PhotonNetwork.PlayerList.Length];
        int i = 0;

        foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
        {
            Debug.Log("OnAllPrefabsSpawned sees " + p.NickName + " with ActorNumber " + p.ActorNumber);
            playersListNameCache.Insert(p.ActorNumber - 1, p.NickName);
            data[i] = playersListNameCache[i];
            i++;
        }

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.CachePlayerNames, data, sendToAllOptions, SendOptions.SendReliable);

        if (!isFamilyGame)
        {
            PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PickSpecialist, null, sendToAllOptions, SendOptions.SendReliable);
        }
        else
        {
            PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceInitialFireFighter, null, sendToAllOptions, SendOptions.SendReliable);
        }

    }


    public static void IncrementTurn()
    {


        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.IncrementTurn, null, sendToAllOptions, SendOptions.SendReliable);
    }

    public void DisplayPlayerTurn()
    {

        //string playerName = (string) playersListNameCache[Turn - 1];
        string playerName = PhotonNetwork.PlayerList[Turn - 1].NickName;
        GameUI.instance.UpdatePlayerTurnName(playerName);
    }

    public void DisplayToConsole(string message)
    {
        GameConsole.instance.FeedbackText.text = message;
    }

    public void DisplayToConsolePlayGame(int turn)
    {
        //string playerName = (string)playersListNameCache[Turn - 1];
        string playerName = (string)PhotonNetwork.PlayerList[turn - 1].NickName;
        string message = "It's " + playerName + "'s turn!";
        GameConsole.instance.FeedbackText.text = message;
    }

    public void DisplayToConsolePlaceFirefighter(int turn)
    {
        //string playerName = (string)playersListNameCache[Turn - 1];
        string playerName = (string)PhotonNetwork.PlayerList[turn - 1].NickName;

        string message = "It's " + playerName + "'s turn to place their Firefighter";
        GameConsole.instance.FeedbackText.text = message;
    }

    public static void advanceFire()
    {
        rollDice();
        Space targetSpace = StateManager.instance.spaceGrid.getGrid()[blackDice, redDice];

        SpaceStatus sp = targetSpace.getSpaceStatus();

        object[] data = new object[] { targetSpace.worldPosition, targetSpace.indexX, targetSpace.indexY };

        if (sp == SpaceStatus.Fire)
        {
            Debug.Log("It's an explosion");
            PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.ResolveExplosion, data, sendToAllOptions, SendOptions.SendReliable);
        }
        else if (sp == SpaceStatus.Smoke)
        {
            Debug.Log("It's turned to Fire.");
            PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.AdvanceFireMarker, data, sendToAllOptions, SendOptions.SendReliable);
        }
        else
        {
            Debug.Log("It's turned to Smoke");
            PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.AdvanceSmokeMarker, data, sendToAllOptions, SendOptions.SendReliable);

        }

        sendResolveFlashOverEvent();

    }

    private static void sendResolveFlashOverEvent()
    {
        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.ResolveFlashOvers, null, sendToAllOptions, SendOptions.SendUnreliable);
    }

    static void rollDice()
    {
        //TODO reset proper randomization

        //System.Random r = new System.Random();
        //blackDice = r.Next(1, 9);
        //redDice = r.Next(1, 7);
        blackDice = 1; 
        redDice = 1;

    }


    void resolveFlashOvers()
    {
        foreach (Space space in StateManager.instance.spaceGrid.grid)
        {
            SpaceStatus status = space.getSpaceStatus();

            if (status == SpaceStatus.Smoke)
            {
                Debug.Log("Found a Smoke marker at " + space.indexX + " and " + space.indexY);

                Space[] neighbors = StateManager.instance.spaceGrid.GetNeighbours(space);
                foreach (Space neighbor in neighbors)
                {
                    if (neighbor != null)
                    {
                        if (neighbor.getSpaceKind() != SpaceKind.Outdoor)
                        {
                            SpaceStatus neighborStatus = neighbor.getSpaceStatus();

                            if (neighborStatus == SpaceStatus.Fire)
                            {
                                space.setSpaceStatus(SpaceStatus.Fire);
                                removeSmokeMarker(space);
                                placeFireMarker(space);
                            }
                        }
                    }
                }
            }
        }
    }

    public bool containsFireORSmoke(int col, int row)
    {
        if (StateManager.instance.spaceGrid.getGrid()[col, row].getSpaceStatus() == SpaceStatus.Fire || StateManager.instance.spaceGrid.getGrid()[col, row].getSpaceStatus() == SpaceStatus.Smoke)
        {
            return true;
        }
        return false;
    }

    public bool alreadyPlaced(int col, int row)
    {
        List<GameUnit> occupants = StateManager.instance.spaceGrid.getGrid()[col, row].getOccupants();
        foreach (GameUnit gu in occupants)
        {
            if (gu.GetType() == typeof(POI) || gu.GetType() == typeof(Hazmat) || gu.GetType() == typeof(HotSpot))
            {
                return true;
            }
        }
        return false;
    }



    public void placeInitialFireMarker()
    {

        int[] rows = new int[] { 2, 2, 3, 3, 3, 3, 4, 5, 5, 6 };
        int[] cols = new int[] { 2, 3, 2, 3, 4, 5, 4, 5, 6, 5 };

        object[] data = { cols, rows };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceInitialFireMarker, data, sendToAllOptions, SendOptions.SendReliable);
    }

    public void placeInitialHotSpot()
    {

        int[] rows = new int[] { 3, 3, 3, 3, 4, 4, 4, 4 };
        int[] cols = new int[] { 3, 4, 5, 6, 6, 5, 4, 3 };

        object[] data = { cols, rows };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceInitialHotSpot, data, sendToAllOptions, SendOptions.SendReliable);
    }
    public void placeInitialFireMarkerExperienced()
    {

        int[] rows = new int[] {  3, 4, 4, 4 };
        int[] cols = new int[] {  6, 6, 5, 3 };

        object[] data = { cols, rows };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceInitialFireMarkerExperienced, data, sendToAllOptions, SendOptions.SendReliable);


    }

    public void randomizePOI()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        int col;
        int row;
        while (true)
        {
            //randomize between 1 and 6
            col = UnityEngine.Random.Range(1, 8);
            //randomize between 1 and 8
            row = UnityEngine.Random.Range(1, 6);
            
            if (containsFireORSmoke(col, row))
            {
                continue;
            }

            if (alreadyPlaced(col, row))
            {
                continue;
            }
            break;
        }


        object[] data = { col, row };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlacePOI, data, sendToAllOptions, SendOptions.SendReliable);
    }

    //TEST FUNCTION NOT USED DURING GAME SOLELY FOR TESTING
    public void testFunctionPlacePOI()
    {
       
            Space currentSpace = StateManager.instance.spaceGrid.getGrid()[1, 2];
            Vector3 position = new Vector3(currentSpace.worldPosition.x, currentSpace.worldPosition.y, -5);
            GameObject POI = Instantiate(Resources.Load("PhotonPrefabs/Prefabs/POIs/POI")) as GameObject;
            //Vector3 newPosition = new Vector3(position.x, position.y, -5);

            POI.GetComponent<Transform>().position = position;
            POI.GetComponent<GameUnit>().setCurrentSpace(currentSpace);
            POI.GetComponent<GameUnit>().setType(FlashPointGameConstants.GAMEUNIT_TYPE_POI);
            POI.GetComponent<GameUnit>().setPhysicalObject(POI);
            currentSpace.addOccupant(POI.GetComponent<POI>());
            numOfActivePOI++;
    }


    //TEST FUNCTION NOT USED DURING GAME SOLELY FOR TESTING 
    public void testFunctionPlaceVictim()
    {

            Space currentSpace = StateManager.instance.spaceGrid.getGrid()[3, 1];
            Vector3 position = new Vector3(currentSpace.worldPosition.x, currentSpace.worldPosition.y, -5);
            GameObject poi = Instantiate(Resources.Load("PhotonPrefabs/Prefabs/POIs/man POI") as GameObject);

            poi.GetComponent<POI>().setPOIKind(POIKind.Victim);
            poi.GetComponent<POI>().setIsFlipped(true);
            poi.GetComponent<Transform>().position = position;
            poi.GetComponent<GameUnit>().setCurrentSpace(currentSpace);
            poi.GetComponent<GameUnit>().setType(FlashPointGameConstants.GAMEUNIT_TYPE_POI);
            poi.GetComponent<GameUnit>().setPhysicalObject(poi);
            currentSpace.addOccupant(poi.GetComponent<POI>());


    }

    void removeSmokeMarker(Space targetSpace)
    {
        int indexX = targetSpace.indexX;
        int indexY = targetSpace.indexY;
        List<GameUnit> spaceOccupants = targetSpace.getOccupants();
        GameUnit targetMarker = null;
        foreach (GameUnit gm in spaceOccupants)
        {
            if (gm.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_SMOKEMARKER)
            {
                Debug.Log("Found a smoke marker");
                targetMarker = gm;
            }
        }
        if (targetMarker != null)
        {
            string message = "Removing Smoke at (" + indexX + "," + indexY + ")";
            Debug.Log(message);
            spaceOccupants.Remove(targetMarker);
            Destroy(targetMarker.physicalObject);
            Destroy(targetMarker);
            targetSpace.setSpaceStatus(SpaceStatus.Safe);

        }
    }

    void removeFireMarker(Space targetSpace)
    {
        int indexX = targetSpace.indexX;
        int indexY = targetSpace.indexY;
        List<GameUnit> spaceOccupants = targetSpace.getOccupants();
        GameUnit targetMarker = null;
        foreach (GameUnit gm in spaceOccupants)
        {
            if (gm.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_FIREMARKER)
            {
                Debug.Log("Found a firemarker");
                targetMarker = gm;
            }
        }
        if (targetMarker != null)
        {
            Debug.Log("Removing targetMarker");
            string message = "Removing Fire at (" + indexX + "," + indexY + ")";
            GameConsole.instance.UpdateFeedback(message);
            spaceOccupants.Remove(targetMarker);
            Destroy(targetMarker.physicalObject);
            Destroy(targetMarker);
            targetSpace.setSpaceStatus(SpaceStatus.Safe);
        }
    }

    void placeFireMarker(Space targetSpace)
    {
        GameObject newFireMarker = Instantiate(Resources.Load("PhotonPrefabs/Prefabs/FireMarker/FireMarker")) as GameObject;
        Vector3 newPosition = new Vector3(targetSpace.worldPosition.x, targetSpace.worldPosition.y, -5);
        newFireMarker.GetComponent<Transform>().position = newPosition;
        newFireMarker.GetComponent<GameUnit>().setCurrentSpace(targetSpace);
        newFireMarker.GetComponent<GameUnit>().setType(FlashPointGameConstants.GAMEUNIT_TYPE_FIREMARKER);
        newFireMarker.GetComponent<GameUnit>().setPhysicalObject(newFireMarker);
        targetSpace.addOccupant(newFireMarker.GetComponent<GameUnit>());
        targetSpace.setSpaceStatus(SpaceStatus.Fire);

        removePOIFromSpace(targetSpace);

        //TODO Find firefighters and select knockdown placement.

        Debug.Log("Firemarker was placed at " + newPosition);

        Debug.Log("It was placed at " + newPosition);

    }

    private void removePOIFromSpace(Space targetSpace)
    {
        List<GameUnit> occupants = targetSpace.getOccupants();

        GameUnit targetVictim = null;
        GameUnit targetPOI = null;
        bool foundUnflippedPOI = false;

        foreach (GameUnit unit in occupants)
        {
            if (unit.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_POI)
            {
                if (unit.GetComponent<POI>().getIsFlipped())
                {
                    targetVictim = unit;
                    Debug.Log("Found a Victim here");
                }
                else
                {
                    targetPOI = unit;
                    Debug.Log("Tagged a POI");
                    foundUnflippedPOI = true;
                }
            }
        }


        if (targetVictim != null)
        {
            Debug.Log("Killing victim");
            occupants.Remove(targetVictim);
            Destroy(targetVictim.physicalObject);
            Destroy(targetVictim);
            GameManager.lostVictims++;
            GameUI.instance.AddLostVictim();
        }

        else if (foundUnflippedPOI)
        {
            Debug.Log("Handling the unflipped poi");

            if (isDestroyingVictim && numVictim > 0)
            {
                isDestroyingVictim = false;
                Debug.Log("Selected the unflipped POI to be a victim");
                occupants.Remove(targetPOI);
                Destroy(targetPOI.physicalObject);
                Destroy(targetPOI);
                lostVictims++;
                numVictim--;
                GameUI.instance.AddLostVictim();
                GameConsole.instance.UpdateFeedback("A victim just perished.");
                numOfActivePOI--;
            }
            else
            {
                isDestroyingVictim = true;
                Debug.Log("Selected the unflipped POI to be a false Alarm");
                occupants.Remove(targetPOI);
                Destroy(targetPOI.physicalObject);
                Destroy(targetPOI);
                NumFA--;
                GameConsole.instance.UpdateFeedback("A false alarm was destroyed");
                numOfActivePOI--;
            }
        }

    }

    void placeSmokeMarker(Space targetSpace)
    {
        GameObject newSmokeMarker = Instantiate(Resources.Load("PhotonPrefabs/Prefabs/Smoke/smoke")) as GameObject;
        Vector3 newPosition = new Vector3(targetSpace.worldPosition.x, targetSpace.worldPosition.y, -5);
        newSmokeMarker.GetComponent<Transform>().position = newPosition;
        newSmokeMarker.GetComponent<GameUnit>().setCurrentSpace(targetSpace);
        newSmokeMarker.GetComponent<GameUnit>().setType(FlashPointGameConstants.GAMEUNIT_TYPE_SMOKEMARKER);
        newSmokeMarker.GetComponent<GameUnit>().setPhysicalObject(newSmokeMarker);
        targetSpace.addOccupant(newSmokeMarker.GetComponent<GameUnit>());
        Debug.Log("Smokemarker was placed at " + newPosition);
    }

    void resolveExplosion(Space targetSpace)
    {

        Space[] neighbors = StateManager.instance.spaceGrid.GetNeighbours(targetSpace);

        for (int i = 0; i < 4; i++)
        {
            if (neighbors[i] != null && neighbors[i].getSpaceKind() != SpaceKind.Outdoor)
            {
                resolveExplosionInDirection(neighbors[i], i);
            }
        }

        //Place necessary damage markers on walls and doors surrounding explosion space.
        Wall[] walls = targetSpace.getWalls();
        Door[] doors = targetSpace.getDoors();
        int indexX = targetSpace.indexX;
        int indexY = targetSpace.indexY;
        if (walls != null)
        {
            for (int i = 0; i < 4; i++)
            {
                int direction = i;
                Wall w = walls[i];
                if (w != null)
                {
                    WallStatus wStatus = w.getWallStatus();

                    //Debug.Log("Wall status before addDamage is " + wStatus);
                    w.addDamage();
                    GameUI.instance.AddDamage(1);
                    //Debug.Log("Adding damage to " + w);
                    //Debug.Log("Wall status after addDamage is " + w.getWallStatus());


                    //Handle wall deletion in relevant spaces
                    if (w.getWallStatus() == WallStatus.Destroyed)
                    {
                        switch (direction)
                        {
                            case 0:
                                targetSpace.addWall(null, direction);
                                int northX = indexX;
                                int northY = indexY - 1;
                                if (northX <= 10 && northY <= 8)
                                {
                                    Space northSpace = StateManager.instance.spaceGrid.grid[northX, northY];
                                    northSpace.addWall(null, 2);
                                }
                                break;
                            case 1:
                                targetSpace.addWall(null, direction);
                                int rightX = indexX + 1;
                                int rightY = indexY;
                                if (rightX <= 10 && rightY <= 8)
                                {
                                    Space rightSpace = StateManager.instance.spaceGrid.grid[rightX, rightY];
                                    rightSpace.addWall(null, 3);
                                }
                                break;
                            case 2:
                                targetSpace.addWall(null, direction);
                                int southX = indexX;
                                int southY = indexY + 1;
                                if (southX <= 10 && southY <= 8)
                                {
                                    Space southSpace = StateManager.instance.spaceGrid.grid[southX, southY];
                                    southSpace.addWall(null, 0);
                                }
                                break;
                            case 3:
                                targetSpace.addWall(null, direction);
                                int leftX = indexX - 1;
                                int leftY = indexY;
                                if (leftX <= 10 && leftY <= 8)
                                {
                                    Space leftSpace = StateManager.instance.spaceGrid.grid[leftX, leftY];
                                    leftSpace.addWall(null, 1);
                                }
                                break;
                        }
                    }
                }
            }       
        }

        if (doors != null)
        {
            for (int i = 0; i < 4; i++)
            {
                if (doors[i] != null)
                {

                    destroyDoor(doors[i]);
                    Debug.Log("Door " + doors[i] + " was destroyed in explosion");

                }
            }
        }

    }

    private void resolveExplosionInDirection(Space targetSpace, int direction)
    {
        //TODO Find and knockdown firefighter
        //TODO destroy POI affected
        //Both of the above can be implemented in place FireMarker.

        if (targetSpace.getSpaceKind() == SpaceKind.Outdoor)
        {
            Debug.Log("Target Space " + targetSpace + " in direction " + direction + "is outdoor");
            return;
        }

        SpaceStatus spaceStatus = targetSpace.getSpaceStatus();
        object[] knockdownData = new object[] { targetSpace.indexX, targetSpace.indexY};

        //If the space is smoke or safe, turn it to fire
        if (spaceStatus == SpaceStatus.Safe)
        {
            placeFireMarker(targetSpace);

            PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.KnockdownFireman, knockdownData, sendToAllOptions, SendOptions.SendReliable);
        }
        else if (spaceStatus == SpaceStatus.Smoke)
        {
            removeSmokeMarker(targetSpace);
            placeFireMarker(targetSpace);

            PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.KnockdownFireman, knockdownData, sendToAllOptions, SendOptions.SendReliable);
        }
        else
        {
            //If there's a wall or door, damage it in relevant direction.
            Wall wallInExplosionDirection = targetSpace.getWalls()[direction];
            Door doorInExplosionDirection = targetSpace.getDoors()[direction];
            if (wallInExplosionDirection != null)
            {
                wallInExplosionDirection.addDamage();
                GameUI.instance.AddDamage(1);
                WallStatus wallInExplosionDirectionStatus = wallInExplosionDirection.getWallStatus();

                //TODO Refactor wall deletion
                //Handle wall deletion in relevant spaces
                if (wallInExplosionDirectionStatus == WallStatus.Destroyed)
                {
                    switch (direction)
                    {
                        case 0:
                            targetSpace.addWall(null, direction);
                            int northX = targetSpace.indexX;
                            int northY = targetSpace.indexY - 1;
                            if (northX <= 10 && northY <= 8)
                            {
                                Space northSpace = StateManager.instance.spaceGrid.grid[northX, northY];
                                northSpace.addWall(null, 2);
                            }
                            break;
                        case 1:
                            targetSpace.addWall(null, direction);
                            int rightX = targetSpace.indexX + 1;
                            int rightY = targetSpace.indexY;
                            if (rightX <= 10 && rightY <= 8)
                            {
                                Space rightSpace = StateManager.instance.spaceGrid.grid[rightX, rightY];
                                rightSpace.addWall(null, 3);
                            }
                            break;
                        case 2:
                            targetSpace.addWall(null, direction);
                            int southX = targetSpace.indexX;
                            int southY = targetSpace.indexY + 1;
                            if (southX <= 10 && southY <= 8)
                            {
                                Space southSpace = StateManager.instance.spaceGrid.grid[southX, southY];
                                southSpace.addWall(null, 0);
                            }
                            break;
                        case 3:
                            targetSpace.addWall(null, direction);
                            int leftX = targetSpace.indexX - 1;
                            int leftY = targetSpace.indexY;
                            if (leftX <= 10 && leftY <= 8)
                            {
                                Space leftSpace = StateManager.instance.spaceGrid.grid[leftX, leftY];
                                leftSpace.addWall(null, 1);
                            }
                            break;
                    }
                }
            }
            //Handle door in that direction.
            else if (doorInExplosionDirection != null)
            {
                destroyDoor(doorInExplosionDirection);
            }
            else //Transmit explosion to next space otherwise
            {
                Space nextSpace = StateManager.instance.spaceGrid.getNeighborInDirection(targetSpace, direction);
                resolveExplosionInDirection(nextSpace, direction);
            }
        }
    }

    private void destroyDoor(Door door)
    {
        door.setDoorStatus(DoorStatus.Destroyed);
        string doorObjectPath = "Board/doorCol45";
        //TODO Change sprite of door.
        //GameObject.Find(doorObjectPath).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("PhotonPrefabs/DamageMarker");

    }
    public static void GameWon()
    {
        GameConsole.instance.UpdateFeedback("YOU WOOOOONNNNNN GANG GANG GANG");
        GameManager.GM.setActivePrefabs("won", true);
    }
    public static void GameLost()
    {
        GameConsole.instance.UpdateFeedback("YOU LOST YOU BEAUTIFUL!");
        GameManager.GM.setActivePrefabs("lost", true);
    }

    public void placeInitialAmbulance()
    {
        int[] rows = new int[] { 5 };
        int[] cols = new int[] { 0 };

        object[] data = { cols, rows };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceInitialAmbulance, data, sendToAllOptions, SendOptions.SendReliable);
    }

    public void placeInitialEngine()
    {
        int[] rows = new int[] { 9 };
        int[] cols = new int[] { 6 };

        object[] data = { cols, rows };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceInitialEngine, data, sendToAllOptions, SendOptions.SendReliable);
    }


    public void placeHazmat()
    {

        int col;
        int row;
        while (true)
        {
            //randomize between 1 and 6
            col = UnityEngine.Random.Range(1, 8);
            //randomize between 1 and 8
            row = UnityEngine.Random.Range(1, 6);

            if (containsFireORSmoke(col, row))
            {
                continue;
            }

            if (alreadyPlaced(col, row))
            {
                continue;
            }
            break;
        }

        object[] data = { col, row };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceHazmats, data, sendToAllOptions, SendOptions.SendReliable);

    }

    public static void replenishPOI()
    {
        if (totalPOIs == 0)
        {
            return;
        }
        switch (numOfActivePOI)
        {
            case 0:
                GameManager.GM.randomizePOI();
                GameManager.GM.randomizePOI();
                GameManager.GM.randomizePOI();
                totalPOIs -= 3;
                break;
            case 1:
                GameManager.GM.randomizePOI();
                GameManager.GM.randomizePOI();
                totalPOIs -= 2;
                break;
            case 2:
                GameManager.GM.randomizePOI();
                totalPOIs -= 1;
                break;
            default:
                break;
        }
    }


    //TODO add that in experienced game
    //add event in the network
    public void replenishPOIExperienced() //experienced game
    {
        //randomize between 1 and 6
        int col = UnityEngine.Random.Range(1, 8);
        //randomize between 1 and 8
        int row = UnityEngine.Random.Range(1, 6);

        while (true)
        {

            if (containsFireORSmoke(col, row) || alreadyPlaced(col, row))
            {
                int[] altSpace = replenishPOIAltSpace(col, row);
                col = altSpace[0];
                row = altSpace[1];
            }
            else
            {
                break;
            }
        }

        object[] data = { col, row };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.ReplenishPOI, data, sendToAllOptions, SendOptions.SendReliable);

    }

    public int[] replenishPOIAltSpace(int col, int row)
    {
        //down arrow
        if ((row == 1 && col >= 2 && col <= 7) || (row == 2 && (col == 4 || col == 5)) || (row == 3 && col == 3) || (row == 4 && (col == 2 || col == 7)))
        {
            return new int[] { col, row + 1 };
        }
        //up arrow
        else if ((row == 3 && (col == 2 || col == 7)) || (row == 4 && col == 6) || (row == 5 && (col == 4 || col == 5)) || (row == 6 && col >= 2 && col <= 7))
        {
            return new int[] { col, row - 1 };
        }
        //right arrow
        else if ((col == 1 && row >= 2 && row <= 5) || (col == 6 && (row == 2 || row == 5)) || (row == 4 && col >= 3 && col <= 5))
        {
            return new int[] { col + 1, row };
        }
        //left arrow
        else if ((col == 8 && row >= 2 && row <= 5) || (col == 3 && (row == 2 || row == 5)) || (row == 3 && col >= 4 && col <= 6))
        {
            return new int[] { col - 1, row };
        }
        //right-down arrow
        else if ((col == 1 && row == 1) || (col == 2 && row == 2))
        {
            return new int[] { col + 1, row + 1 };
        }
        //left-down arrow
        else if ((col == 8 && row == 1) || (col == 7 && row == 2))
        {
            return new int[] { col - 1, row + 1 };
        }
        //right-up arrow
        else if ((col == 1 && row == 6) || (col == 2 && row == 5))
        {
            return new int[] { col + 1, row - 1 };
        }
        //left-up arrow
        else if ((col == 8 && row == 6) || (col == 7 && row == 5))
        {
            return new int[] { col - 1, row - 1 };
        }
        else
        {
            return new int[] { 0, 0 }; //failed function
        }
    }

    public void setActivePrefabs(string name, bool boolean)
    {
        if (string.Compare(name, "won") == 0)
        {
            GameWonUIPrefab.SetActive(boolean);
        }
        else
        {
            GameLostUIPrefab.SetActive(boolean);
        }
    }

    public static void FlipPOI(Space space)
    {
        string[] mylist = new string[] {
            "man POI", "woman POI", "false alarm", "dog POI"
        };

        int currentSpaceX = space.indexX;
        int currentSpaceY = space.indexY;
        string POIname = "";
        int r;

        while (true)
        {
            r = UnityEngine.Random.Range(0, mylist.Length - 1);
            if (string.Compare(mylist[r], "false alarm") == 0 && GameManager.NumFA <= 0)
                continue;
            else
            {
                if (GameManager.numVictim <= 0)
                    continue;
            }
            break;
        }

        POIname = mylist[r];

        object[] data = { currentSpaceX, currentSpaceY, POIname };


        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.FlipPOI, data, sendToAllOptions, SendOptions.SendReliable);

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

        //Increment turn.
        if (evCode == (byte)PhotonEventCodes.IncrementTurn)
        {
            Turn++;
            Debug.Log("Turn is now " + Turn);
            Debug.Log("number of players is " +NumberOfPlayers);
            if (Turn > NumberOfPlayers)
            {
                Debug.Log("resetting  turn");

                if (isPickSpecialist)
                {
                    Debug.Log("changing pick specialist to false");
                    isPickSpecialist = false;
                    PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceInitialFireFighter, null, sendToAllOptions, SendOptions.SendReliable);
                }
                else if (isFirstReset)
                {
                    //change the status to play game
                    GameStatus = FlashPointGameConstants.GAME_STATUS_PLAY_GAME;
                    FiremanUI.instance.SetAP(4);
                    GameUI.instance.AddGameState(GameStatus);
                    isFirstReset = false;
                    Turn = 1;
                    DisplayPlayerTurn();
                    DisplayToConsolePlayGame(Turn);
                    randomizePOI();
                    randomizePOI();
                    randomizePOI();
                }
                else
                {
                    Turn = 1;
                    DisplayPlayerTurn();
                    DisplayToConsolePlayGame(Turn);
                }
            }
            else
            {
                if (isFirstReset)
                {
                    DisplayToConsolePlaceFirefighter(Turn);
                }
                else
                {
                    DisplayToConsolePlayGame(Turn);
                }
                DisplayPlayerTurn();

            }
        }

        else if (evCode == (byte)PhotonEventCodes.PlaceInitialFireFighter)
        {
            Turn = 1;
            isFirstReset = true;
            GameStatus = FlashPointGameConstants.GAME_STATUS_INITIALPLACEMENT;
            DisplayPlayerTurn();
            DisplayToConsolePlaceFirefighter(Turn);
            GameUI.instance.AddGameState(GameStatus);

        }
        else if (evCode == (byte)PhotonEventCodes.AdvanceFireMarker)
        {
            object[] dataReceived = eventData.CustomData as object[];
            Vector3 receivedPosition = (Vector3)dataReceived[0];
            int indexX = (int)dataReceived[1];
            int indexY = (int)dataReceived[2];

            Space targetSpace = StateManager.instance.spaceGrid.getGrid()[indexX, indexY];

            removeSmokeMarker(targetSpace);
            targetSpace.setSpaceStatus(SpaceStatus.Fire);
            placeFireMarker(targetSpace);

            object[] knockdownData = new object[] {indexX, indexY};
            PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.KnockdownFireman, knockdownData, sendToAllOptions, SendOptions.SendReliable);
        }
        else if(evCode == (byte)PhotonEventCodes.KnockdownFireman) { //pass the space x, and space y for data
            object[] dataReceived = eventData.CustomData as object[];
            int x = (int) dataReceived[0];
            int y = (int) dataReceived[1];

            Space space = StateManager.instance.spaceGrid.grid[x, y];
            Space ambulanceSpot = StateManager.instance.spaceGrid.getClosestAmbulanceSpot(space);
            Debug.Log("ambulance spot x: " + ambulanceSpot.indexX + ", y: " + ambulanceSpot.indexY);

            Vector3 pos = new Vector3(ambulanceSpot.worldPosition.x, ambulanceSpot.worldPosition.y, -10);

            List<GameUnit> occupants = space.occupants;
            Debug.Log("OCCUPANTS LIST LENGTH IS " + occupants.Count);
            foreach (GameUnit gameUnit in occupants) {
                if (gameUnit.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_FIREMAN) {
                    gameUnit.transform.position = pos;
                    gameUnit.GetComponent<Fireman>().setCurrentSpace(ambulanceSpot);
                    ambulanceSpot.addOccupant(gameUnit);
                }
                Debug.Log("GameUnit type is " + gameUnit.getType());
            }
        }

        else if (evCode == (byte)PhotonEventCodes.AdvanceSmokeMarker)
        {
            object[] dataReceived = eventData.CustomData as object[];
            Vector3 receivedPosition = (Vector3)dataReceived[0];
            int indexX = (int)dataReceived[1];
            int indexY = (int)dataReceived[2];

            Space targetSpace = StateManager.instance.spaceGrid.getGrid()[indexX, indexY];
            targetSpace.setSpaceStatus(SpaceStatus.Smoke);
            placeSmokeMarker(targetSpace);
        }
        else if (evCode == (byte)PhotonEventCodes.RemoveFireMarker)
        {
            object[] dataReceived = eventData.CustomData as object[];
            int indexX = (int)dataReceived[0];
            int indexY = (int)dataReceived[1];

            Space targetSpace = StateManager.instance.spaceGrid.grid[indexX, indexY];

            removeFireMarker(targetSpace);

        }
        else if (evCode == (byte)PhotonEventCodes.RemoveSmokeMarker)
        {
            object[] dataReceived = eventData.CustomData as object[];
            int indexX = (int)dataReceived[0];
            int indexY = (int)dataReceived[1];

            Space targetSpace = StateManager.instance.spaceGrid.grid[indexX, indexY];

            removeSmokeMarker(targetSpace);

        }
        else if (evCode == (byte)PhotonEventCodes.ChopWall)
        {
            object[] dataReceived = eventData.CustomData as object[];
            int indexX = (int)dataReceived[0];
            int indexY = (int)dataReceived[1];
            int direction = (int)dataReceived[2];

            Space targetSpace = StateManager.instance.spaceGrid.grid[indexX, indexY];
            Wall targetWall = targetSpace.getWalls()[direction];
            /*if(targetWall != null)
            {*/
            Debug.Log("before chop, wall status: " + targetWall.getWallStatus());
            Debug.Log("before chop, damage counter: " + GameManager.GM.buildingDamage);

            targetWall.addDamage();
            GameUI.instance.AddDamage(1);

            Debug.Log("after chop, wall status: " + targetWall.getWallStatus());
            Debug.Log("after chop, damage counter: " + GameManager.GM.buildingDamage);

            if (targetWall.getWallStatus() == WallStatus.Destroyed)
            {
                switch (direction)
                {
                    case 0:
                        targetSpace.addWall(null, direction);
                        int northX = indexX;
                        int northY = indexY - 1;
                        if (northX <= 10 && northY <= 8)
                        {
                            Space northSpace = StateManager.instance.spaceGrid.grid[northX, northY];
                            northSpace.addWall(null, 2);
                        }
                        break;
                    case 1:
                        targetSpace.addWall(null, direction);
                        int rightX = indexX + 1;
                        int rightY = indexY;
                        if (rightX <= 10 && rightY <= 8)
                        {
                            Space rightSpace = StateManager.instance.spaceGrid.grid[rightX, rightY];
                            rightSpace.addWall(null, 3);
                        }
                        break;
                    case 2:
                        targetSpace.addWall(null, direction);
                        int southX = indexX;
                        int southY = indexY + 1;
                        if (southX <= 10 && southY <= 8)
                        {
                            Space southSpace = StateManager.instance.spaceGrid.grid[southX, southY];
                            southSpace.addWall(null, 0);
                        }
                        break;
                    case 3:
                        targetSpace.addWall(null, direction);
                        int leftX = indexX - 1;
                        int leftY = indexY;
                        if (leftX <= 10 && leftY <= 8)
                        {
                            Space leftSpace = StateManager.instance.spaceGrid.grid[leftX, leftY];
                            leftSpace.addWall(null, 1);
                        }
                        break;
                }
            }

            if (buildingDamage >= 24)
            {
                //Building colapses
                Debug.Log("u just lost YIKESSS");
                GameLost();

            }

            /*
            if (targetWall.getWallStatus() == WallStatus.Damaged)
            {
                //place damage marker
                GameObject newDamageMarker = Instantiate(Resources.Load("PhotonPrefabs/Prefabs/DamageMarker/damageMarker")) as GameObject;
                Vector3 wallPosition = targetWall.GetComponent<Transform>().position;
                Vector3 newPosition = new Vector3(wallPosition.x, wallPosition.y, -5);
                newDamageMarker.GetComponent<Transform>().position = newPosition;
                Debug.Log("It was placed at " + newPosition);

            }
            else if (targetWall.getWallStatus() == WallStatus.Destroyed)
            {
                //destroy wall
                Debug.Log("destroy wall");

            }
            //}
            */

        }

        else if (evCode == (byte)PhotonEventCodes.PlacePOI)
        {
            object[] dataReceived = eventData.CustomData as object[];
            int col = (int)dataReceived[0];
            int row = (int)dataReceived[1];

            Space currentSpace = StateManager.instance.spaceGrid.getGrid()[col, row];
            Vector3 position = currentSpace.worldPosition;
            GameObject POI = Instantiate(Resources.Load("PhotonPrefabs/Prefabs/POIs/POI")) as GameObject;
            Vector3 newPosition = new Vector3(position.x, position.y, -5);

            POI.GetComponent<Transform>().position = newPosition;
            POI.GetComponent<GameUnit>().setCurrentSpace(currentSpace);
            POI.GetComponent<GameUnit>().setType(FlashPointGameConstants.GAMEUNIT_TYPE_POI);
            POI.GetComponent<GameUnit>().setPhysicalObject(POI);
            currentSpace.addOccupant(POI.GetComponent<POI>());
            numOfActivePOI++;
            totalPOIs--;
        }
        else if (evCode == (byte)PhotonEventCodes.PlaceInitialFireMarker)
        {

            object[] dataReceived = eventData.CustomData as object[];
            int[] cols = (int[])dataReceived[0];
            int[] rows = (int[])dataReceived[1];

            for (int i = 0; i < rows.Length; i++)
            {
                Space currentSpace = StateManager.instance.spaceGrid.getGrid()[cols[i], rows[i]];
                Vector3 position = currentSpace.worldPosition;
                GameObject newFireMarker = Instantiate(Resources.Load("PhotonPrefabs/Prefabs/FireMarker/FireMarker")) as GameObject;
                Vector3 newPosition = new Vector3(position.x, position.y, -5);

                newFireMarker.GetComponent<Transform>().position = newPosition;
                newFireMarker.GetComponent<GameUnit>().setCurrentSpace(currentSpace);
                newFireMarker.GetComponent<GameUnit>().setType(FlashPointGameConstants.GAMEUNIT_TYPE_FIREMARKER);
                newFireMarker.GetComponent<GameUnit>().setPhysicalObject(newFireMarker);
                currentSpace.addOccupant(newFireMarker.GetComponent<GameUnit>());
                currentSpace.setSpaceStatus(SpaceStatus.Fire);
            }

        }
        else if (evCode == (byte)PhotonEventCodes.ResolveFlashOvers)
        {
            resolveFlashOvers();
        }
        else if (evCode == (byte)PhotonEventCodes.ResolveExplosion)
        {
            object[] dataReceived = eventData.CustomData as object[];
            Vector3 receivedPosition = (Vector3)dataReceived[0];
            int indexX = (int)dataReceived[1];
            int indexY = (int)dataReceived[2];

            Space targetSpace = StateManager.instance.spaceGrid.grid[indexX, indexY];

            resolveExplosion(targetSpace);

        } 
        else if (evCode == (byte)PhotonEventCodes.PlaceHazmats)
        {
            object[] dataReceived = eventData.CustomData as object[];
            int col = (int)dataReceived[0];
            int row = (int)dataReceived[1];

            Space currentSpace = StateManager.instance.spaceGrid.getGrid()[col, row];
            Vector3 position = currentSpace.worldPosition;
            GameObject Hazmat = Instantiate(Resources.Load("PhotonPrefabs/Prefabs/Hazmat/hazmat")) as GameObject;
            Vector3 newPosition = new Vector3(position.x, position.y, -5);

            Hazmat.GetComponent<Transform>().position = newPosition;
            Hazmat.GetComponent<GameUnit>().setCurrentSpace(currentSpace);
            Hazmat.GetComponent<GameUnit>().setType(FlashPointGameConstants.GAMEUNIT_TYPE_HAZMAT);
            Hazmat.GetComponent<GameUnit>().setPhysicalObject(Hazmat);
            currentSpace.addOccupant(Hazmat.GetComponent<Hazmat>());
        }
        else if (evCode == (byte)PhotonEventCodes.PlaceInitialHotSpot)
        {
            object[] dataReceived = eventData.CustomData as object[];
            int[] cols = (int[])dataReceived[0];
            int[] rows = (int[])dataReceived[1];

            for (int i = 0; i < rows.Length; i++)
            {
                Space currentSpace = StateManager.instance.spaceGrid.getGrid()[cols[i], rows[i]];
                Vector3 position = currentSpace.worldPosition;
                GameObject newHotSpot = Instantiate(Resources.Load("PhotonPrefabs/Prefabs/HotSpot/hotspot")) as GameObject;
                Vector3 newPosition = new Vector3(position.x, position.y, -5);

                newHotSpot.GetComponent<Transform>().position = newPosition;
                newHotSpot.GetComponent<GameUnit>().setCurrentSpace(currentSpace);
                newHotSpot.GetComponent<GameUnit>().setType(FlashPointGameConstants.GAMEUNIT_TYPE_HOTSPOT);
                newHotSpot.GetComponent<GameUnit>().setPhysicalObject(newHotSpot);
                currentSpace.addOccupant(newHotSpot.GetComponent<GameUnit>());
                currentSpace.setSpaceStatus(SpaceStatus.Fire);
            }
        }
        else if (evCode == (byte)PhotonEventCodes.PlaceInitialAmbulance)
        {
            object[] dataReceived = eventData.CustomData as object[];
            int[] cols = (int[])dataReceived[0];
            int[] rows = (int[])dataReceived[1];

            for (int i = 0; i < rows.Length; i++)
            {
                Space currentSpace = StateManager.instance.spaceGrid.getGrid()[5, 0];
                Vector3 position = currentSpace.worldPosition;
                GameObject Ambulance = Instantiate(Resources.Load("PhotonPrefabs/Prefabs/Vehicles/ambulance")) as GameObject;
                Vector3 ambulancePosition = new Vector3(position.x+0.5f, position.y, -5);

                Ambulance.GetComponent<Transform>().position = ambulancePosition;
                Ambulance.GetComponent<GameUnit>().setCurrentSpace(currentSpace);
                Ambulance.GetComponent<GameUnit>().setType(FlashPointGameConstants.GAMEUNIT_TYPE_AMBULANCE);
                Ambulance.GetComponent<GameUnit>().setPhysicalObject(Ambulance);
                currentSpace.addOccupant(Ambulance.GetComponent<GameUnit>());
            }
        }
        else if (evCode == (byte)PhotonEventCodes.PlaceInitialEngine)
        {
            object[] dataReceived = eventData.CustomData as object[];
            int[] cols = (int[])dataReceived[0];
            int[] rows = (int[])dataReceived[1];

            for (int i = 0; i < rows.Length; i++)
            {
                //rotate
                Space currentSpaceEngine = StateManager.instance.spaceGrid.getGrid()[9, 6];
                Vector3 position2 = currentSpaceEngine.worldPosition;
                GameObject Engine = Instantiate(Resources.Load("PhotonPrefabs/Prefabs/Vehicles/engine")) as GameObject;
                Vector3 enginePosition = new Vector3(position2.x, position2.y+0.5f, -5);

                Engine.GetComponent<Transform>().position = enginePosition;
                Engine.GetComponent<GameUnit>().setCurrentSpace(currentSpaceEngine);
                Engine.GetComponent<GameUnit>().setType(FlashPointGameConstants.GAMEUNIT_TYPE_ENGINE);
                Engine.GetComponent<GameUnit>().setPhysicalObject(Engine);
                currentSpaceEngine.addOccupant(Engine.GetComponent<GameUnit>());

            }
        }

        else if (evCode == (byte)PhotonEventCodes.PlaceInitialFireMarkerExperienced)
        {
            object[] dataReceived = eventData.CustomData as object[];
            int[] cols = (int[])dataReceived[0];
            int[] rows = (int[])dataReceived[1];

            for (int i = 0; i < rows.Length; i++)
            {
                Space currentSpace = StateManager.instance.spaceGrid.getGrid()[cols[i], rows[i]];
                Vector3 position = currentSpace.worldPosition;
                GameObject newFireMarker2 = Instantiate(Resources.Load("PhotonPrefabs/Prefabs/FireMarker/FireMarker")) as GameObject;
                Vector3 newPosition = new Vector3(position.x, position.y, -5);

                newFireMarker2.GetComponent<Transform>().position = newPosition;
                newFireMarker2.GetComponent<GameUnit>().setCurrentSpace(currentSpace);
                newFireMarker2.GetComponent<GameUnit>().setType(FlashPointGameConstants.GAMEUNIT_TYPE_HOTSPOT);
                newFireMarker2.GetComponent<GameUnit>().setPhysicalObject(newFireMarker2);
                currentSpace.addOccupant(newFireMarker2.GetComponent<GameUnit>());
                currentSpace.setSpaceStatus(SpaceStatus.Fire);
            }
        }
        else if(evCode == (byte)PhotonEventCodes.ReplenishPOI)
        {

            object[] dataReceived = eventData.CustomData as object[];
            int col = (int)dataReceived[0];
            int row = (int)dataReceived[1];

            Space currentSpace = StateManager.instance.spaceGrid.getGrid()[col, row];
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
        else if (evCode == (byte)PhotonEventCodes.Door)
        {
            object[] dataReceived = eventData.CustomData as object[];

            int currentSpaceX = (int)dataReceived[0];
            int currentSpaceY = (int)dataReceived[1];

            int doorDir = 4;//forbidden value
            Door[] doors = StateManager.instance.spaceGrid.getGrid()[currentSpaceX, currentSpaceY].getDoors();

            for (int i = 0; i < 4; i++)
            {
                if (doors[i] != null)
                {
                    doorDir = i;
                }
            }
            if (doorDir >= 0 && doorDir <= 3)
            {
                Door door = doors[doorDir];

                if (door.getDoorStatus() == DoorStatus.Open)
                {
                    door.setDoorStatus(DoorStatus.Closed);
                    door.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/closed door");
                }
                else if (door.getDoorStatus() == DoorStatus.Closed)
                {
                    door.setDoorStatus(DoorStatus.Open);
                    door.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/open door");
                }
            }
        }
        else if (evCode == (byte)PhotonEventCodes.FlipPOI)
        {
            object[] dataReceived = eventData.CustomData as object[];

            int currentSpaceX = (int)dataReceived[0];
            int currentSpaceY = (int)dataReceived[1];
            string POIname = (string)dataReceived[2];

            Space curr = StateManager.instance.spaceGrid.getGrid()[currentSpaceX, currentSpaceY];
            List<GameUnit> gameUnits = curr.getOccupants();
            GameUnit questionMark = null;
            foreach (GameUnit gu in gameUnits)
            {
                if (gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_POI)
                {
                    questionMark = gu;
                    break;
                }
            }
            Vector3 position = new Vector3(curr.worldPosition.x, curr.worldPosition.y, -5);
            
            if (string.Compare(POIname, "false alarm") == 0)
            {
                NumFA--;
                gameUnits.Remove(questionMark);
                Destroy(questionMark.physicalObject);
                Destroy(questionMark);
                GameConsole.instance.UpdateFeedback("It was a false alarm!");
                numOfActivePOI--;
                return;
            }
            else
            {
                GameConsole.instance.UpdateFeedback("It was a Victim!");
                numVictim--;
            }
            //Instantiate Object
            GameObject poi = Instantiate(Resources.Load("PhotonPrefabs/Prefabs/POIs/" + POIname) as GameObject);

            poi.GetComponent<POI>().setPOIKind(POIKind.Victim);
            poi.GetComponent<POI>().setIsFlipped(true);
            poi.GetComponent<Transform>().position = position;
            poi.GetComponent<GameUnit>().setCurrentSpace(curr);
            poi.GetComponent<GameUnit>().setType(FlashPointGameConstants.GAMEUNIT_TYPE_POI);
            poi.GetComponent<GameUnit>().setPhysicalObject(poi);

            gameUnits.Remove(questionMark);
            curr.addOccupant(poi.GetComponent<GameUnit>());

            Debug.Log("This is from flip poi. The flipped status is " + poi.GetComponent<POI>().getIsFlipped());


            if (questionMark != null)
            {
                Destroy(questionMark.physicalObject);
                Destroy(questionMark);
            }
           


        }

        /*
        else if (evCode == (byte) PhotonEventCodes.CachePlayerNames)
        {
            object[] receivedData = eventData.CustomData as object[];

            for (int i = 0; i < receivedData.Length; i++)
            {
                Debug.Log("Received at " + i + " the name " + receivedData[i]);
                playersListNameCache.Insert(i, receivedData[i]);
            }
        }*/

    }
}