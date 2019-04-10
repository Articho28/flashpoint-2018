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

    public Specialist[] availableSpecialists;
    public int[] freeSpecialistIndex; //all specilaists are free at first
    public int buildingDamage;
    public static int blackDice;
    public static int redDice;
    public static int numOfActivePOI;
    public bool isFamilyGame; //true if family game, false if experienced
    public static Difficulty difficulty; //Recruit, Veteran, Heroic
    public static int savedVictims;
    public static int lostVictims;
    public static int totalPOIs = 15;
    public static int NumFA = 5;
    public static int numVictim = 10;
    public static bool isDestroyingVictim;
    public static int placeInitialPOI = 3;
    public static int[] initialFireMarkerRows = new int[] { 2, 2, 3, 3, 3, 3, 4, 5, 5, 6 };
    public static int[] initialFireMarkerColumns = new int[] { 2, 3, 2, 3, 4, 5, 4, 5, 6, 5 };



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
            isPickSpecialist = false;
            playersListNameCache = new ArrayList();
            isFamilyGame = RoomSetup.RM.getIsFamilyGame();
            Debug.Log("From GM: isFamilyGame parameter received to be" + isFamilyGame);
            if (!isFamilyGame)
            {
                int difficultyIndex = RoomSetup.RM.getExperiencedModeDifficultyIndex();
                isPickSpecialist = true;
                switch (difficultyIndex)
                {
                    case 0:
                        difficulty = Difficulty.Recruit;
                        break;
                    case 1:
                        difficulty = Difficulty.Veteran;
                        break;
                    case 2: difficulty = Difficulty.Heroic;
                        break;
                }

            }
            Destroy(GameObject.Find("/RoomSetupTransfer"));
            isDestroyingVictim = false;
            availableSpecialists = new Specialist [10];
            availableSpecialists[0] = Specialist.Paramedic;
            availableSpecialists[1] = Specialist.FireCaptain;
            availableSpecialists[2] = Specialist.ImagingTechnician;
            availableSpecialists[3] = Specialist.CAFSFirefighter;
            availableSpecialists[4] = Specialist.HazmatTechinician;
            availableSpecialists[5] = Specialist.Generalist;
            availableSpecialists[6] = Specialist.RescueSpecialist;
            availableSpecialists[7] = Specialist.DriverOperator;
            availableSpecialists[8] = Specialist.RescueDog;
            availableSpecialists[9] = Specialist.Veteran;
            freeSpecialistIndex = new int[10];
            freeSpecialistIndex[0] = 1;
            freeSpecialistIndex[1] = 1;
            freeSpecialistIndex[2] = 1;
            freeSpecialistIndex[3] = 1;
            freeSpecialistIndex[4] = 1;
            freeSpecialistIndex[5] = 1;
            freeSpecialistIndex[6] = 1;
            freeSpecialistIndex[7] = 1;
            freeSpecialistIndex[8] = 1;
            freeSpecialistIndex[9] = 1;

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
        GameObject.Find("InGame").GetComponent<AudioSource>().Play();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void initialSetup()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("i am master client");
            placeInitialFireMarker();

            if (!isFamilyGame)
            {
                Debug.Log("not family game");

                placeInitialFireMarkerExperienced();
                //placeInitialHotSpot();
                placeInitialAmbulance();
                placeInitialEngine();

                int difficultyIndex;

                if (difficulty == Difficulty.Recruit) //3 hazmats + 3 initial explosions
                {
                    Debug.Log("recruit difficulty");

                    difficultyIndex = 0;

                    //explosion 1
                    Space targetSpace1 = ExperiencedExplosion1();
                    bool hasFire1 = containsFireOrSmoke(targetSpace1.indexX, targetSpace1.indexY);

                    //explosion 2
                    Space targetSpace2 = ExperiencedExplosion2();
                    bool hasFire2 = containsFireOrSmoke(targetSpace2.indexX, targetSpace2.indexY);

                    //explosion 3
                    Space targetSpace3 = ExperiencedExplosion3();
                    bool hasFire3 = containsFireOrSmoke(targetSpace3.indexX, targetSpace3.indexY);

                    object[] experiencedSetupData = new object[] { difficultyIndex, 
                    targetSpace1.indexX, targetSpace1.indexY, hasFire1, 
                    targetSpace2.indexX, targetSpace2.indexY, hasFire2, 
                    targetSpace3.indexX, targetSpace3.indexY, hasFire3, 
                    null, null, null };

                    PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.ResolveInitialExplosionsExperienced, experiencedSetupData, sendToAllOptions, SendOptions.SendReliable);


                }

                else if (difficulty == Difficulty.Veteran) //4 hazmats + 3 initial explosions
                {
                    Debug.Log("veteran difficulty");

                    difficultyIndex = 1;

                    //explosion 1
                    Space targetSpace1 = ExperiencedExplosion1();
                    bool hasFire1 = containsFireOrSmoke(targetSpace1.indexX, targetSpace1.indexY);

                    //explosion 2
                    Space targetSpace2 = ExperiencedExplosion2();
                    bool hasFire2 = containsFireOrSmoke(targetSpace2.indexX, targetSpace2.indexY);

                    //explosion 3
                    Space targetSpace3 = ExperiencedExplosion3();
                    bool hasFire3 = containsFireOrSmoke(targetSpace3.indexX, targetSpace3.indexY);

                    object[] experiencedSetupData = new object[] { difficultyIndex,
                    targetSpace1.indexX, targetSpace1.indexY, hasFire1,
                    targetSpace2.indexX, targetSpace2.indexY, hasFire2,
                    targetSpace3.indexX, targetSpace3.indexY, hasFire3,
                    null, null, null };

                    PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.ResolveInitialExplosionsExperienced, experiencedSetupData, sendToAllOptions, SendOptions.SendReliable);


                }

                else if (difficulty == Difficulty.Heroic) //5 hazmats + 4 initial explosions
                {
                    Debug.Log("heroic difficulty");

                    difficultyIndex = 2;

                    //explosion 1
                    Space targetSpace1 = ExperiencedExplosion1();
                    bool hasFire1 = containsFireOrSmoke(targetSpace1.indexX, targetSpace1.indexY);

                    //explosion 2
                    Space targetSpace2 = ExperiencedExplosion2();
                    bool hasFire2 = containsFireOrSmoke(targetSpace2.indexX, targetSpace2.indexY);

                    //explosion 3
                    Space targetSpace3 = ExperiencedExplosion3();
                    bool hasFire3 = containsFireOrSmoke(targetSpace3.indexX, targetSpace3.indexY);

                    //explosion 4
                    Space targetSpace4 = ExperiencedExplosion2();
                    bool hasFire4 = containsFireOrSmoke(targetSpace4.indexX, targetSpace4.indexY);


                    object[] experiencedSetupData = new object[] { difficultyIndex,
                    targetSpace1.indexX, targetSpace1.indexY, hasFire1,
                    targetSpace2.indexX, targetSpace2.indexY, hasFire2,
                    targetSpace3.indexX, targetSpace3.indexY, hasFire3,
                    targetSpace4.indexX, targetSpace4.indexY, hasFire4 };



                    PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.ResolveInitialExplosionsExperienced, experiencedSetupData, sendToAllOptions, SendOptions.SendReliable);

                }
               
            }

            object[] data5 = new object[] { };
            PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.InitializePOI, data5, sendToAllOptions, SendOptions.SendReliable);

        }
    }

    public Space ExperiencedExplosion1()
    {
        rollDice();

        Debug.Log("This is explosion 1");
        Debug.Log("red: " + redDice);
        Debug.Log("black: " + blackDice);
        Space targetSpace = StateManager.instance.spaceGrid.getGrid()[3, 3]; //random value for start hehe
        if (blackDice == 1)
        {
            targetSpace = StateManager.instance.spaceGrid.getGrid()[3, 3];
        }
        else if (blackDice == 2)
        {
            targetSpace = StateManager.instance.spaceGrid.getGrid()[4, 3];
        }
        else if (blackDice == 3)
        {
            targetSpace = StateManager.instance.spaceGrid.getGrid()[5, 3];
        }
        else if (blackDice == 4)
        {
            targetSpace = StateManager.instance.spaceGrid.getGrid()[6, 3];
        }
        else if (blackDice == 5)
        {
            targetSpace = StateManager.instance.spaceGrid.getGrid()[6, 4];
        }
        else if (blackDice == 6)
        {
            targetSpace = StateManager.instance.spaceGrid.getGrid()[5, 4];
        }
        else if (blackDice == 7)
        {
            targetSpace = StateManager.instance.spaceGrid.getGrid()[4, 4];
        }
        else if (blackDice == 8)
        {
            targetSpace = StateManager.instance.spaceGrid.getGrid()[3, 4];
        }
        return targetSpace;
    }

    public Space ExperiencedExplosion2()
    {
        rollDice();
        Debug.Log("red2: " + redDice);
        Debug.Log("black2: " + blackDice);
        Space targetSpace = StateManager.instance.spaceGrid.getGrid()[blackDice , redDice];
        while (targetSpace.getSpaceStatus() == SpaceStatus.Fire)
        {
            rollDice();
            targetSpace = StateManager.instance.spaceGrid.getGrid()[blackDice, redDice];
        }
        return targetSpace;
    }

    public Space ExperiencedExplosion3()
    {
        int oldBlack = blackDice;
        int otherSideBlack = 9 - oldBlack;
        rollDice();
        Debug.Log("red2: " + redDice);
        Debug.Log("black2: " + otherSideBlack);
        Space targetSpace = StateManager.instance.spaceGrid.getGrid()[otherSideBlack, redDice];
        while (targetSpace.getSpaceStatus() == SpaceStatus.Fire)
        {
            rollDice();
            targetSpace = StateManager.instance.spaceGrid.getGrid()[blackDice, redDice];
        }
        return targetSpace;
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

    public static void rollDice()
    {
        System.Random r = new System.Random();
        blackDice = r.Next(1, 9);
        redDice = r.Next(1, 7);
        //blackDice = 3; 
        //redDice = 3;

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
                                removeSmokeMarker(space);
                                placeFireMarker(space);
                            }
                        }
                    }
                }
            }
        }
    }

    public bool containsFireOrSmoke(int col, int row)
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
            if (gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_POI 
                || gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_HAZMAT 
                || gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_HOTSPOT
                || gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_FIREMAN)
            {
                return true;
            }
        }
        return false;
    }

    public bool alreadyPlacedHazmat(int col, int row)
    {
        List<GameUnit> occupants = StateManager.instance.spaceGrid.getGrid()[col, row].getOccupants();
        foreach (GameUnit gu in occupants)
        {
            if (gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_HAZMAT)
            {
                return true;
            }
        }
        return false;
    }



    public void placeInitialFireMarker()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

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
        if (!PhotonNetwork.IsMasterClient)
            return;

        int[] rows = new int[] {  3, 4, 4, 4 };
        int[] cols = new int[] {  6, 6, 5, 3 };

        object[] data = { cols, rows };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceInitialFireMarker, data, sendToAllOptions, SendOptions.SendReliable);


    }

    public void randomizePOI()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        int col;
        int row;
        while (true)
        {
            col = UnityEngine.Random.Range(1, 8);
            row = UnityEngine.Random.Range(1, 6);

            if (containsFireOrSmoke(col, row))
            {
                continue;
            }

            if (alreadyPlaced(col, row))
            {
                continue;
            }
            break;
        }

        object[] data = { col, row , GameManager.numOfActivePOI };
        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlacePOI, data, sendToAllOptions, SendOptions.SendReliable);
    }

    public void randomizePOIExperienced()
    {
        //randomize between 1 and 6
        int col = UnityEngine.Random.Range(1, 8);
        //randomize between 1 and 8
        int row = UnityEngine.Random.Range(1, 6);

        while (true)
        {
            if (GameManager.GM.containsFireOrSmoke(col, row) || GameManager.GM.alreadyPlaced(col, row))
            {
                int[] altSpace = GameManager.GM.replenishPOIAltSpace(col, row);
                col = altSpace[0];
                row = altSpace[1];
            }
            else
            {
                break;
            }
        }

        object[] data = { col, row, GameManager.numOfActivePOI };
        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.ReplenishPOI, data, sendToAllOptions, SendOptions.SendReliable);
    }

    public void removeSmokeMarker(Space targetSpace)
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

    public void removeFireMarker(Space targetSpace)
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

        //knockdown placement - handles carried poi's
        Space ambulanceSpot = StateManager.instance.spaceGrid.getClosestAmbulanceSpot(targetSpace);
        List<Fireman> firemen = targetSpace.getFiremen();
        foreach (Fireman fireman in firemen) {
            if (fireman.spec == Specialist.Generalist) StartCoroutine(performVeteran());
            else knockdownFireman(fireman, ambulanceSpot);
        }

        removePOIFromSpace(targetSpace);
    }

    IEnumerator performVeteran() {
        GameConsole.instance.UpdateFeedback("Awaiting your choice...\npress 0 if you would like to dodge\npress 1 if you would like to get knocked down");
        yield return StartCoroutine(UserInputManager.instance.waitForValidUserInput(new KeyCode[] { KeyCode.Alpha0, KeyCode.Alpha1 }));
       
        KeyCode input = UserInputManager.instance.validInput;
        if (input == KeyCode.Alpha0) {
            GameConsole.instance.UpdateFeedback("you pressed 0!");
        }
        else if (input == KeyCode.Alpha1) {
            GameConsole.instance.UpdateFeedback("you pressed 1!");
        }
    }

    private static void knockdownFireman(Fireman fireman, Space ambulanceSpot) {
        Space currSpace = fireman.getCurrentSpace();
        Fireman.moveFirefighter(fireman, currSpace, ambulanceSpot);

        Dictionary<int, Victim> d = StateManager.instance.firemanCarriedVictims;
        if (d.ContainsKey(fireman.PV.ViewID)) {
            object[] data = { currSpace.indexX, currSpace.indexY, fireman.PV.ViewID, false };
            PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.RemoveVictim, data, sendToAllOptions, SendOptions.SendReliable);
        }
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

            if (lostVictims > 4)
            {
                Debug.Log("You lost bruh");
                GameLost();
            }
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

                //object[] data = { targetSpace.indexX, targetSpace.indexY, PV.ViewID, true };
                //PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.RemoveVictim, data, sendToAllOptions, SendOptions.SendReliable);
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
        targetSpace.setSpaceStatus(SpaceStatus.Smoke);
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

                        if (buildingDamage >= 24)
                        {
                            Debug.Log("You lost bruh");
                            GameLost();
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
        }
        else if (spaceStatus == SpaceStatus.Smoke)
        {
            removeSmokeMarker(targetSpace);
            placeFireMarker(targetSpace);
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
                if (doorInExplosionDirection.getDoorStatus() == DoorStatus.Destroyed)
                {
                    Space nextSpace = StateManager.instance.spaceGrid.getNeighborInDirection(targetSpace, direction);
                    resolveExplosionInDirection(nextSpace, direction);
                }
                else
                {
                    destroyDoor(doorInExplosionDirection);
                }
            }
            else //Transmit explosion to next space otherwise
            {
                Space nextSpace = StateManager.instance.spaceGrid.getNeighborInDirection(targetSpace, direction);
                resolveExplosionInDirection(nextSpace, direction);
            }
        }
    }

    public static void RerollRedDie()
    {
        int black = blackDice;
        rollDice();
        blackDice = black;
    }
    public static void RerollBlackDie()
    {
        int red = redDice;
        rollDice();
        redDice = red;
    }

    private void destroyDoor(Door door)
    {
        door.setDoorStatus(DoorStatus.Destroyed);
        door.GetComponent<SpriteRenderer>().sprite = null;
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

    public void placeAmbulanceParkingSpot()
    {
        int[] rows = { 5, 9, 4, 0 };
        int[] cols = { 0, 2, 9, 5 };

        object[] data = { cols, rows };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceAmbulanceParkingSpot, data, sendToAllOptions, SendOptions.SendReliable);
    }

    public void placeEngineParkingSpot()
    {
        int[] rows = { 8, 9, 2, 0 };
        int[] cols = { 0, 5, 9, 1 };

        object[] data = { cols, rows };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceEngineParkingSpot, data, sendToAllOptions, SendOptions.SendReliable);
    }

    public void placeInitialAmbulance()
    {
        int[] rows = { 5 };
        int[] cols = { 0 };

        object[] data = { cols, rows };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceInitialAmbulance, data, sendToAllOptions, SendOptions.SendReliable);
    }

    public void placeInitialEngine()
    {
        int[] rows = { 9 };
        int[] cols = { 5 };

        object[] data = { cols, rows };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceInitialEngine, data, sendToAllOptions, SendOptions.SendReliable);
    }


    public int[] getRandomHazmatPlacementLocation()
    {

        int col;
        int row;
        while (true)
        {
            //randomize between 1 and 6
            col = UnityEngine.Random.Range(1, 8);
            //randomize between 1 and 8
            row = UnityEngine.Random.Range(1, 6);

            if (containsFireOrSmoke(col, row))
            {
                continue;
            }

            if (alreadyPlacedHazmat(col, row))
            {
                continue;
            }
            break;
        }

        int[] coords = new int[] { col, row };

        return coords;

    }

    public void placeHazmat(int x, int y)
    {
        Space currentSpace = StateManager.instance.spaceGrid.getGrid()[x, y];
        Vector3 position = currentSpace.worldPosition;
        GameObject Hazmat = Instantiate(Resources.Load("PhotonPrefabs/Prefabs/Hazmat/hazmat")) as GameObject;
        Vector3 newPosition = new Vector3(position.x, position.y, -5);

        Hazmat.GetComponent<Transform>().position = newPosition;
        Hazmat.GetComponent<GameUnit>().setCurrentSpace(currentSpace);
        Hazmat.GetComponent<GameUnit>().setType(FlashPointGameConstants.GAMEUNIT_TYPE_HAZMAT);
        Hazmat.GetComponent<GameUnit>().setPhysicalObject(Hazmat);
        currentSpace.addOccupant(Hazmat.GetComponent<Hazmat>());
    }

    public void placeHotSpot(int x, int y)
    {
        Space currentSpace = StateManager.instance.spaceGrid.getGrid()[x, y];
        Vector3 position = currentSpace.worldPosition;
        GameObject newHotSpot = Instantiate(Resources.Load("PhotonPrefabs/Prefabs/HotSpot/hotspot")) as GameObject;
        Vector3 newPosition = new Vector3(position.x, position.y, -6);

        newHotSpot.GetComponent<Transform>().position = newPosition;
        newHotSpot.GetComponent<GameUnit>().setCurrentSpace(currentSpace);
        newHotSpot.GetComponent<GameUnit>().setType(FlashPointGameConstants.GAMEUNIT_TYPE_HOTSPOT);
        newHotSpot.GetComponent<GameUnit>().setPhysicalObject(newHotSpot);
        currentSpace.addOccupant(newHotSpot.GetComponent<GameUnit>());
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
                break;
            case 1:
                GameManager.GM.randomizePOI();
                GameManager.GM.randomizePOI();
                break;
            case 2:
                GameManager.GM.randomizePOI();
                break;
            default:
                break;
        }
    }

    public static void replenishPOIExperienced() //experienced game
    {
        if (totalPOIs == 0)
        {
            return;
        }
        switch (numOfActivePOI)
        {
            case 0:
                GameManager.GM.randomizePOIExperienced();
                GameManager.GM.randomizePOIExperienced();
                GameManager.GM.randomizePOIExperienced();
                break;
            case 1:
                GameManager.GM.randomizePOIExperienced();
                GameManager.GM.randomizePOIExperienced();
                break;
            case 2:
                GameManager.GM.randomizePOIExperienced();
                break;
            default:
                break;
        }
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
            Debug.Log("number of players is " + NumberOfPlayers);
            Debug.Log("IN inc turn: status of isFamilyGame is :" + isFamilyGame);
            if (Turn > NumberOfPlayers)
            {
                Debug.Log("resetting  turn");

                if (!isFamilyGame && isPickSpecialist)
                {
                    Debug.Log("changing pick specialist to false");
                    isPickSpecialist = false;
                    PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceInitialFireFighter, null, sendToAllOptions, SendOptions.SendReliable);
                }
                else if (isFirstReset)
                {
                    Debug.Log("Entered is First Reset");
                    //change the status to play game
                    GameStatus = FlashPointGameConstants.GAME_STATUS_PLAY_GAME;
                    //FiremanUI.instance.SetAP(4);
                    GameUI.instance.AddGameState(GameStatus);
                    isFirstReset = false;
                    Turn = 1;
                    DisplayPlayerTurn();
                    DisplayToConsolePlayGame(Turn);
                    initialSetup(); //hazmats + initial explosions (for experienced)
                }
                else
                {
                    Debug.Log("Simply resetting turn to player 1");
                    Turn = 1;
                    DisplayPlayerTurn();
                    DisplayToConsolePlayGame(Turn);
                }
            }
            else
            {
                Debug.Log("Increment turn");
                if (isFirstReset)
                {
                    DisplayToConsolePlaceFirefighter(Turn);
                }
                else if (isPickSpecialist)
                {
                    Fireman.selectSpecialist();

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
        else if (evCode == (byte)PhotonEventCodes.InitializePOI)
        {

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("========INITIALIZING POI WITH THIS GRID========");
                StateManager.instance.spaceGrid.printStatus();
                randomizePOI();
                randomizePOI();
                randomizePOI();
                Debug.Log("===============================================");
            }


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

            int difficultyIndex = (int)dataReceived[0];

            int hazmatIndexX1 = (int)dataReceived[1];
            int hazmatIndexY1 = (int)dataReceived[2];

            placeHazmat(hazmatIndexX1, hazmatIndexY1);

            int hazmatIndexX2 = (int)dataReceived[3];
            int hazmatIndexY2 = (int)dataReceived[4];

            placeHazmat(hazmatIndexX2, hazmatIndexY2);

            int hazmatIndexX3 = (int)dataReceived[5];
            int hazmatIndexY3 = (int)dataReceived[6];

            placeHazmat(hazmatIndexX3, hazmatIndexY3);

            if (difficultyIndex == 1)
            {
                int hazmatIndexX4 = (int)dataReceived[7];
                int hazmatIndexY4 = (int)dataReceived[8];

                placeHazmat(hazmatIndexX4, hazmatIndexY4);


            }

            else if (difficultyIndex == 2)
            {
                int hazmatIndexX4 = (int)dataReceived[7];
                int hazmatIndexY4 = (int)dataReceived[8];

                placeHazmat(hazmatIndexX4, hazmatIndexY4);

                int hazmatIndexX5 = (int)dataReceived[9];
                int hazmatIndexY5 = (int)dataReceived[10];

                placeHazmat(hazmatIndexX5, hazmatIndexY5);
            }
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
        else if (evCode == (byte)PhotonEventCodes.PlaceAmbulanceParkingSpot)
        {
            object[] dataReceived = eventData.CustomData as object[];
            int[] cols = (int[])dataReceived[0];
            int[] rows = (int[])dataReceived[1];

            for (int i = 0; i < rows.Length; i++)
            {
                Space currentSpace = StateManager.instance.spaceGrid.getGrid()[cols[i], rows[i]];
                Vector3 position = currentSpace.worldPosition;
                GameObject AmbulanceParkingSpot = Instantiate(Resources.Load("PhotonPrefabs/Prefabs/ParkingSpots/ambulanceParkingSpot")) as GameObject;
                Vector3 ambulancePosition = new Vector3(position.x, position.y, -5);

                AmbulanceParkingSpot.GetComponent<Transform>().position = ambulancePosition;
                AmbulanceParkingSpot.GetComponent<GameUnit>().setCurrentSpace(currentSpace);
                AmbulanceParkingSpot.GetComponent<GameUnit>().setType(FlashPointGameConstants.GAMEUNIT_TYPE_AMBULANCEPARKINGSPOT);
                AmbulanceParkingSpot.GetComponent<GameUnit>().setPhysicalObject(AmbulanceParkingSpot);
                currentSpace.addOccupant(AmbulanceParkingSpot.GetComponent<GameUnit>());
            }
        }
        else if (evCode == (byte)PhotonEventCodes.PlaceEngineParkingSpot)
        {
            object[] dataReceived = eventData.CustomData as object[];
            int[] cols = (int[])dataReceived[0];
            int[] rows = (int[])dataReceived[1];

            for (int i = 0; i < rows.Length; i++)
            {
                Space currentSpace = StateManager.instance.spaceGrid.getGrid()[cols[i], rows[i]];
                Vector3 position = currentSpace.worldPosition;
                GameObject EngineParkingSpot = Instantiate(Resources.Load("PhotonPrefabs/Prefabs/ParkingSpots/engineParkingSpot")) as GameObject;
                Vector3 enginePosition = new Vector3(position.x, position.y, -5);

                EngineParkingSpot.GetComponent<Transform>().position = enginePosition;
                EngineParkingSpot.GetComponent<GameUnit>().setCurrentSpace(currentSpace);
                EngineParkingSpot.GetComponent<GameUnit>().setType(FlashPointGameConstants.GAMEUNIT_TYPE_ENGINEPARKINGSPOT);
                EngineParkingSpot.GetComponent<GameUnit>().setPhysicalObject(EngineParkingSpot);
                currentSpace.addOccupant(EngineParkingSpot.GetComponent<GameUnit>());
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

                Vector3 ambulancePosition = new Vector3(position.x, position.y, -5);


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
                Space currentSpaceEngine = StateManager.instance.spaceGrid.getGrid()[9, 5];
                Vector3 position2 = currentSpaceEngine.worldPosition;
                GameObject Engine = Instantiate(Resources.Load("PhotonPrefabs/Prefabs/Vehicles/engine")) as GameObject;

                Vector3 enginePosition = new Vector3(position2.x, position2.y, -5);

                Engine.GetComponent<Transform>().position = enginePosition;
                Engine.GetComponent<GameUnit>().setCurrentSpace(currentSpaceEngine);
                Engine.GetComponent<GameUnit>().setType(FlashPointGameConstants.GAMEUNIT_TYPE_ENGINE);
                Engine.GetComponent<GameUnit>().setPhysicalObject(Engine);
                currentSpaceEngine.addOccupant(Engine.GetComponent<GameUnit>());

            }
        }

        else if (evCode == (byte)PhotonEventCodes.ReplenishPOI)
        {

            object[] dataReceived = eventData.CustomData as object[];
            int col = (int)dataReceived[0];
            int row = (int)dataReceived[1];
            int POICounter = (int)dataReceived[2];

            if (POICounter < 3)
            {
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
        else if (evCode == (byte)PhotonEventCodes.SpecialistIsPicked)
        {
            object[] dataReceived = eventData.CustomData as object[];
            int[] updatedIndexList = (int[])dataReceived[0];
            freeSpecialistIndex = updatedIndexList;


        }

        else if (evCode == (byte)PhotonEventCodes.ChangeCrew)
        {
            object[] dataReceived = eventData.CustomData as object[];
            int[] updatedIndexList = (int[])dataReceived[0];

            freeSpecialistIndex = updatedIndexList;

        }
        else if (evCode == (byte)PhotonEventCodes.UpdateCarriedVictimsState)
        { //0: indexX, 1: indexY, 2: index in state dictionary/fireman unique network id
            object[] dataReceived = eventData.CustomData as object[];
            int indexX = (int)dataReceived[0];
            int indexY = (int)dataReceived[1];
            int firemanId = (int)dataReceived[2];

            Space space = StateManager.instance.spaceGrid.grid[indexX, indexY];
            Victim victim = null;
            foreach (GameUnit gu in space.getOccupants())
            {
                //TODO check if victim is carried by another fireman after drop functionality is implemented
                if (gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_POI)
                {
                    Victim v = gu.GetComponent<Victim>();
                    victim = v;
                }
            }

            Dictionary<int, Victim> d = StateManager.instance.firemanCarriedVictims;
            if (d.ContainsKey(firemanId))
            {
                d[firemanId] = victim;
            }
            else d.Add(firemanId, victim);

        }


        else if (evCode == (byte)PhotonEventCodes.CachePlayerNames)
        {
            object[] receivedData = eventData.CustomData as object[];

            for (int i = 0; i < receivedData.Length; i++)
            {
                Debug.Log("Received at " + i + " the name " + receivedData[i]);
                playersListNameCache.Insert(i, receivedData[i]);
            }
        }

        /*
        else if (evCode == (byte) PhotonEventCodes.PlaceFireMarker)
        {
            object[] receivedData = eventData.CustomData as object[];

            int indexX = (int) receivedData[0];
            int indexY = (int)receivedData[1];

            Space targetSpace = StateManager.instance.spaceGrid.getGrid()[indexX, indexY];

            placeFireMarker(targetSpace);
        }*/

        else if (evCode == (byte)PhotonEventCodes.ResolveInitialExplosionsExperienced)
        {
            object[] receivedData = eventData.CustomData as object[];

            int difficultyIndex = (int)receivedData[0];

            int indexX1 = (int)receivedData[1];
            int indexY1 = (int)receivedData[2];
            bool hasFire1 = (bool)receivedData[3];

            Space targetSpace1 = StateManager.instance.spaceGrid.getGrid()[indexX1, indexY1];

            if (!hasFire1)
            {
                placeFireMarker(targetSpace1);
            }

            resolveExplosion(targetSpace1);

            placeHotSpot(targetSpace1.indexX, targetSpace1.indexY);


            int indexX2 = (int)receivedData[4];
            int indexY2 = (int)receivedData[5];
            bool hasFire2 = (bool)receivedData[6];

            Space targetSpace2 = StateManager.instance.spaceGrid.getGrid()[indexX2, indexY2];

            if (!hasFire2)
            {
                placeFireMarker(targetSpace2);
            }


            resolveExplosion(targetSpace2);

            placeHotSpot(targetSpace2.indexX, targetSpace2.indexY);



            int indexX3 = (int)receivedData[7];
            int indexY3 = (int)receivedData[8];
            bool hasFire3 = (bool)receivedData[9];

            Space targetSpace3 = StateManager.instance.spaceGrid.getGrid()[indexX3, indexY3];


            if (!hasFire3)
            {
                placeFireMarker(targetSpace3);
            }

            resolveExplosion(targetSpace3);

            placeHotSpot(targetSpace3.indexX, targetSpace3.indexY);


            if (receivedData[9] != null && receivedData[10] != null && receivedData[11] != null)
            {
                int indexX4 = (int)receivedData[10];
                int indexY4 = (int)receivedData[11];
                bool hasFire4 = (bool)receivedData[12];

                Space targetSpace4 = StateManager.instance.spaceGrid.getGrid()[indexX4, indexY4];

                if (!hasFire4)
                {
                    placeFireMarker(targetSpace4);
                }

                resolveExplosion(targetSpace4);

                placeHotSpot(targetSpace4.indexX, targetSpace4.indexY);

            }

            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            //Always at least three hazmats to place


            int[] hazmatCoords1 = getRandomHazmatPlacementLocation();

            //Make sure we get different coordinates for each hazmat. 
            int[] hazmatCoords2;
            do
            {
                hazmatCoords2 = getRandomHazmatPlacementLocation();
            }
            while (hazmatCoords1[0] == hazmatCoords2[0] && hazmatCoords1[1] == hazmatCoords2[1]);

            int[] hazmatCoords3;
            do
            {
                hazmatCoords3 = getRandomHazmatPlacementLocation();
            }
            while ((hazmatCoords1[0] == hazmatCoords3[0] && hazmatCoords1[1] == hazmatCoords3[1])
            || (hazmatCoords2[0] == hazmatCoords3[0] && hazmatCoords2[1] == hazmatCoords3[1]));


            //Start loading locations to send to clients;
            object[] hazmatLocations = new object[] {difficultyIndex,
            hazmatCoords1[0], hazmatCoords1[1],
            hazmatCoords2[0], hazmatCoords2[1],
            hazmatCoords3[0], hazmatCoords3[1],
            null, null,
            null, null };


            //If the difficulty is at least greater than Recruit, we will have 4 hazmats.
            //Get another location and load it into the data to send.
            int[] hazmatCoords4;

            if (difficultyIndex > 0)
            {
                do
                {
                     hazmatCoords4 = getRandomHazmatPlacementLocation();
                }
                while ((hazmatCoords1[0] == hazmatCoords4[0] && hazmatCoords1[1] == hazmatCoords4[1])
               || (hazmatCoords2[0] == hazmatCoords4[0] && hazmatCoords2[1] == hazmatCoords4[1])
               || (hazmatCoords3[0] == hazmatCoords4[0] && hazmatCoords3[1] == hazmatCoords4[1]));

                hazmatLocations[7] = hazmatCoords4[0];
                hazmatLocations[8] = hazmatCoords4[1];

                int[] hazmatCoords5;

                //If difficulty is Heroic, load another hazmat location.
                if (difficultyIndex > 1)
                {
                    do
                    {
                        hazmatCoords5 = getRandomHazmatPlacementLocation();
                    }
                    while ((hazmatCoords1[0] == hazmatCoords5[0] && hazmatCoords1[1] == hazmatCoords5[1])
                   || (hazmatCoords2[0] == hazmatCoords5[0] && hazmatCoords2[1] == hazmatCoords5[1])
                   || (hazmatCoords3[0] == hazmatCoords5[0] && hazmatCoords3[1] == hazmatCoords5[1])
                   || (hazmatCoords4[0] == hazmatCoords5[0] && hazmatCoords4[1] == hazmatCoords5[1]));

                    hazmatLocations[9] = hazmatCoords5[0];
                    hazmatLocations[10] = hazmatCoords5[1];

                }
            }

            PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceHazmats, hazmatLocations, sendToAllOptions, SendOptions.SendReliable);
        }

    }


}
