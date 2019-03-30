using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.IO;

public class GameManager : MonoBehaviourPun
{
    //Initialize Singleton.
    public static GameManager GM;

    //Variables for game status and turn.
    public static string GameStatus;
    public int Turn = 1;

    //Local store of NumberOfPlayers.
    public static int NumberOfPlayers;
    public bool isFirstReset;

    //Game relevant variables
    public int buildingDamage;
    static int blackDice;
    static int redDice;
    public static int numOfActivePOI;
    public bool isFamilyGame; //true if family game, false if experienced
    public static Difficulty difficulty; //Recruit, Veteran, Heroic
    public static int savedVictims;
    public static int lostVictims;
    public static int totalPOIs = 10;

    //Network Options

    public static Photon.Realtime.RaiseEventOptions  sendToAllOptions = new Photon.Realtime.RaiseEventOptions()
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
            NumberOfPlayers = PhotonNetwork.CountOfPlayers;
            isFirstReset = true;
            buildingDamage = 0;
            Turn = 1;
            numOfActivePOI = 0;
            savedVictims = 0;
            lostVictims = 0;
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
        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceInitialFireMarker, null, sendToAllOptions, SendOptions.SendReliable);
        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlacePOI, null, sendToAllOptions, SendOptions.SendReliable);
        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlacePOI, null, sendToAllOptions, SendOptions.SendReliable);
        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlacePOI, null, sendToAllOptions, SendOptions.SendReliable);
        totalPOIs-=3;

        if (!isFamilyGame) 
        {
            PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceInitialFireMarkerExperienced, null, sendToAllOptions, SendOptions.SendReliable);
            PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceInitialHotSpot, null, sendToAllOptions, SendOptions.SendReliable);

            if (difficulty == Difficulty.Recruit) //3 hazmats
            {
                PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceHazmats, null, sendToAllOptions, SendOptions.SendReliable);
                PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceHazmats, null, sendToAllOptions, SendOptions.SendReliable);
                PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceHazmats, null, sendToAllOptions, SendOptions.SendReliable);
            }
            else if (difficulty == Difficulty.Veteran) //4 hazmats
            {
                PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceHazmats, null, sendToAllOptions, SendOptions.SendReliable);
                PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceHazmats, null, sendToAllOptions, SendOptions.SendReliable);
                PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceHazmats, null, sendToAllOptions, SendOptions.SendReliable);
                PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceHazmats, null, sendToAllOptions, SendOptions.SendReliable);
            }
            else if (difficulty == Difficulty.Heroic) //5 hazmats
            {
                PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceHazmats, null, sendToAllOptions, SendOptions.SendReliable);
                PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceHazmats, null, sendToAllOptions, SendOptions.SendReliable);
                PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceHazmats, null, sendToAllOptions, SendOptions.SendReliable);
                PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceHazmats, null, sendToAllOptions, SendOptions.SendReliable);
                PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceHazmats, null, sendToAllOptions, SendOptions.SendReliable);
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnAllPrefabsSpawned()
    {   
           

        Photon.Realtime.RaiseEventOptions options = new Photon.Realtime.RaiseEventOptions()
        {
            CachingOption = Photon.Realtime.EventCaching.DoNotCache,
            Receivers = Photon.Realtime.ReceiverGroup.All
        };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceInitialFireFighter, null, options, SendOptions.SendReliable);

    }

    
    public static void IncrementTurn()
    {

        Photon.Realtime.RaiseEventOptions options = new Photon.Realtime.RaiseEventOptions()
        {
            CachingOption = Photon.Realtime.EventCaching.DoNotCache,
            Receivers = Photon.Realtime.ReceiverGroup.All
        };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.IncrementTurn, null, options, SendOptions.SendReliable);
    }

    public void DisplayPlayerTurn()
    {
        string playerName = PhotonNetwork.PlayerList[Turn - 1].NickName;
        GameUI.instance.UpdatePlayerTurnName(playerName);
    }

    public void DisplayToConsole(string message)
    {
        GameConsole.instance.FeedbackText.text = message;
    }

    public void DisplayToConsolePlayGame(int turn)
    {
        string playerName = PhotonNetwork.PlayerList[turn - 1].NickName;
        string message = "It's " + playerName + "'s turn!";
        GameConsole.instance.FeedbackText.text = message;
    }

    public void DisplayToConsolePlaceFirefighter(int turn)
    {
            string playerName = PhotonNetwork.PlayerList[turn - 1].NickName;
        string message = "It's " + playerName + "'s turn to place their Firefighter";
        GameConsole.instance.FeedbackText.text = message;
    }

    public static void advanceFire()
    {
        rollDice();
        Space targetSpace = StateManager.instance.spaceGrid.getGrid()[blackDice, redDice];
        Debug.Log("Found target space is : " + targetSpace.indexX + " and " + targetSpace.indexY);

        SpaceStatus sp = targetSpace.getSpaceStatus();

        object[] data = new object[] { targetSpace.worldPosition, targetSpace.indexX, targetSpace.indexY };

        if (sp == SpaceStatus.Fire)
        {
            Debug.Log("It's an explosion");

        }
        else if(sp == SpaceStatus.Smoke)
        {
            Debug.Log("It's turned to Fire.");
            PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.AdvanceFireMarker, data, sendToAllOptions, SendOptions.SendReliable);
        }
        else
        {
            Debug.Log("It's turned to Smoke");
            PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.AdvanceSmokeMarker, data, sendToAllOptions, SendOptions.SendReliable);

        }


    }

    static void rollDice()
    {
        //TODO reset proper randomization
        //System.Random r = new System.Random();
        //blackDice = r.Next(1, 9);
        //redDice = r.Next(1, 7);
        blackDice = 1;
        redDice = 6;

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

    public void placeInitialHotSpot()
    {

        int[] rows = new int[] { 3, 3, 3, 3, 4, 4, 4, 4 };
        int[] cols = new int[] { 3, 4, 5, 6, 6, 5, 4, 3 };

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
    public void placeInitialFireMarkerExperienced()
    {

        int[] rows = new int[] { 3, 3, 3, 3, 4, 4, 4, 4 };
        int[] cols = new int[] { 3, 4, 5, 6, 6, 5, 4, 3 };

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

    public void randomizePOI()
    {
        int col;
        int row;
        while(true) 
        { 
            //randomize between 1 and 6
            col = Random.Range(1, 8);
            //randomize between 1 and 8
            row = Random.Range(1, 6);

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

    public static void GameWon()
    {
        GameConsole.instance.UpdateFeedback("YOU WOOOOONNNNNN GANG GANG GANG");
    }
    public static void GameLost()
    {
        GameConsole.instance.UpdateFeedback("YOU LOST YOU BEAUTIFUL!");
        System.Environment.Exit(0);
    }

    public void placeHazmat()
    {

        int col;
        int row;
        while (true)
        {
            //randomize between 1 and 6
            col = Random.Range(1, 8);
            //randomize between 1 and 8
            row = Random.Range(1, 6);

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

            if (Turn > NumberOfPlayers)
            {
                if (isFirstReset)
                {
                    //change the status to play game
                    GameStatus = FlashPointGameConstants.GAME_STATUS_PLAY_GAME;
                    FiremanUI.instance.SetAP(4);
                    GameUI.instance.AddGameState(GameStatus);
                    isFirstReset = false;
                }
                Turn = 1;
                DisplayPlayerTurn();
                DisplayToConsolePlayGame(Turn);
            }
            else
            {
                if (isFirstReset)
                {
                    DisplayToConsolePlaceFirefighter(Turn);
                    DisplayPlayerTurn();
                }
            }
        }

        else if (evCode == (byte)PhotonEventCodes.PlaceInitialFireFighter)
        {
            Turn = 1;
            GameStatus = FlashPointGameConstants.GAME_STATUS_INITIALPLACEMENT;
            DisplayPlayerTurn();
            DisplayToConsolePlaceFirefighter(Turn);
            GameUI.instance.AddGameState(GameStatus);

        }
        else if (evCode == (byte) PhotonEventCodes.AdvanceFireMarker)
        {
            object[] dataReceived = eventData.CustomData as object[];
            Vector3 receivedPosition = (Vector3) dataReceived[0];
            int indexX = (int)dataReceived[1];
            int indexY = (int)dataReceived[2];

            Space targetSpace = StateManager.instance.spaceGrid.getGrid()[indexX, indexY];

            targetSpace.setSpaceStatus(SpaceStatus.Fire);
            GameObject newFireMarker = Instantiate(Resources.Load("PhotonPrefabs/Prefabs/FireMarker/FireMarker")) as GameObject;
            Vector3 newPosition = new Vector3(targetSpace.worldPosition.x, targetSpace.worldPosition.y, 0);
            newFireMarker.GetComponent<Transform>().position = newPosition;
            Debug.Log("It was placed at " + newPosition);
        }

        else if (evCode == (byte)PhotonEventCodes.AdvanceSmokeMarker)
        {


            object[] dataReceived = eventData.CustomData as object[];
            Vector3 receivedPosition = (Vector3)dataReceived[0];
            int indexX = (int)dataReceived[1];
            int indexY = (int)dataReceived[2];

            Space targetSpace = StateManager.instance.spaceGrid.getGrid()[indexX, indexY];


            targetSpace.setSpaceStatus(SpaceStatus.Smoke);
            GameObject newSmokeMarker = Instantiate(Resources.Load("PhotonPrefabs/Prefabs/Smoke/smoke")) as GameObject;
            Vector3 newPosition = new Vector3(targetSpace.worldPosition.x, targetSpace.worldPosition.y, -5);
            newSmokeMarker.GetComponent<Transform>().position = newPosition;
            newSmokeMarker.GetComponent<GameUnit>().setCurrentSpace(targetSpace);
            newSmokeMarker.GetComponent<GameUnit>().setType(FlashPointGameConstants.GAMEUNIT_TYPE_SMOKEMARKER);
            newSmokeMarker.GetComponent<GameUnit>().setPhysicalObject(newSmokeMarker);
            targetSpace.addOccupant(newSmokeMarker.GetComponent<GameUnit>()); 
            Debug.Log("Smokemarker was placed at " + newPosition);
        }
        else if (evCode == (byte)PhotonEventCodes.RemoveFireMarker)
        {
            object[] dataReceived = eventData.CustomData as object[];
            int indexX = (int)dataReceived[0];
            int indexY = (int)dataReceived[1];

            Space targetSpace = StateManager.instance.spaceGrid.grid[indexX, indexY];

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
        else if (evCode == (byte)PhotonEventCodes.RemoveSmokeMarker)
        {
            object[] dataReceived = eventData.CustomData as object[];
            int indexX = (int)dataReceived[0];
            int indexY = (int)dataReceived[1];

            Space targetSpace = StateManager.instance.spaceGrid.grid[indexX, indexY];

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
                Debug.Log("Removing Smoke Marker");
                string message = "Removing Smoke at (" + indexX + "," + indexY + ")";
                GameConsole.instance.UpdateFeedback(message);
                spaceOccupants.Remove(targetMarker);
                Destroy(targetMarker.physicalObject);
                Destroy(targetMarker);
                targetSpace.setSpaceStatus(SpaceStatus.Safe);

            }
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
            randomizePOI();
        }
        else if (evCode == (byte)PhotonEventCodes.PlaceInitialFireMarker)
        {

            placeInitialFireMarker();

        }
        else if (evCode == (byte)PhotonEventCodes.PlaceHazmats)
        {
            placeHazmat();
        }
        else if (evCode == (byte)PhotonEventCodes.PlaceInitialHotSpot)
        {
            placeInitialHotSpot();
        }
        else if (evCode == (byte)PhotonEventCodes.PlaceInitialFireMarkerExperienced)
        {
            placeInitialFireMarkerExperienced();
        }

        /*
        else if (evCode == (byte) PhotonEventCodes.TurnFireToSmoke)
        {
            /*
            object[] dataReceived = eventData.CustomData as object[];
            int indexX = (int)dataReceived[0];
            int indexY = (int)dataReceived[1];

            Space targetSpace = StateManager.instance.spaceGrid.grid[indexX, indexY];

            //TODO change that
            GameConsole.instance.UpdateFeedback("Turning Fire to smoke...");

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
                Debug.Log("Removing Smoke Marker");
                string message = "Removing Smoke at (" + indexX + "," + indexY + ")";
                GameConsole.instance.UpdateFeedback(message);
                spaceOccupants.Remove(targetMarker);
                Destroy(targetMarker.physicalObject);
                Destroy(targetMarker);
                targetSpace.setSpaceStatus(SpaceStatus.Smoke);

            }

            //PLACING SMOKEMARKER

            Vector3 position = targetSpace.worldPosition;
            GameObject newSmokeMarker = Instantiate(Resources.Load("PhotonPrefabs/Prefabs/Smoke/smoke")) as GameObject;

            */


    }


}