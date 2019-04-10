using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class Fireman : GameUnit
{
    int AP;
    int savedAP;
    int commandAP;
    int extinguishAP;
    int moveAP;
    int startOfTurnAP;
    FMStatus status;
    Victim carriedVictim;
    Victim treatedVictim;
    Hazmat carriedHazmat;
    Ambulance movedAmbulance;
    Engine movedEngine;
    private List<Fireman> commandedFiremen;
    private Space commandedSpace;
    public PhotonView PV;
    private static bool isWaitingForInput;
    private bool isExtinguishingFire;
    private bool isChoppingWall;
    private bool isCallingAmbulance;
    private bool isCallingEngine;
    private bool isRidingVehicle;
    private bool isSelectingExtinguishOption;
    private static bool isSelectingSpecialist;
    private bool isChangingCrew;
    private bool isOnEngine;
    private bool isOnAmbulance;
    private bool isIdentifyingPOI;
    private bool isFiringDeckGun;
    private bool isClickingFirefighter;
    private bool driverRerolledBlackDice;
    private bool driverRerolledRedDice;
    private bool isRevealingPOI;
    private bool isCommandingFirefighter;
    private bool isSqueezing;
    public bool isDoubleSpec; //for ap decrementing
    public int actorNumber;
    public ArrayList validInputOptions;
    Space locationArgument;
    public Specialist spec;

    public static Photon.Realtime.RaiseEventOptions sendToAllOptions = new Photon.Realtime.RaiseEventOptions()
    {
        CachingOption = Photon.Realtime.EventCaching.DoNotCache,
        Receivers = Photon.Realtime.ReceiverGroup.All
    };

    void Start()
    {
        //AP = 4;
        actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        savedAP = 0;
        carriedVictim = null;
        movedEngine = null;
        movedAmbulance = null;
        commandedFiremen = new List<Fireman> { };
        PV = GetComponent<PhotonView>();
        isWaitingForInput = false;
        isExtinguishingFire = false;
        isCallingAmbulance = false;
        isCallingEngine = false;
        isRidingVehicle = false;
        validInputOptions = new ArrayList();
        isChoppingWall = false;
        isSelectingExtinguishOption = false;
        isOnEngine = false;
        isOnAmbulance = false;
        isChangingCrew = false;
        isSelectingSpecialist = false;
        isIdentifyingPOI = false;
        isCommandingFirefighter = false;
        isRevealingPOI = false;
        isSqueezing = false;
        isFiringDeckGun = false;
        driverRerolledRedDice = false;
        driverRerolledBlackDice = false;
        isDoubleSpec = false;
        isClickingFirefighter = false;

        if (GameManager.GM.isFamilyGame == true)
        {
            this.spec = Specialist.FamilyGame;
            AP = 4;
            FiremanUI.instance.SetAP(this.getAP());
        }
    }

    void Update()
    {


        if (PV.IsMine && GameManager.GM.Turn == actorNumber && GameManager.GameStatus ==
       FlashPointGameConstants.GAME_STATUS_PLAY_GAME)
        {
            //ADDITIONAL KEYS IN EXPERIENCED GAME
            //Fire the Deck Gun "G"
            //Drive vehicle "H"
            //Crew Change "W"

            if (Input.GetKeyDown(KeyCode.G))
            {
                if(this.spec == Specialist.RescueDog)
                {
                    GameConsole.instance.UpdateFeedback("Rescue dog cannot do this DAWG!");
                    return;
                }
                else if (!GameManager.GM.isFamilyGame)
                {
                    CallDeckGun();
                }
                else
                {
                    GameConsole.instance.UpdateFeedback("This is not available in family game!");
                }
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                if (!GameManager.GM.isFamilyGame)
                {
                    dispose();
                }
            }
            else if (Input.GetKeyDown(KeyCode.H))
            {
                if (this.spec == Specialist.RescueDog)
                {
                    GameConsole.instance.UpdateFeedback("Rescue dog cannot do this DAWG!");
                    return;
                }
                else if (!GameManager.GM.isFamilyGame)
                {
                    CallAmbulance();
                }
                else
                {
                    GameConsole.instance.UpdateFeedback("This is not available in family game!");
                }
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                if (this.spec == Specialist.RescueDog)
                {
                    GameConsole.instance.UpdateFeedback("Rescue dog cannot do this DAWG!");
                    return;
                }
                else if (!GameManager.GM.isFamilyGame)
                {
                    CallEngine();
                }
                else
                {
                    GameConsole.instance.UpdateFeedback("This is not available in family game!");
                }
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                if (this.spec == Specialist.RescueDog)
                {
                    GameConsole.instance.UpdateFeedback("Rescue dog cannot do this DAWG!");
                    return;
                }
                else if (!GameManager.GM.isFamilyGame)
                {
                    GameConsole.instance.UpdateFeedback("Press 1 if you want to ride the ambulance \n" +
                                            "Press 2 if you want to ride the engine \n");
                    isWaitingForInput = true;
                    isRidingVehicle = true;
                }
                else
                {
                    GameConsole.instance.UpdateFeedback("This is not available in family game!");
                }
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                if (this.spec == Specialist.RescueDog)
                {
                    GameConsole.instance.UpdateFeedback("Rescue dog cannot do this DAWG!");
                    return;
                }
                else if (!GameManager.GM.isFamilyGame)
                {
                    exitVehicle();
                }
                else
                {
                    GameConsole.instance.UpdateFeedback("This is not available in family game!");
                }

            }
            else if (Input.GetKeyDown(KeyCode.K))
            {
                if (this.spec == Specialist.RescueDog)
                {
                    GameConsole.instance.UpdateFeedback("Rescue dog cannot do this DAWG!");
                    return;
                }
                else if (!GameManager.GM.isFamilyGame)
                {
                    if (this.spec == Specialist.Paramedic)
                    {
                        treatVictim();
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("You have to be a Paramedic to do this move!");
                    }
                }
                else
                {
                    GameConsole.instance.UpdateFeedback("This is not available in family game!");
                }
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                //Identify POI anywhere on the board
                //Only for imaging technicians
                if (!GameManager.GM.isFamilyGame)
                {
                    identifyPOI();
                }
                else
                {
                    GameConsole.instance.UpdateFeedback("This is not available in family game!");
                }
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                //command any firefighter to move and/or open/close door
                //Only for fire captain

                if (!GameManager.GM.isFamilyGame)
                {
                    command();
                }
                else
                {
                    GameConsole.instance.UpdateFeedback("This is not available in family game!");
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1) && !GameManager.GM.isFamilyGame)
            {
                if (isWaitingForInput && isFiringDeckGun)
                {
                    DriverReroll(1);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && !GameManager.GM.isFamilyGame)
            {
                if (isWaitingForInput && isFiringDeckGun)
                {
                    DriverReroll(2);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) && !GameManager.GM.isFamilyGame)
            {
                if (isWaitingForInput && isFiringDeckGun)
                {
                    DriverReroll(3);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4) && !GameManager.GM.isFamilyGame)
            {
                if (isWaitingForInput && isFiringDeckGun)
                {
                    isWaitingForInput = false;
                    isFiringDeckGun = false;
                    fireDeckGun();
                }
            }
            else if (Input.GetMouseButtonDown(0) && !GameManager.GM.isFamilyGame)
            { // if left button pressed
                Space spaceClicked = null; //initialize it randomly hehe
                if (isWaitingForInput && isIdentifyingPOI)
                {
                    isWaitingForInput = false;
                    isIdentifyingPOI = false;

                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        GameObject objectClicked = hit.transform.gameObject;

                        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Board"))
                        {
                            Debug.Log("a tile was clicked");
                            spaceClicked = StateManager.instance.spaceGrid.WorldPointToSpace(objectClicked.transform.position);
                            Debug.Log("space clicked x: " + spaceClicked.indexX + "space clicked y: " + spaceClicked.indexY);
                        }

                    }
                    List<GameUnit> gameUnits = spaceClicked.getOccupants();
                    bool hasPOI = false;
                    foreach (GameUnit gu in gameUnits)
                    {
                        if (gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_POI)
                        {
                            hasPOI = true;
                            break;
                        }
                    }

                    if (hasPOI == true)
                    {
                        if (this.getAP() >= 1)
                        {
                            GameManager.FlipPOI(spaceClicked);
                            this.setAP(this.getAP() - 1);
                            FiremanUI.instance.SetAP(this.getAP());
                        }
                        Debug.Log("Not enough AP!");  //Used to show the player why he can’t perform an action in case of failure
                        GameConsole.instance.UpdateFeedback("Not enough AP!");
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("This space doesn't have a POI! Try again");
                        isWaitingForInput = true;
                        isIdentifyingPOI = true;
                    }
                }
                else if (isWaitingForInput && isClickingFirefighter)
                {
                    isWaitingForInput = false;
                    isClickingFirefighter = false;

                    //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    //RaycastHit hit;
                    //if (Physics.Raycast(ray, out hit))
                    //{
                    //    GameObject objectClicked = hit.transform.gameObject;

                    //    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Board"))
                    //    {
                    //        Debug.Log("a tile was clicked");
                    //        spaceClicked = StateManager.instance.spaceGrid.WorldPointToSpace(objectClicked.transform.position);
                    //        Debug.Log("space clicked x: " + spaceClicked.indexX + "space clicked y: " + spaceClicked.indexY);
                    //    }

                    //}

                    spaceClicked = UserInputManager.instance.getLastSpaceClicked();

                    Dictionary<int, Space> firemenSpaces = StateManager.instance.firemanCurrentSpaces;
                    foreach(KeyValuePair<int, Space> space in firemenSpaces)
                    {
                        if(spaceClicked.indexX == space.Value.indexX && spaceClicked.indexY == space.Value.indexY)
                        {
                            commandedSpace = space.Value;
                        }
                    }

                    Debug.Log("click X " + spaceClicked.indexX);
                    Debug.Log("click Y " + spaceClicked.indexY);
                    bool hasFireman = false;

                    //commandedSpace = spaceClicked;
                    if (commandedSpace != null)
                    {
                        Debug.Log("comm X " + commandedSpace.indexX);
                        Debug.Log("comm Y " + commandedSpace.indexY);

                        foreach(GameUnit gu in commandedSpace.getOccupants())
                        {
                            if(gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_FIREMAN || gu.GetType() == typeof(Fireman))
                            {
                                hasFireman = true;
                                break;
                            }
                        }

                    }

                    if (hasFireman == true)
                    {
                        GameConsole.instance.UpdateFeedback("You clicked on a fireman! You can now Move (click the arrows) and/or Open/Close Doors (click D) with that fireman");
                        //command it
                        isWaitingForInput = true;
                        isCommandingFirefighter = true;
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("This space doesn't have a Firefighter! Try again");
                        isWaitingForInput = true;
                        isClickingFirefighter = true;
                    }

                }
            }

            else if (Input.GetKeyDown(KeyCode.L))
            {
                //reveal POI
                if (!GameManager.GM.isFamilyGame)
                {
                    if (this.spec == Specialist.RescueDog)
                    {
                        revealPOI();
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("You have to be a rescue dog to do this!");
                    }
                }
                else
                {
                    GameConsole.instance.UpdateFeedback("This is not available in family game!");
                }
            }
            else if (Input.GetKeyDown(KeyCode.U))
            {
                if (!GameManager.GM.isFamilyGame)
                {
                    if (this.spec == Specialist.RescueDog)
                    {
                        squeeze();
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("You have to be a rescue dog to do this!");
                    }
                }
                else
                {
                    GameConsole.instance.UpdateFeedback("This is not available in family game!");
                }

            }

            //MOVE: ARROWS WITH DIRECTION
            //OPEN/CLOSE DOOR: "D"
            //CHOP WALL "C"
            //END TURN "Q"
            //EXTINGUISH FIRE/SMOKE "E" + Number.

            //NORTH = 0; EAST = 1; SOUTH = 2; WEST = 3
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (isWaitingForInput && isCommandingFirefighter)
                {
                    isWaitingForInput = false;
                    isCommandingFirefighter = false;

                    Fireman f = null;

                    foreach (GameUnit gu in commandedSpace.getOccupants())
                    {
                        if (gu != null && (gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_FIREMAN || gu.GetType() == typeof(Fireman)))
                        {
                            f = gu.GetComponent<Fireman>();
                        }
                    }

                    if (f != null)
                    {
                        Space curr = f.getCurrentSpace();
                        Space[] neighbors = StateManager.instance.spaceGrid.GetNeighbours(curr);
                        Space destination = neighbors[0];

                        Debug.Log("we callin ittttt 0");
                        if(destination != null) {
                            moveFirefighter(f, curr, destination);
                        }
                        else
                        {
                            GameConsole.instance.UpdateFeedback("Invalid move. Try again.");
                            isWaitingForInput = true;
                            isCommandingFirefighter = true;
                        }
                    }
                }
                else //normal move
                {
                    object[] data = { PV.ViewID, 0 };
                    PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.Move, data, sendToAllOptions, SendOptions.SendReliable);
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (isWaitingForInput && isCommandingFirefighter)
                {
                    isWaitingForInput = false;
                    isCommandingFirefighter = false;

                    Fireman f = null;

                    foreach (GameUnit gu in commandedSpace.getOccupants())
                    {
                        if (gu != null && (gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_FIREMAN || gu.GetType() == typeof(Fireman)))
                        {
                            f = gu.GetComponent<Fireman>();
                        }
                    }

                    if (f != null)
                    {
                        Space curr = f.getCurrentSpace();
                        Space[] neighbors = StateManager.instance.spaceGrid.GetNeighbours(curr);
                        Space destination = neighbors[2];

                        Debug.Log("we callin  ittttt 2");
                        if(destination != null)
                            moveFirefighter(f, curr, destination);
                        else
                        {
                            GameConsole.instance.UpdateFeedback("Invalid move. Try again.");
                            isWaitingForInput = true;
                            isCommandingFirefighter = true;
                        }
                    }
                }
                else
                {
                    object[] data = { PV.ViewID, 2 };
                    PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.Move, data, sendToAllOptions, SendOptions.SendReliable);
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (isWaitingForInput && isCommandingFirefighter)
                {
                    isWaitingForInput = false;
                    isCommandingFirefighter = false;

                    Fireman f = null;

                    foreach (GameUnit gu in commandedSpace.getOccupants())
                    {
                        if (gu != null && (gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_FIREMAN || gu.GetType() == typeof(Fireman)))
                        {
                            f = gu.GetComponent<Fireman>();
                        }
                    }

                    if (f != null)
                    {
                        Space curr = f.getCurrentSpace();
                        Space[] neighbors = StateManager.instance.spaceGrid.GetNeighbours(curr);
                        Space destination = neighbors[1];

                        Debug.Log("we callin  ittttt 1");
                        if(destination != null)
                            moveFirefighter(f, curr, destination);
                        else
                        {
                            GameConsole.instance.UpdateFeedback("Invalid move. Try again.");
                            isWaitingForInput = true;
                            isCommandingFirefighter = true;
                        }
                    }
                }
                else
                {
                    object[] data = { PV.ViewID, 1 };
                    PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.Move, data, sendToAllOptions, SendOptions.SendReliable);
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (isWaitingForInput && isCommandingFirefighter)
                {
                    isWaitingForInput = false;
                    isCommandingFirefighter = false;
                    Fireman f = null;

                    foreach(GameUnit gu in commandedSpace.getOccupants())
                    {
                        if (gu != null && (gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_FIREMAN || gu.GetType() == typeof(Fireman) ) )
                        {
                            f = gu.GetComponent<Fireman>();
                        }
                    }

                    if (f != null)
                    {
                        Space curr = f.getCurrentSpace();
                        Space[] neighbors = StateManager.instance.spaceGrid.GetNeighbours(curr);
                        Space destination = neighbors[3];

                        Debug.Log("we callin  ittttt 3");
                        if(destination != null)
                            f.moveFirefighter(curr, destination,0,true);
                        else
                        {
                            GameConsole.instance.UpdateFeedback("Invalid move. Try again.");
                            isWaitingForInput = true;
                            isCommandingFirefighter = true;
                        }
                    }
                
                }
                else
                {
                    object[] data = { PV.ViewID, 3 };
                    PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.Move, data, sendToAllOptions, SendOptions.SendReliable);
                }
            }
            else if (Input.GetKeyDown(KeyCode.D)) //open/close door
            {
                if (isWaitingForInput && isCommandingFirefighter)
                {
                    isWaitingForInput = false;
                    isCommandingFirefighter = false;

                    int commandedSpaceX = commandedSpace.indexX;
                    Debug.Log("command X" + commandedSpaceX);
                    int commandedSpaceY = commandedSpace.indexY;
                    Debug.Log("command Y" + commandedSpaceY);
                    object[] data = { commandedSpaceX, commandedSpaceY };

                    int doorDir = 4;//forbidden value
                    Door[] doors = commandedSpace.getDoors();

                    for (int i = 0; i < 4; i++)
                    {
                        if (doors[i] != null)
                        {
                            doorDir = i;
                            Debug.Log("door dir" + doorDir);
                        }
                    }
                    if (doorDir >= 0 && doorDir <= 3)
                    {
                        Door door = doors[doorDir];

                        if (door.getDoorStatus() == DoorStatus.Open)
                        {
                            if (this.getAP() >= 1)
                            {
                                decrementAP(1);
                                FiremanUI.instance.SetAP(this.getAP());
                                PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.Door, data, sendToAllOptions, SendOptions.SendReliable);
                                GameConsole.instance.UpdateFeedback("Door closed successfully!");
                            }
                            else
                            {
                                GameConsole.instance.UpdateFeedback("Insufficient AP");
                                return;
                            }

                        }
                        else if (door.getDoorStatus() == DoorStatus.Closed)
                        {
                            if (this.getAP() >= 1)
                            {
                                decrementAP(1);
                                FiremanUI.instance.SetAP(this.getAP());
                                PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.Door, data, sendToAllOptions, SendOptions.SendReliable);
                                GameConsole.instance.UpdateFeedback("Door opened successfully!");
                            }
                            else
                            {
                                GameConsole.instance.UpdateFeedback("Insufficient AP");
                                return;
                            }
                        }

                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("there are no doors near the Fireman!");
                    }
                }

                else
                {
                    if(this.spec == Specialist.RescueDog)
                    {
                        GameConsole.instance.UpdateFeedback("Rescue dog cannot do this DAWG!");
                    }
                    else
                    {
                        int currentSpaceX = this.getCurrentSpace().indexX;
                        int currentSpaceY = this.getCurrentSpace().indexY;
                        object[] data = { currentSpaceX, currentSpaceY };

                        int doorDir = 4;//forbidden value
                        Door[] doors = this.getCurrentSpace().getDoors();

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
                                if (this.getAP() >= 1)
                                {
                                    decrementAP(1);
                                    FiremanUI.instance.SetAP(this.getAP());
                                    PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.Door, data, sendToAllOptions, SendOptions.SendReliable);
                                    GameConsole.instance.UpdateFeedback("Door closed successfully!");
                                }
                                else
                                {
                                    GameConsole.instance.UpdateFeedback("Insufficient AP");
                                    return;
                                }

                            }
                            else if (door.getDoorStatus() == DoorStatus.Closed)
                            {
                                if (this.getAP() >= 1)
                                {
                                    decrementAP(1);
                                    FiremanUI.instance.SetAP(this.getAP());
                                    PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.Door, data, sendToAllOptions, SendOptions.SendReliable);
                                    GameConsole.instance.UpdateFeedback("Door opened successfully!");
                                }
                                else
                                {
                                    GameConsole.instance.UpdateFeedback("Insufficient AP");
                                    return;
                                }
                            }
                        }
                        else
                        {
                            GameConsole.instance.UpdateFeedback("there are no doors near the space you're on!");
                        }
                    }
                }

            }

            else if (Input.GetKeyDown(KeyCode.E))
            {
                if (this.spec == Specialist.RescueDog)
                {
                    GameConsole.instance.UpdateFeedback("Rescue dog cannot do this DAWG!");
                    return;
                }
                
                Debug.Log("Extinguish Fire Detected");
                extinguishFire();
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                if (this.spec == Specialist.RescueDog)
                {
                    GameConsole.instance.UpdateFeedback("Rescue dog cannot do this DAWG!");
                    return;
                }

                Debug.Log("Chop Wall Detected");
                chopWall();
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                if (!GameManager.GM.isFamilyGame)
                {
                    Debug.Log("Crew Change Detected");
                    changeCrew();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                if (isWaitingForInput && isExtinguishingFire)
                {
                    Debug.Log("Input 0 Received");
                    isWaitingForInput = false;
                    isExtinguishingFire = false;
                    if (validInputOptions.Contains(0))
                    {
                        Debug.Log("This is a valid extinguish option.");
                        validInputOptions = new ArrayList();
                        Space targetSpace = StateManager.instance.spaceGrid.getNeighborInDirection(this.currentSpace, 0);
                        int numAP = this.getAP();
                        if (targetSpace.getSpaceStatus() == SpaceStatus.Smoke)
                        {
                            this.decrementRemoveSmokeAP();
                            FiremanUI.instance.SetAP(this.getAP());
                            sendSmokeMarkerExtinguishEvent(targetSpace);
                        }
                        else
                        {
                            if ((!isDoubleSpec && numAP < 2) || (isDoubleSpec && numAP < 4))
                            {
                                GameConsole.instance.UpdateFeedback("Turning Fire To Smoke...");
                                this.decrementFireToSmokeAP();
                                FiremanUI.instance.SetAP(this.getAP());
                                sendTurnFireMarkerToSmokeEvent(targetSpace);
                            }
                            else
                            {
                                GameConsole.instance.UpdateFeedback("Press Y to extinguish the Fire altogether or N to turn the Fire to smoke");

                                isWaitingForInput = true;
                                isSelectingExtinguishOption = true;
                                locationArgument = targetSpace;
                            }
                        }

                    }
                    else
                    {
                        string oldMessage = GameConsole.instance.FeedbackText.text;
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isExtinguishingFire = true;
                    }
                }
                else if (isWaitingForInput && isChoppingWall)
                {
                    Debug.Log("Input 0 Received");
                    isWaitingForInput = false;
                    isChoppingWall = false;
                    if (validInputOptions.Contains(0))
                    {
                        Debug.Log("This is a valid chop wall option.");
                        GameConsole.instance.UpdateFeedback("Chopping wall.");
                        validInputOptions = new ArrayList();
                        Space targetSpace = this.getCurrentSpace();
                        this.decrementChopWallAP();
                        FiremanUI.instance.SetAP(this.getAP());
                        sendChopWallEvent(targetSpace, 0);
                    }
                    else
                    {
                        string oldMessage = GameConsole.instance.FeedbackText.text;
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isChoppingWall = true;
                    }
                }
                else if (isWaitingForInput && isChangingCrew)
                {
                    Debug.Log("Input 0 Received");
                    string oldMessage = GameConsole.instance.FeedbackText.text;
                    isWaitingForInput = false;
                    isChangingCrew = false;
                    if (GameManager.GM.freeSpecialistIndex[0] != 0) //make sure that specialist is in there 
                    {
                        Debug.Log("changing specialist to paramedic (0)");
                        Specialist oldSpec = this.spec;
                        this.spec = Specialist.Paramedic;
                        FiremanUI.instance.SetSpecialist(Specialist.Paramedic);
                        GameManager.GM.freeSpecialistIndex[0] = 0;
                        newSpecAP();
                        this.setAP(this.getAP() - 2);
                        FiremanUI.instance.SetAP(this.getAP());
                        GameConsole.instance.UpdateFeedback("Updated Specialist to Paramedic.");


                        if (oldSpec == Specialist.Paramedic)
                        {
                            isWaitingForInput = true;
                            isChangingCrew = true;
                            GameConsole.instance.UpdateFeedback("You're already a paramedic. \n" + oldMessage);
                        }
                        else if (oldSpec == Specialist.FireCaptain)
                        {
                            GameManager.GM.freeSpecialistIndex[1] = 1;
                        }
                        else if (oldSpec == Specialist.ImagingTechnician)
                        {
                            GameManager.GM.freeSpecialistIndex[2] = 1;
                        }
                        else if (oldSpec == Specialist.CAFSFirefighter)
                        {
                            GameManager.GM.freeSpecialistIndex[3] = 1;
                        }
                        else if (oldSpec == Specialist.HazmatTechinician)
                        {
                            GameManager.GM.freeSpecialistIndex[4] = 1;
                        }
                        else if (oldSpec == Specialist.Generalist)
                        {
                            GameManager.GM.freeSpecialistIndex[5] = 1;
                        }
                        else if (oldSpec == Specialist.RescueSpecialist)
                        {
                            GameManager.GM.freeSpecialistIndex[6] = 1;
                        }
                        else if (oldSpec == Specialist.DriverOperator)
                        {
                            GameManager.GM.freeSpecialistIndex[7] = 1;
                        }
                        else if (oldSpec == Specialist.Veteran)
                        {
                            GameManager.GM.freeSpecialistIndex[9] = 1;
                        }
                        else if (oldSpec == Specialist.RescueDog)
                        {
                            GameManager.GM.freeSpecialistIndex[8] = 1;
                        }
                        else if (oldSpec == Specialist.Veteran)
                        {
                            GameManager.GM.freeSpecialistIndex[9] = 1;
                        }
                        sendChangeCrewEvent(GameManager.GM.freeSpecialistIndex);


                        Space curr = this.getCurrentSpace();
                        Space destination = getEngineSpaces();
                        moveFirefighter(curr, destination, 0, true);



                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isChangingCrew = true;
                    }

                }

                if (isWaitingForInput && isRevealingPOI)
                {
                    isWaitingForInput = false;
                    isRevealingPOI = false;

                    if (validInputOptions.Contains(0))
                    {
                        Debug.Log("This is a valid reveal option.");
                        validInputOptions = new ArrayList();
                        Space targetSpace = StateManager.instance.spaceGrid.getNeighborInDirection(this.currentSpace, 0);
                        List<GameUnit> gu = targetSpace.getOccupants();
                        bool spaceHasPOI = false;
                        foreach (GameUnit g in gu)
                        {
                            if (g.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_POI)
                            {
                                spaceHasPOI = true;
                                break;
                            }
                        }

                        if (spaceHasPOI == true)
                        {
                            GameManager.FlipPOI(targetSpace);
                        }
                        else
                        {
                            GameConsole.instance.UpdateFeedback("This space doesn't have a POI! Try again");
                            isWaitingForInput = true;
                            isRevealingPOI = true;
                        }
                    }
                    else
                    {
                        string oldMessage = GameConsole.instance.FeedbackText.text;
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isRevealingPOI = true;
                    }
                }
                else if(isWaitingForInput && isSqueezing)
                {
                    isWaitingForInput = false;
                    isSqueezing = false;

                    if (validInputOptions.Contains(0))
                    {
                        Debug.Log("This is a valid squeeze through wall option.");
                        GameConsole.instance.UpdateFeedback("Squeezing through wall.");
                        validInputOptions = new ArrayList();
                        Space curr = this.getCurrentSpace();
                        Space destination = StateManager.instance.spaceGrid.getNeighborInDirection(curr, 0);
                        moveFirefighter(curr, destination, 1, true);
                        //object[] data = { PV.ViewID, 0 };
                        //PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.MoveRescueDog, data, sendToAllOptions, SendOptions.SendReliable);
                    }
                    else
                    {
                        string oldMessage = GameConsole.instance.FeedbackText.text;
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isSqueezing = true;
                    }
                }

            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (isWaitingForInput && isExtinguishingFire)
                {
                    Debug.Log("Input 1 Received");
                    isWaitingForInput = false;
                    isExtinguishingFire = false;
                    if (validInputOptions.Contains(1))
                    {
                        Debug.Log("This is a valid extinguish option.");
                        validInputOptions = new ArrayList();
                        Space targetSpace = StateManager.instance.spaceGrid.getNeighborInDirection(this.currentSpace, 1);
                        int numAP = this.getAP();
                        if (targetSpace.getSpaceStatus() == SpaceStatus.Smoke)
                        {
                            this.decrementRemoveSmokeAP();
                            FiremanUI.instance.SetAP(this.getAP());
                            sendSmokeMarkerExtinguishEvent(targetSpace);
                        }
                        else
                        {
                            if ((!isDoubleSpec && numAP < 2) || (isDoubleSpec && numAP < 4))
                            {
                                GameConsole.instance.UpdateFeedback("Turning Fire To Smoke...");
                                this.decrementFireToSmokeAP();
                                FiremanUI.instance.SetAP(this.getAP());
                                sendTurnFireMarkerToSmokeEvent(targetSpace);
                            }
                            else
                            {
                                GameConsole.instance.UpdateFeedback("Press Y to extinguish the Fire altogether or N to turn the Fire to smoke");

                                isWaitingForInput = true;
                                isSelectingExtinguishOption = true;
                                locationArgument = targetSpace;
                            }
                        }

                    }
                    else
                    {
                        string oldMessage = GameConsole.instance.FeedbackText.text;
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isExtinguishingFire = true;
                    }
                }
                else if (isWaitingForInput && isChoppingWall)
                {
                    Debug.Log("Input 1 Received");
                    isWaitingForInput = false;
                    isChoppingWall = false;
                    if (validInputOptions.Contains(1))
                    {
                        Debug.Log("This is a valid chop wall option.");
                        GameConsole.instance.UpdateFeedback("Chopping wall.");
                        validInputOptions = new ArrayList();
                        Space targetSpace = this.getCurrentSpace();
                        this.decrementChopWallAP();
                        FiremanUI.instance.SetAP(this.getAP());
                        sendChopWallEvent(targetSpace, 1);
                    }
                    else
                    {
                        string oldMessage = GameConsole.instance.FeedbackText.text;
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isChoppingWall = true;
                    }
                }
                else if (isWaitingForInput && isRidingVehicle)
                {
                    Debug.Log("Input 1 Received");
                    isWaitingForInput = false;
                    isRidingVehicle = false;
                    sendRideAmbulanceEvent(1);
                }
                else if (isWaitingForInput && isCallingAmbulance)
                {
                    Debug.Log("Input 1 Received");
                    isWaitingForInput = false;
                    isCallingAmbulance = false;
                    decrementAP(2);
                    FiremanUI.instance.SetAP(this.AP);
                    sendDriveAmbulanceEvent(1);
                    GameConsole.instance.UpdateFeedback("You have moved with the ambulance successfully");
                }
                else if (isWaitingForInput && isCallingEngine)
                {
                    Debug.Log("Input 1 Received");
                    isWaitingForInput = false;
                    isCallingEngine = false;
                    decrementAP(2);
                    FiremanUI.instance.SetAP(this.AP);
                    sendDriveEngineEvent(1);
                    GameConsole.instance.UpdateFeedback("You have moved with the engine successfully");
                }
                else if (isWaitingForInput && isChangingCrew)
                {
                    Debug.Log("Input 1 Received");
                    string oldMessage = GameConsole.instance.FeedbackText.text;
                    isWaitingForInput = false;
                    isChangingCrew = false;
                    if (GameManager.GM.freeSpecialistIndex[1] != 0) //make sure that specialist is in there 
                    {
                        Debug.Log("changing specialist to fire captain (1)");
                        Specialist oldSpec = this.spec;
                        this.spec = Specialist.FireCaptain;
                        FiremanUI.instance.SetSpecialist(Specialist.FireCaptain);
                        GameManager.GM.freeSpecialistIndex[1] = 0;
                        newSpecAP();
                        this.setAP(this.getAP() - 2);
                        FiremanUI.instance.SetAP(this.getAP());
                        GameConsole.instance.UpdateFeedback("Updated Specialist to Fire Captain.");


                        if (oldSpec == Specialist.Paramedic)
                        {
                            GameManager.GM.freeSpecialistIndex[0] = 1;
                        }
                        else if (oldSpec == Specialist.FireCaptain)
                        {
                            isWaitingForInput = true;
                            isChangingCrew = true;
                            GameConsole.instance.UpdateFeedback("You're already a fire captain. \n" + oldMessage);
                        }
                        else if (oldSpec == Specialist.ImagingTechnician)
                        {
                            GameManager.GM.freeSpecialistIndex[2] = 1;
                        }
                        else if (oldSpec == Specialist.CAFSFirefighter)
                        {
                            GameManager.GM.freeSpecialistIndex[3] = 1;
                        }
                        else if (oldSpec == Specialist.HazmatTechinician)
                        {
                            GameManager.GM.freeSpecialistIndex[4] = 1;
                        }
                        else if (oldSpec == Specialist.Generalist)
                        {
                            GameManager.GM.freeSpecialistIndex[5] = 1;
                        }
                        else if (oldSpec == Specialist.RescueSpecialist)
                        {
                            GameManager.GM.freeSpecialistIndex[6] = 1;
                        }
                        else if (oldSpec == Specialist.DriverOperator)
                        {
                            GameManager.GM.freeSpecialistIndex[7] = 1;
                        }
                        else if (oldSpec == Specialist.RescueDog)
                        {
                            GameManager.GM.freeSpecialistIndex[8] = 1;
                        }
                        else if (oldSpec == Specialist.Veteran)
                        {
                            GameManager.GM.freeSpecialistIndex[9] = 1;
                        }

                        sendChangeCrewEvent(GameManager.GM.freeSpecialistIndex);


                        Space curr = this.getCurrentSpace();
                        Space destination = getEngineSpaces();
                        moveFirefighter(curr, destination, 0, true);
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isChangingCrew = true;
                    }
                }
                if (isWaitingForInput && isRevealingPOI)
                {
                    isWaitingForInput = false;
                    isRevealingPOI = false;

                    if (validInputOptions.Contains(1))
                    {
                        Debug.Log("This is a valid reveal option.");
                        validInputOptions = new ArrayList();
                        Space targetSpace = StateManager.instance.spaceGrid.getNeighborInDirection(this.currentSpace, 1);
                        List<GameUnit> gu = targetSpace.getOccupants();
                        bool spaceHasPOI = false;
                        foreach (GameUnit g in gu)
                        {
                            if (g.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_POI)
                            {
                                spaceHasPOI = true;
                                break;
                            }
                        }

                        if (spaceHasPOI == true)
                        {
                            GameManager.FlipPOI(targetSpace);
                        }
                        else
                        {
                            GameConsole.instance.UpdateFeedback("This space doesn't have a POI! Try again");
                            isWaitingForInput = true;
                            isRevealingPOI = true;
                        }
                    }
                    else
                    {
                        string oldMessage = GameConsole.instance.FeedbackText.text;
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isRevealingPOI = true;
                    }
                }
                else if (isWaitingForInput && isSqueezing)
                {
                    isWaitingForInput = false;
                    isSqueezing = false;

                    if (validInputOptions.Contains(1))
                    {
                        Debug.Log("This is a valid squeeze through wall option.");
                        GameConsole.instance.UpdateFeedback("Squeezing through wall.");
                        validInputOptions = new ArrayList();
                        Space curr = this.getCurrentSpace();
                        Space destination = StateManager.instance.spaceGrid.getNeighborInDirection(curr, 1);
                        moveFirefighter(curr, destination, 1, true);
                        //object[] data = { PV.ViewID, 1 };
                        //PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.MoveRescueDog, data, sendToAllOptions, SendOptions.SendReliable);
                    }
                    else
                    {
                        string oldMessage = GameConsole.instance.FeedbackText.text;
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isSqueezing = true;
                    }
                }
            }

            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (isWaitingForInput && isExtinguishingFire)
                {
                    Debug.Log("Input 2 Received");
                    isWaitingForInput = false;
                    isExtinguishingFire = false;
                    if (validInputOptions.Contains(2))
                    {
                        Debug.Log("This is a valid extinguish option.");
                        validInputOptions = new ArrayList();
                        Space targetSpace = StateManager.instance.spaceGrid.getNeighborInDirection(this.currentSpace, 2);
                        int numAP = this.getAP();
                        if (targetSpace.getSpaceStatus() == SpaceStatus.Smoke)
                        {
                            this.decrementRemoveSmokeAP();
                            FiremanUI.instance.SetAP(this.getAP());
                            sendSmokeMarkerExtinguishEvent(targetSpace);
                        }
                        else
                        {
                            if ((!isDoubleSpec && numAP < 2) || (isDoubleSpec && numAP < 4))
                            {
                                GameConsole.instance.UpdateFeedback("Turning Fire To Smoke...");
                                this.decrementFireToSmokeAP();
                                FiremanUI.instance.SetAP(this.getAP());
                                sendTurnFireMarkerToSmokeEvent(targetSpace);
                            }
                            else
                            {
                                GameConsole.instance.UpdateFeedback("Press Y to extinguish the Fire altogether or N to turn the Fire to smoke");

                                isWaitingForInput = true;
                                isSelectingExtinguishOption = true;
                                locationArgument = targetSpace;
                            }
                        }

                    }
                    else
                    {
                        string oldMessage = GameConsole.instance.FeedbackText.text;
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isExtinguishingFire = true;
                    }
                }
                else if (isWaitingForInput && isChoppingWall)
                {
                    Debug.Log("Input 2 Received");
                    isWaitingForInput = false;
                    isChoppingWall = false;
                    if (validInputOptions.Contains(2))
                    {
                        Debug.Log("This is a valid chop wall option.");
                        GameConsole.instance.UpdateFeedback("Chopping wall.");
                        validInputOptions = new ArrayList();
                        Space targetSpace = this.getCurrentSpace();
                        this.decrementChopWallAP();
                        FiremanUI.instance.SetAP(this.getAP());
                        sendChopWallEvent(targetSpace, 2);
                    }
                    else
                    {
                        string oldMessage = GameConsole.instance.FeedbackText.text;
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isChoppingWall = true;
                    }
                }
               else if (isWaitingForInput && isRidingVehicle)
                {
                    Debug.Log("Input 2 Received");
                    isWaitingForInput = false;
                    isRidingVehicle = false;
                    sendRideEngineEvent(2);
                }
                else if (isWaitingForInput && isCallingAmbulance)
                {
                    Debug.Log("Input 2 Received");
                    isWaitingForInput = false;
                    isCallingAmbulance = false;
                    decrementAP(4);
                    FiremanUI.instance.SetAP(this.AP);
                    sendDriveAmbulanceEvent(2);
                    GameConsole.instance.UpdateFeedback("You have moved with the ambulance successfully");
                }
                else if (isWaitingForInput && isCallingEngine)
                {
                    Debug.Log("Input 1 Received");
                    isWaitingForInput = false;
                    isCallingEngine = false;
                    decrementAP(2);
                    FiremanUI.instance.SetAP(this.AP);
                    sendDriveEngineEvent(2);
                    GameConsole.instance.UpdateFeedback("You have moved with the engine successfully");
                }
                else if (isWaitingForInput && isChangingCrew)
                {
                    Debug.Log("Input 2 Received");
                    string oldMessage = GameConsole.instance.FeedbackText.text;
                    isWaitingForInput = false;
                    isChangingCrew = false;
                    if (GameManager.GM.freeSpecialistIndex[2] != 0) //make sure that specialist is in there 
                    {
                        Debug.Log("changing specialist to imaging technician (2)");
                        Specialist oldSpec = this.spec;
                        this.spec = Specialist.ImagingTechnician;
                        FiremanUI.instance.SetSpecialist(Specialist.ImagingTechnician);
                        GameManager.GM.freeSpecialistIndex[2] = 0;
                        newSpecAP();
                        this.setAP(this.getAP() - 2);
                        FiremanUI.instance.SetAP(this.getAP());
                        GameConsole.instance.UpdateFeedback("Updated Specialist to Imaging Technician.");


                        if (oldSpec == Specialist.Paramedic)
                        {
                            GameManager.GM.freeSpecialistIndex[0] = 1;
                        }
                        else if (oldSpec == Specialist.FireCaptain)
                        {
                            GameManager.GM.freeSpecialistIndex[1] = 1;
                        }
                        else if (oldSpec == Specialist.ImagingTechnician)
                        {
                            isWaitingForInput = true;
                            isChangingCrew = true;
                            GameConsole.instance.UpdateFeedback("You're already an imaging technician. \n" + oldMessage);
                        }
                        else if (oldSpec == Specialist.CAFSFirefighter)
                        {
                            GameManager.GM.freeSpecialistIndex[3] = 1;
                        }
                        else if (oldSpec == Specialist.HazmatTechinician)
                        {
                            GameManager.GM.freeSpecialistIndex[4] = 1;
                        }
                        else if (oldSpec == Specialist.Generalist)
                        {
                            GameManager.GM.freeSpecialistIndex[5] = 1;
                        }
                        else if (oldSpec == Specialist.RescueSpecialist)
                        {
                            GameManager.GM.freeSpecialistIndex[6] = 1;
                        }
                        else if (oldSpec == Specialist.DriverOperator)
                        {
                            GameManager.GM.freeSpecialistIndex[7] = 1;
                        }
                        else if (oldSpec == Specialist.RescueDog)
                        {
                            GameManager.GM.freeSpecialistIndex[8] = 1;
                        }
                        else if (oldSpec == Specialist.Veteran)
                        {
                            GameManager.GM.freeSpecialistIndex[9] = 1;
                        }

                        sendChangeCrewEvent(GameManager.GM.freeSpecialistIndex);

                        Space curr = this.getCurrentSpace();
                        Space destination = getEngineSpaces();
                        moveFirefighter(curr, destination, 0, true);
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isChangingCrew = true;
                    }
                }
                if (isWaitingForInput && isRevealingPOI)
                {
                    isWaitingForInput = false;
                    isRevealingPOI = false;

                    if (validInputOptions.Contains(2))
                    {
                        Debug.Log("This is a valid reveal option.");
                        validInputOptions = new ArrayList();
                        Space targetSpace = StateManager.instance.spaceGrid.getNeighborInDirection(this.currentSpace, 2);
                        List<GameUnit> gu = targetSpace.getOccupants();
                        bool spaceHasPOI = false;
                        foreach (GameUnit g in gu)
                        {
                            if (g.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_POI)
                            {
                                spaceHasPOI = true;
                                break;
                            }
                        }

                        if (spaceHasPOI == true)
                        {
                            GameManager.FlipPOI(targetSpace);
                        }
                        else
                        {
                            GameConsole.instance.UpdateFeedback("This space doesn't have a POI! Try again");
                            isWaitingForInput = true;
                            isRevealingPOI = true;
                        }
                    }
                    else
                    {
                        string oldMessage = GameConsole.instance.FeedbackText.text;
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isRevealingPOI = true;
                    }
                }
                else if (isWaitingForInput && isSqueezing)
                {
                    isWaitingForInput = false;
                    isSqueezing = false;

                    if (validInputOptions.Contains(2))
                    {
                        Debug.Log("This is a valid squeeze through wall option.");
                        GameConsole.instance.UpdateFeedback("Squeezing through wall.");
                        validInputOptions = new ArrayList();
                        Space curr = this.getCurrentSpace();
                        Space destination = StateManager.instance.spaceGrid.getNeighborInDirection(curr, 2);
                        moveFirefighter(curr, destination, 1, true);
                        //object[] data = { PV.ViewID, 2 };
                        //PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.MoveRescueDog, data, sendToAllOptions, SendOptions.SendReliable);
                    }
                    else
                    {
                        string oldMessage = GameConsole.instance.FeedbackText.text;
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isSqueezing = true;
                    }
                }

            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (isWaitingForInput && isExtinguishingFire)
                {
                    Debug.Log("Input 3 Received");
                    isWaitingForInput = false;
                    isExtinguishingFire = false;
                    if (validInputOptions.Contains(3))
                    {
                        Debug.Log("This is a valid extinguish option.");
                        validInputOptions = new ArrayList();
                        Space targetSpace = StateManager.instance.spaceGrid.getNeighborInDirection(this.currentSpace, 3);
                        int numAP = this.getAP();
                        if (targetSpace.getSpaceStatus() == SpaceStatus.Smoke)
                        {
                            this.decrementRemoveSmokeAP();
                            FiremanUI.instance.SetAP(this.getAP());
                            sendSmokeMarkerExtinguishEvent(targetSpace);
                        }
                        else
                        {
                            if ((!isDoubleSpec && numAP < 2) || (isDoubleSpec && numAP < 4))
                            {
                                GameConsole.instance.UpdateFeedback("Turning Fire To Smoke...");
                                this.decrementFireToSmokeAP();
                                FiremanUI.instance.SetAP(this.getAP());
                                sendTurnFireMarkerToSmokeEvent(targetSpace);
                            }
                            else
                            {
                                GameConsole.instance.UpdateFeedback("Press Y to extinguish the Fire altogether or N to turn the Fire to smoke");

                                isWaitingForInput = true;
                                isSelectingExtinguishOption = true;
                                locationArgument = targetSpace;
                            }
                        }

                    }
                    else
                    {
                        string oldMessage = GameConsole.instance.FeedbackText.text;
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isExtinguishingFire = true;
                    }
                }
                else if (isWaitingForInput && isChoppingWall)
                {
                    Debug.Log("Input 3 Received");
                    isWaitingForInput = false;
                    isChoppingWall = false;
                    if (validInputOptions.Contains(3))
                    {
                        Debug.Log("This is a valid chop wall option.");
                        GameConsole.instance.UpdateFeedback("Chopping wall.");
                        validInputOptions = new ArrayList();
                        Space targetSpace = this.getCurrentSpace();
                        this.decrementChopWallAP();
                        FiremanUI.instance.SetAP(this.getAP());
                        sendChopWallEvent(targetSpace, 3);
                    }
                    else
                    {
                        string oldMessage = GameConsole.instance.FeedbackText.text;
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isChoppingWall = true;
                    }

                }
                else if (isWaitingForInput && isCallingAmbulance)
                {
                    Debug.Log("Input 3 Received");
                    isWaitingForInput = false;
                    isCallingAmbulance = false;
                    decrementAP(2);
                    FiremanUI.instance.SetAP(this.AP);
                    sendDriveAmbulanceEvent(3);
                    GameConsole.instance.UpdateFeedback("You have moved with the ambulance successfully");
                }
                else if (isWaitingForInput && isCallingEngine)
                {
                    Debug.Log("Input 1 Received");
                    isWaitingForInput = false;
                    isCallingEngine = false;
                    decrementAP(2);
                    FiremanUI.instance.SetAP(this.AP);
                    sendDriveEngineEvent(3);
                    GameConsole.instance.UpdateFeedback("You have moved with the engine successfully");
                }
                else if (isWaitingForInput && isChangingCrew)
                {
                    Debug.Log("Input 3 Received");
                    string oldMessage = GameConsole.instance.FeedbackText.text;
                    isWaitingForInput = false;
                    isChangingCrew = false;
                    if (GameManager.GM.freeSpecialistIndex[3]!=0) //make sure that specialist is in there 
                    {
                        Debug.Log("changing specialist to CAFS Firefighter (3)");
                        Specialist oldSpec = this.spec;
                        this.spec = Specialist.CAFSFirefighter;
                        FiremanUI.instance.SetSpecialist(Specialist.CAFSFirefighter);
                        GameManager.GM.freeSpecialistIndex[3] = 0;
                        newSpecAP();
                        this.setAP(this.getAP() - 2);
                        FiremanUI.instance.SetAP(this.getAP());
                        GameConsole.instance.UpdateFeedback("Updated Specialist to CAFS Firefighter.");


                        if (oldSpec == Specialist.Paramedic)
                        {
                            GameManager.GM.freeSpecialistIndex[0] = 1;
                        }
                        else if (oldSpec == Specialist.FireCaptain)
                        {
                            GameManager.GM.freeSpecialistIndex[1] = 1;
                        }
                        else if (oldSpec == Specialist.ImagingTechnician)
                        {
                            GameManager.GM.freeSpecialistIndex[2] = 1;
                        }
                        else if (oldSpec == Specialist.CAFSFirefighter)
                        {
                            isWaitingForInput = true;
                            isChangingCrew = true;
                            GameConsole.instance.UpdateFeedback("You're already a CAFS Firefighter. \n" + oldMessage);
                        }
                        else if (oldSpec == Specialist.HazmatTechinician)
                        {
                            GameManager.GM.freeSpecialistIndex[4] = 1;
                        }
                        else if (oldSpec == Specialist.Generalist)
                        {
                            GameManager.GM.freeSpecialistIndex[5] = 1;
                        }
                        else if (oldSpec == Specialist.RescueSpecialist)
                        {
                            GameManager.GM.freeSpecialistIndex[6] = 1;
                        }
                        else if (oldSpec == Specialist.DriverOperator)
                        {
                            GameManager.GM.freeSpecialistIndex[7] = 1;
                        }
                        else if (oldSpec == Specialist.RescueDog)
                        {
                            GameManager.GM.freeSpecialistIndex[8] = 1;
                        }
                        else if (oldSpec == Specialist.Veteran)
                        {
                            GameManager.GM.freeSpecialistIndex[9] = 1;
                        }

                        sendChangeCrewEvent(GameManager.GM.freeSpecialistIndex);

                        Space curr = this.getCurrentSpace();
                        Space destination = getEngineSpaces();
                        moveFirefighter(curr, destination, 0, true);
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isChangingCrew = true;
                    }
                }
                if (isWaitingForInput && isRevealingPOI)
                {
                    isWaitingForInput = false;
                    isRevealingPOI = false;

                    if (validInputOptions.Contains(3))
                    {
                        Debug.Log("This is a valid reveal option.");
                        validInputOptions = new ArrayList();
                        Space targetSpace = StateManager.instance.spaceGrid.getNeighborInDirection(this.currentSpace, 3);
                        List<GameUnit> gu = targetSpace.getOccupants();
                        bool spaceHasPOI = false;
                        foreach (GameUnit g in gu)
                        {
                            if (g.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_POI)
                            {
                                spaceHasPOI = true;
                                break;
                            }
                        }

                        if (spaceHasPOI == true)
                        {
                            GameManager.FlipPOI(targetSpace);
                        }
                        else
                        {
                            GameConsole.instance.UpdateFeedback("This space doesn't have a POI! Try again");
                            isWaitingForInput = true;
                            isRevealingPOI = true;
                        }
                    }
                    else
                    {
                        string oldMessage = GameConsole.instance.FeedbackText.text;
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isRevealingPOI = true;
                    }
                }
                else if (isWaitingForInput && isSqueezing)
                {
                    isWaitingForInput = false;
                    isSqueezing = false;

                    if (validInputOptions.Contains(3))
                    {
                        Debug.Log("This is a valid squeeze through wall option.");
                        GameConsole.instance.UpdateFeedback("Squeezing through wall.");
                        validInputOptions = new ArrayList();
                        Space curr = this.getCurrentSpace();
                        Space destination = StateManager.instance.spaceGrid.getNeighborInDirection(curr, 3);
                        moveFirefighter(curr, destination, 1, true);
                        //object[] data = { PV.ViewID, 3 };
                        //PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.MoveRescueDog, data, sendToAllOptions, SendOptions.SendReliable);
                    }
                    else
                    {
                        string oldMessage = GameConsole.instance.FeedbackText.text;
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isSqueezing = true;
                    }
                }

            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (isWaitingForInput && isExtinguishingFire)
                {
                    Debug.Log("Input 4 Received");
                    isWaitingForInput = false;
                    isExtinguishingFire = false;
                    if (validInputOptions.Contains(4))
                    {
                        Debug.Log("This is a valid extinguish option.");
                        validInputOptions = new ArrayList();
                        Space targetSpace = StateManager.instance.spaceGrid.getNeighborInDirection(this.currentSpace, 4);
                        int numAP = this.getAP();
                        if (targetSpace.getSpaceStatus() == SpaceStatus.Smoke)
                        {
                            this.decrementRemoveSmokeAP();
                            FiremanUI.instance.SetAP(this.getAP());
                            sendSmokeMarkerExtinguishEvent(targetSpace);
                        }
                        else
                        {
                            if ((!isDoubleSpec && numAP < 2) || (isDoubleSpec && numAP < 4))
                            {
                                GameConsole.instance.UpdateFeedback("Turning Fire To Smoke...");
                                this.decrementFireToSmokeAP();
                                FiremanUI.instance.SetAP(this.getAP());
                                sendTurnFireMarkerToSmokeEvent(targetSpace);
                            }
                            else
                            {
                                GameConsole.instance.UpdateFeedback("Press Y to extinguish the Fire altogether or N to turn the Fire to smoke");

                                isWaitingForInput = true;
                                isSelectingExtinguishOption = true;
                                locationArgument = targetSpace;
                            }
                        }

                    }
                    else
                    {
                        string oldMessage = GameConsole.instance.FeedbackText.text;
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isExtinguishingFire = true;
                    }
                }
                else if (isWaitingForInput && isChangingCrew)
                {
                    Debug.Log("Input 4 Received");
                    string oldMessage = GameConsole.instance.FeedbackText.text;
                    isWaitingForInput = false;
                    isChangingCrew = false;
                    if (GameManager.GM.freeSpecialistIndex[4]!=0) //make sure that specialist is in there 
                    {
                        Debug.Log("changing specialist to Hazmat Technician (4)");
                        Specialist oldSpec = this.spec;
                        this.spec = Specialist.HazmatTechinician;
                        FiremanUI.instance.SetSpecialist(Specialist.HazmatTechinician);
                        GameManager.GM.freeSpecialistIndex[4] = 0;
                        newSpecAP();
                        this.setAP(this.getAP() - 2);
                        FiremanUI.instance.SetAP(this.getAP());
                        GameConsole.instance.UpdateFeedback("Updated Specialist to Hazmat Technician.");


                        if (oldSpec == Specialist.Paramedic)
                        {
                            GameManager.GM.freeSpecialistIndex[0] = 1;
                        }
                        else if (oldSpec == Specialist.FireCaptain)
                        {
                            GameManager.GM.freeSpecialistIndex[1] = 1;
                        }
                        else if (oldSpec == Specialist.ImagingTechnician)
                        {
                            GameManager.GM.freeSpecialistIndex[2] = 1;
                        }
                        else if (oldSpec == Specialist.CAFSFirefighter)
                        {
                            GameManager.GM.freeSpecialistIndex[3] = 1;
                        }
                        else if (oldSpec == Specialist.HazmatTechinician)
                        {
                            isWaitingForInput = true;
                            isChangingCrew = true;
                            GameConsole.instance.UpdateFeedback("You're already a Hazmat Technician. \n" + oldMessage);
                        }
                        else if (oldSpec == Specialist.Generalist)
                        {
                            GameManager.GM.freeSpecialistIndex[5] = 1;
                        }
                        else if (oldSpec == Specialist.RescueSpecialist)
                        {
                            GameManager.GM.freeSpecialistIndex[6] = 1;
                        }
                        else if (oldSpec == Specialist.DriverOperator)
                        {
                            GameManager.GM.freeSpecialistIndex[7] = 1;
                        }
                        else if (oldSpec == Specialist.RescueDog)
                        {
                            GameManager.GM.freeSpecialistIndex[8] = 1;
                        }
                        else if (oldSpec == Specialist.Veteran)
                        {
                            GameManager.GM.freeSpecialistIndex[9] = 1;
                        }

                        sendChangeCrewEvent(GameManager.GM.freeSpecialistIndex);

                        Space curr = this.getCurrentSpace();
                        Space destination = getEngineSpaces();
                        moveFirefighter(curr, destination, 0, true);
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isChangingCrew = true;
                    }
                }
            }

            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                if (isWaitingForInput && isChangingCrew)
                {
                    Debug.Log("Input 5 Received");
                    string oldMessage = GameConsole.instance.FeedbackText.text;
                    isWaitingForInput = false;
                    isChangingCrew = false;
                    if (GameManager.GM.freeSpecialistIndex[5]!=0) //make sure that specialist is in there 
                    {
                        Debug.Log("changing specialist to Generalist (5)");
                        Specialist oldSpec = this.spec;
                        this.spec = Specialist.Generalist;
                        FiremanUI.instance.SetSpecialist(Specialist.Generalist);
                        GameManager.GM.freeSpecialistIndex[5] = 0;
                        newSpecAP();
                        this.setAP(this.getAP() - 2);
                        FiremanUI.instance.SetAP(this.getAP());
                        GameConsole.instance.UpdateFeedback("Updated Specialist to Generalist.");


                        if (oldSpec == Specialist.Paramedic)
                        {
                            GameManager.GM.freeSpecialistIndex[0] = 1;
                        }
                        else if (oldSpec == Specialist.FireCaptain)
                        {
                            GameManager.GM.freeSpecialistIndex[1] = 1;
                        }
                        else if (oldSpec == Specialist.ImagingTechnician)
                        {
                            GameManager.GM.freeSpecialistIndex[2] = 1;
                        }
                        else if (oldSpec == Specialist.CAFSFirefighter)
                        {
                            GameManager.GM.freeSpecialistIndex[3] = 1;
                        }
                        else if (oldSpec == Specialist.HazmatTechinician)
                        {
                            GameManager.GM.freeSpecialistIndex[4] = 1;
                        }
                        else if (oldSpec == Specialist.Generalist)
                        {
                            isWaitingForInput = true;
                            isChangingCrew = true;
                            GameConsole.instance.UpdateFeedback("You're already a Hazmat Technician. \n" + oldMessage);
                        }
                        else if (oldSpec == Specialist.RescueSpecialist)
                        {
                            GameManager.GM.freeSpecialistIndex[6] = 1;
                        }
                        else if (oldSpec == Specialist.DriverOperator)
                        {
                            GameManager.GM.freeSpecialistIndex[7] = 1;
                        }
                        else if (oldSpec == Specialist.RescueDog)
                        {
                            GameManager.GM.freeSpecialistIndex[8] = 1;
                        }
                        else if (oldSpec == Specialist.Veteran)
                        {
                            GameManager.GM.freeSpecialistIndex[9] = 1;
                        }

                        sendChangeCrewEvent(GameManager.GM.freeSpecialistIndex);

                        Space curr = this.getCurrentSpace();
                        Space destination = getEngineSpaces();
                        moveFirefighter(curr, destination, 0, true);
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isChangingCrew = true;
                    }
                }
            }

            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                if (isWaitingForInput && isChangingCrew)
                {
                    Debug.Log("Input 6 Received");
                    string oldMessage = GameConsole.instance.FeedbackText.text;
                    isWaitingForInput = false;
                    isChangingCrew = false;
                    if (GameManager.GM.freeSpecialistIndex[6]!=0) //make sure that specialist is in there 
                    {
                        Debug.Log("changing specialist to Rescue Specialist (6)");
                        Specialist oldSpec = this.spec;
                        this.spec = Specialist.RescueSpecialist;
                        FiremanUI.instance.SetSpecialist(Specialist.RescueSpecialist);
                        GameManager.GM.freeSpecialistIndex[6] = 0;
                        newSpecAP();
                        this.setAP(this.getAP() - 2);
                        FiremanUI.instance.SetAP(this.getAP());
                        GameConsole.instance.UpdateFeedback("Updated Specialist to Rescue Specialist.");


                        if (oldSpec == Specialist.Paramedic)
                        {
                            GameManager.GM.freeSpecialistIndex[0] = 1;
                        }
                        else if (oldSpec == Specialist.FireCaptain)
                        {
                            GameManager.GM.freeSpecialistIndex[1] = 1;
                        }
                        else if (oldSpec == Specialist.ImagingTechnician)
                        {
                            GameManager.GM.freeSpecialistIndex[2] = 1;
                        }
                        else if (oldSpec == Specialist.CAFSFirefighter)
                        {
                            GameManager.GM.freeSpecialistIndex[3] = 1;
                        }
                        else if (oldSpec == Specialist.HazmatTechinician)
                        {
                            GameManager.GM.freeSpecialistIndex[4] = 1;
                        }
                        else if (oldSpec == Specialist.Generalist)
                        {
                            GameManager.GM.freeSpecialistIndex[5] = 1;
                        }
                        else if (oldSpec == Specialist.RescueSpecialist)
                        {
                            isWaitingForInput = true;
                            isChangingCrew = true;
                            GameConsole.instance.UpdateFeedback("You're already a Rescue Specialist. \n" + oldMessage);
                        }
                        else if (oldSpec == Specialist.DriverOperator)
                        {
                            GameManager.GM.freeSpecialistIndex[7] = 1;
                        }
                        else if (oldSpec == Specialist.RescueDog)
                        {
                            GameManager.GM.freeSpecialistIndex[8] = 1;
                        }
                        else if (oldSpec == Specialist.Veteran)
                        {
                            GameManager.GM.freeSpecialistIndex[9] = 1;
                        }

                        sendChangeCrewEvent(GameManager.GM.freeSpecialistIndex);

                        Space curr = this.getCurrentSpace();
                        Space destination = getEngineSpaces();
                        moveFirefighter(curr, destination, 0, true);
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isChangingCrew = true;
                    }
                }
            }

            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                if (isWaitingForInput && isChangingCrew)
                {
                    Debug.Log("Input 7 Received");
                    string oldMessage = GameConsole.instance.FeedbackText.text;
                    isWaitingForInput = false;
                    isChangingCrew = false;
                    if (GameManager.GM.freeSpecialistIndex[7]!=0) //make sure that specialist is in there 
                    {
                        Debug.Log("changing specialist to Driver/Operator (7)");
                        Specialist oldSpec = this.spec;
                        this.spec = Specialist.DriverOperator;
                        FiremanUI.instance.SetSpecialist(Specialist.DriverOperator);
                        GameManager.GM.freeSpecialistIndex[7] = 0;
                        newSpecAP();
                        this.setAP(this.getAP() - 2);
                        FiremanUI.instance.SetAP(this.getAP());
                        GameConsole.instance.UpdateFeedback("Updated Specialist to Driver/Operator.");


                        if (oldSpec == Specialist.Paramedic)
                        {
                            GameManager.GM.freeSpecialistIndex[0] = 1;
                        }
                        else if (oldSpec == Specialist.FireCaptain)
                        {
                            GameManager.GM.freeSpecialistIndex[1] = 1;
                        }
                        else if (oldSpec == Specialist.ImagingTechnician)
                        {
                            GameManager.GM.freeSpecialistIndex[2] = 1;
                        }
                        else if (oldSpec == Specialist.CAFSFirefighter)
                        {
                            GameManager.GM.freeSpecialistIndex[3] = 1;
                        }
                        else if (oldSpec == Specialist.HazmatTechinician)
                        {
                            GameManager.GM.freeSpecialistIndex[4] = 1;
                        }
                        else if (oldSpec == Specialist.Generalist)
                        {
                            GameManager.GM.freeSpecialistIndex[5] = 1;
                        }
                        else if (oldSpec == Specialist.RescueSpecialist)
                        {
                            GameManager.GM.freeSpecialistIndex[6] = 1;
                        }
                        else if (oldSpec == Specialist.DriverOperator)
                        {
                            isWaitingForInput = true;
                            isChangingCrew = true;
                            GameConsole.instance.UpdateFeedback("You're already a Driver/Operator. \n" + oldMessage);
                        }
                        else if (oldSpec == Specialist.RescueDog)
                        {
                            GameManager.GM.freeSpecialistIndex[8] = 1;
                        }
                        else if (oldSpec == Specialist.Veteran)
                        {
                            GameManager.GM.freeSpecialistIndex[9] = 1;
                        }


                        sendChangeCrewEvent(GameManager.GM.freeSpecialistIndex);

                        Space curr = this.getCurrentSpace();
                        Space destination = getEngineSpaces();
                        moveFirefighter(curr, destination, 0, true);
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isChangingCrew = true;
                    }
                }
            }

            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                if (isWaitingForInput && isChangingCrew)
                {
                    Debug.Log("Input 8 Received");
                    string oldMessage = GameConsole.instance.FeedbackText.text;
                    isWaitingForInput = false;
                    isChangingCrew = false;
                    if (GameManager.GM.freeSpecialistIndex[8] != 0) //make sure that specialist is in there 
                    {
                        Debug.Log("changing specialist to Rescue Dog (8)");
                        Specialist oldSpec = this.spec;
                        this.spec = Specialist.RescueDog;
                        FiremanUI.instance.SetSpecialist(Specialist.RescueDog);
                        GameManager.GM.freeSpecialistIndex[8] = 0;
                        newSpecAP();
                        this.setAP(this.getAP() - 2);
                        FiremanUI.instance.SetAP(this.getAP());
                        GameConsole.instance.UpdateFeedback("Updated Specialist to Rescue Dog.");


                        if (oldSpec == Specialist.Paramedic)
                        {
                            GameManager.GM.freeSpecialistIndex[0] = 1;
                        }
                        else if (oldSpec == Specialist.FireCaptain)
                        {
                            GameManager.GM.freeSpecialistIndex[1] = 1;
                        }
                        else if (oldSpec == Specialist.ImagingTechnician)
                        {
                            GameManager.GM.freeSpecialistIndex[2] = 1;
                        }
                        else if (oldSpec == Specialist.CAFSFirefighter)
                        {
                            GameManager.GM.freeSpecialistIndex[3] = 1;
                        }
                        else if (oldSpec == Specialist.HazmatTechinician)
                        {
                            GameManager.GM.freeSpecialistIndex[4] = 1;
                        }
                        else if (oldSpec == Specialist.Generalist)
                        {
                            GameManager.GM.freeSpecialistIndex[5] = 1;
                        }
                        else if (oldSpec == Specialist.RescueSpecialist)
                        {
                            GameManager.GM.freeSpecialistIndex[6] = 1;
                        }
                        else if (oldSpec == Specialist.DriverOperator)
                        {
                            GameManager.GM.freeSpecialistIndex[7] = 1;
                        }
                        else if (oldSpec == Specialist.RescueDog)
                        {
                            isWaitingForInput = true;
                            isChangingCrew = true;
                            GameConsole.instance.UpdateFeedback("You're already a Driver/Operator. \n" + oldMessage);
                        }
                        else if (oldSpec == Specialist.Veteran)
                        {
                            GameManager.GM.freeSpecialistIndex[9] = 1;
                        }

                        sendChangeCrewEvent(GameManager.GM.freeSpecialistIndex);

                        Space curr = this.getCurrentSpace();
                        Space destination = getEngineSpaces();
                        moveFirefighter(curr, destination, 0, true);
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isChangingCrew = true;
                    }
                }
            }

            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                if (isWaitingForInput && isChangingCrew)
                {
                    Debug.Log("Input 9 Received");
                    string oldMessage = GameConsole.instance.FeedbackText.text;
                    isWaitingForInput = false;
                    isChangingCrew = false;
                    if (GameManager.GM.freeSpecialistIndex[9] != 0) //make sure that specialist is in there 
                    {
                        Debug.Log("changing specialist to Veteran (9)");
                        Specialist oldSpec = this.spec;
                        this.spec = Specialist.Veteran;
                        FiremanUI.instance.SetSpecialist(Specialist.Veteran);
                        GameManager.GM.freeSpecialistIndex[9] = 0;
                        newSpecAP();
                        this.setAP(this.getAP() - 2);
                        FiremanUI.instance.SetAP(this.getAP());
                        GameConsole.instance.UpdateFeedback("Updated Specialist to Veteran.");


                        if (oldSpec == Specialist.Paramedic)
                        {
                            GameManager.GM.freeSpecialistIndex[0] = 1;
                        }
                        else if (oldSpec == Specialist.FireCaptain)
                        {
                            GameManager.GM.freeSpecialistIndex[1] = 1;
                        }
                        else if (oldSpec == Specialist.ImagingTechnician)
                        {
                            GameManager.GM.freeSpecialistIndex[2] = 1;
                        }
                        else if (oldSpec == Specialist.CAFSFirefighter)
                        {
                            GameManager.GM.freeSpecialistIndex[3] = 1;
                        }
                        else if (oldSpec == Specialist.HazmatTechinician)
                        {
                            GameManager.GM.freeSpecialistIndex[4] = 1;
                        }
                        else if (oldSpec == Specialist.Generalist)
                        {
                            GameManager.GM.freeSpecialistIndex[5] = 1;
                        }
                        else if (oldSpec == Specialist.RescueSpecialist)
                        {
                            GameManager.GM.freeSpecialistIndex[6] = 1;
                        }
                        else if (oldSpec == Specialist.DriverOperator)
                        {
                            GameManager.GM.freeSpecialistIndex[7] = 1;
                        }
                        else if (oldSpec == Specialist.RescueDog)
                        {
                            GameManager.GM.freeSpecialistIndex[8] = 1;
                        }
                        else if (oldSpec == Specialist.Veteran)
                        {
                            isWaitingForInput = true;
                            isChangingCrew = true;
                            GameConsole.instance.UpdateFeedback("You're already a Driver/Operator. \n" + oldMessage);
                        }

                        sendChangeCrewEvent(GameManager.GM.freeSpecialistIndex);

                        Space curr = this.getCurrentSpace();
                        Space destination = getEngineSpaces();
                        moveFirefighter(curr, destination, 0, true);
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isChangingCrew = true;
                    }
                }
            }


            else if (Input.GetKeyDown(KeyCode.Q))
            {
                endTurn();
            }
            else if (Input.GetKeyDown(KeyCode.Y))
            {
                if (isWaitingForInput && isSelectingExtinguishOption)
                {
                    sendFireMarkerExtinguishEvent(locationArgument);
                    locationArgument = null;
                    isWaitingForInput = false;
                    isSelectingExtinguishOption = false;
                    this.decrementRemoveFireAP();
                    FiremanUI.instance.SetAP(this.getAP());
                }
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                if (isWaitingForInput && isSelectingExtinguishOption)
                {
                    sendTurnFireMarkerToSmokeEvent(locationArgument);
                    locationArgument = null;
                    isWaitingForInput = false;
                    isSelectingExtinguishOption = false;
                    this.decrementFireToSmokeAP();
                    FiremanUI.instance.SetAP(this.getAP());
                } 

            }
            else if (Input.GetKeyDown(KeyCode.V))
            {
                carryVictim();
            }
            else if(Input.GetKeyDown(KeyCode.M)) {
                carryHazmat();
            }
        }

       else if (PV.IsMine && GameManager.GM.Turn == actorNumber && GameManager.GameStatus ==
       FlashPointGameConstants.GAME_STATUS_PICK_SPECIALIST)
        {
            //if the user presses 0
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                if (isWaitingForInput && isSelectingSpecialist)
                {
                    Debug.Log("Input 0 Received");
                    string oldMessage = GameConsole.instance.FeedbackText.text;
                    isWaitingForInput = false;
                    isSelectingSpecialist = false;

                    if (GameManager.GM.freeSpecialistIndex[0] != 0)
                    {
                        GameManager.GM.freeSpecialistIndex[0] = 0;
                        this.spec = Specialist.Paramedic;
                        FiremanUI.instance.SetSpecialist(Specialist.Paramedic);
                        newSpecAP();
                        GameConsole.instance.UpdateFeedback("Paramedic is picked as Specialist.");
                        GameManager.IncrementTurn();
                        sendChangeCrewEvent(GameManager.GM.freeSpecialistIndex);
                    }

                    else
                    {
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isSelectingSpecialist = true;
                    }
                }
            } 
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (isWaitingForInput && isSelectingSpecialist)
                {
                    Debug.Log("Input 1 Received");
                    string oldMessage = GameConsole.instance.FeedbackText.text;
                    isWaitingForInput = false;
                    isSelectingSpecialist = false;

                    if (GameManager.GM.freeSpecialistIndex[1] != 0)
                    {
                        GameManager.GM.freeSpecialistIndex[1] = 0;
                        this.spec = Specialist.FireCaptain;
                        FiremanUI.instance.SetSpecialist(Specialist.FireCaptain);
                        newSpecAP();
                        GameConsole.instance.UpdateFeedback("Fire Captain is picked as Specialist.");
                        GameManager.IncrementTurn();
                        sendChangeCrewEvent(GameManager.GM.freeSpecialistIndex);
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isSelectingSpecialist = true;
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (isWaitingForInput && isSelectingSpecialist)
                {
                    Debug.Log("Input 2 Received");
                    string oldMessage = GameConsole.instance.FeedbackText.text;
                    isWaitingForInput = false;
                    isSelectingSpecialist = false;

                    if (GameManager.GM.freeSpecialistIndex[2] != 0)
                    {
                        GameManager.GM.freeSpecialistIndex[2] = 0;
                        this.spec = Specialist.ImagingTechnician;
                        FiremanUI.instance.SetSpecialist(Specialist.ImagingTechnician);
                        newSpecAP();
                        GameConsole.instance.UpdateFeedback("Imaging Technician is picked as Specialist.");
                        GameManager.IncrementTurn();
                        sendChangeCrewEvent(GameManager.GM.freeSpecialistIndex);
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isSelectingSpecialist = true;
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (isWaitingForInput && isSelectingSpecialist)
                {
                    Debug.Log("Input 3 Received");
                    string oldMessage = GameConsole.instance.FeedbackText.text;
                    isWaitingForInput = false;
                    isSelectingSpecialist = false;

                    if (GameManager.GM.freeSpecialistIndex[3] != 0)
                    {
                        GameManager.GM.freeSpecialistIndex[3] = 0;
                        this.spec = Specialist.CAFSFirefighter;
                        FiremanUI.instance.SetSpecialist(Specialist.CAFSFirefighter);
                        newSpecAP();
                        GameConsole.instance.UpdateFeedback("CAFS Firefighter is picked as Specialist.");
                        GameManager.IncrementTurn();
                        sendChangeCrewEvent(GameManager.GM.freeSpecialistIndex);
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isSelectingSpecialist = true;
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (isWaitingForInput && isSelectingSpecialist)
                {
                    Debug.Log("Input 4 Received");
                    string oldMessage = GameConsole.instance.FeedbackText.text;
                    isWaitingForInput = false;
                    isSelectingSpecialist = false;

                    if (GameManager.GM.freeSpecialistIndex[4] != 0)
                    {
                        GameManager.GM.freeSpecialistIndex[4] = 0;
                        this.spec = Specialist.HazmatTechinician;
                        FiremanUI.instance.SetSpecialist(Specialist.HazmatTechinician);
                        newSpecAP();
                        GameConsole.instance.UpdateFeedback("Hazmat Technician is picked as Specialist.");
                        GameManager.IncrementTurn();
                        sendChangeCrewEvent(GameManager.GM.freeSpecialistIndex);
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isSelectingSpecialist = true;
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                if (isWaitingForInput && isSelectingSpecialist)
                {
                    Debug.Log("Input 5 Received");
                    string oldMessage = GameConsole.instance.FeedbackText.text;
                    isWaitingForInput = false;
                    isSelectingSpecialist = false;

                    if (GameManager.GM.freeSpecialistIndex[5] != 0)
                    {
                        GameManager.GM.freeSpecialistIndex[5] = 0;
                        this.spec = Specialist.Generalist;
                        FiremanUI.instance.SetSpecialist(Specialist.Generalist);
                        newSpecAP();
                        GameConsole.instance.UpdateFeedback("Generalist is picked as Specialist.");
                        GameManager.IncrementTurn();
                        sendChangeCrewEvent(GameManager.GM.freeSpecialistIndex);
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isSelectingSpecialist = true;
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6)) 
            {
                if (isWaitingForInput && isSelectingSpecialist)
                {
                    Debug.Log("Input 6 Received");
                    string oldMessage = GameConsole.instance.FeedbackText.text;
                    isWaitingForInput = false;
                    isSelectingSpecialist = false;

                    if (GameManager.GM.freeSpecialistIndex[6] != 0)
                    {
                        GameManager.GM.freeSpecialistIndex[6] = 0;
                        this.spec = Specialist.RescueSpecialist;
                        FiremanUI.instance.SetSpecialist(Specialist.RescueSpecialist);
                        newSpecAP();
                        GameConsole.instance.UpdateFeedback("Rescue Specialist is picked as Specialist.");
                        GameManager.IncrementTurn();
                        sendChangeCrewEvent(GameManager.GM.freeSpecialistIndex);
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isSelectingSpecialist = true;
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                if (isWaitingForInput && isSelectingSpecialist)
                {
                    Debug.Log("Input 7 Received");
                    string oldMessage = GameConsole.instance.FeedbackText.text;
                    isWaitingForInput = false;
                    isSelectingSpecialist = false;

                    if (GameManager.GM.freeSpecialistIndex[7] != 0)
                    {
                        GameManager.GM.freeSpecialistIndex[7] = 0;
                        this.spec = Specialist.DriverOperator;
                        FiremanUI.instance.SetSpecialist(Specialist.DriverOperator);
                        newSpecAP();
                        GameConsole.instance.UpdateFeedback("Driver Operator is picked as Specialist.");
                        GameManager.IncrementTurn();
                        sendChangeCrewEvent(GameManager.GM.freeSpecialistIndex);
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isSelectingSpecialist = true;
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                if (isWaitingForInput && isSelectingSpecialist)
                {
                    Debug.Log("Input 8 Received");
                    string oldMessage = GameConsole.instance.FeedbackText.text;
                    isWaitingForInput = false;
                    isSelectingSpecialist = false;

                    if (GameManager.GM.freeSpecialistIndex[8] != 0)
                    {
                        GameManager.GM.freeSpecialistIndex[8] = 0;
                        this.spec = Specialist.RescueDog;
                        FiremanUI.instance.SetSpecialist(Specialist.RescueDog);
                        newSpecAP();
                        GameConsole.instance.UpdateFeedback("Rescue Dog is picked as Specialist.");
                        GameManager.IncrementTurn();
                        sendChangeCrewEvent(GameManager.GM.freeSpecialistIndex);
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isSelectingSpecialist = true;
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                if (isWaitingForInput && isSelectingSpecialist)
                {
                    Debug.Log("Input 9 Received");
                    string oldMessage = GameConsole.instance.FeedbackText.text;
                    isWaitingForInput = false;
                    isSelectingSpecialist = false;

                    if (GameManager.GM.freeSpecialistIndex[9] != 0)
                    {
                        GameManager.GM.freeSpecialistIndex[9] = 0;
                        this.spec = Specialist.Veteran;
                        FiremanUI.instance.SetSpecialist(Specialist.Veteran);
                        newSpecAP();
                        GameConsole.instance.UpdateFeedback("Veteran is picked as Specialist.");
                        GameManager.IncrementTurn();
                        sendChangeCrewEvent(GameManager.GM.freeSpecialistIndex);
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isSelectingSpecialist = true;
                    }
                }
            }
        }
        else if (GameManager.GM.Turn != actorNumber)
        {
            GameConsole.instance.UpdateFeedback("It's not your turn!");
        }
    }

    public int getAP()
    {
        return AP;
    }

    public void setAP(int newAP)
    {
        AP = newAP;
    }

    public Specialist GetSpecialist()
    {
        return this.spec;
    }

    public void setSpecialist(Specialist newSpecialist)
    {
        this.spec = newSpecialist;
    }

    public void newSpecAP()
    {

        if (this.spec == Specialist.FamilyGame)
        {
            this.setAP(4);
            FiremanUI.instance.SetAP(this.getAP());
            this.commandAP = 0;
            this.extinguishAP = 0;
            this.moveAP = 0;
            FiremanUI.instance.SetSpecialistAP(0);
        }
        else if (this.spec == Specialist.Paramedic)
        {
            this.setAP(4);
            FiremanUI.instance.SetAP(this.getAP());
            this.commandAP = 0;
            this.extinguishAP = 0;
            this.moveAP = 0;
            FiremanUI.instance.SetSpecialistAP(0);
        }
        else if (this.spec == Specialist.FireCaptain)
        {
            this.setAP(4);
            FiremanUI.instance.SetAP(this.getAP());
            this.commandAP = 2;
            this.extinguishAP = 0;
            this.moveAP = 0;
            FiremanUI.instance.SetSpecialistAP(2);
        }
        else if (this.spec == Specialist.ImagingTechnician)
        {
            this.setAP(4);
            FiremanUI.instance.SetAP(this.getAP());
            this.commandAP = 0;
            this.extinguishAP = 0;
            this.moveAP = 0;
            FiremanUI.instance.SetSpecialistAP(0);
        }
        else if (this.spec == Specialist.CAFSFirefighter)
        {
            this.setAP(3);
            FiremanUI.instance.SetAP(this.getAP());
            this.commandAP = 0;
            this.extinguishAP = 3;
            this.moveAP = 0;
            FiremanUI.instance.SetSpecialistAP(3);
        }
        else if (this.spec == Specialist.HazmatTechinician)
        {
            this.setAP(4);
            FiremanUI.instance.SetAP(this.getAP());
            this.commandAP = 0;
            this.extinguishAP = 0;
            this.moveAP = 0;
            FiremanUI.instance.SetSpecialistAP(0);
        }
        else if (this.spec == Specialist.Generalist)
        {
            this.setAP(5);
            FiremanUI.instance.SetAP(this.getAP());
            this.commandAP = 0;
            this.extinguishAP = 0;
            this.moveAP = 0;
            FiremanUI.instance.SetSpecialistAP(0);
        }
        else if (this.spec == Specialist.RescueSpecialist)
        {
            this.setAP(4);
            FiremanUI.instance.SetAP(this.getAP());
            this.commandAP = 0;
            this.extinguishAP = 0;
            this.moveAP = 3;
            FiremanUI.instance.SetSpecialistAP(3);
        }
        else if (this.spec == Specialist.DriverOperator)
        {
            this.setAP(4);
            FiremanUI.instance.SetAP(this.getAP());
            this.commandAP = 0;
            this.extinguishAP = 0;
            this.moveAP = 0;
            FiremanUI.instance.SetSpecialistAP(0);
        }
        else if (this.spec == Specialist.Veteran) //TODO
        {
            this.setAP(4);
            FiremanUI.instance.SetAP(this.getAP());
            this.commandAP = 0;
            this.extinguishAP = 0;
            this.moveAP = 0;
            FiremanUI.instance.SetSpecialistAP(0);
        }
        else if (this.spec == Specialist.RescueDog)
        {
            this.setAP(12);
            FiremanUI.instance.SetAP(this.getAP());
            this.commandAP = 0;
            this.extinguishAP = 0;
            this.moveAP = 0;
            FiremanUI.instance.SetSpecialistAP(0);
        }
    }

    public int getSavedAP()
    {
        return AP;
    }

    public void decrementAP(int amount)
    {
        this.AP -= amount;
    }

    public void setAPStartTurn()
    {
        AP = getSavedAP() + 4;
    }

    public FMStatus getStatus()
    {
        return this.status;
    }

    public void setStatus(FMStatus newStatus)
    {
        this.status = newStatus;
    }

    public Ambulance getAmbulance()
    {
        return this.movedAmbulance;
    }

    public void setAmbulance(Ambulance h)
    {
        this.movedAmbulance = h;
    }

    public Engine getEngine()
    {
        return this.movedEngine;
    }

    public void setEngine(Engine n)
    {
        this.movedEngine = n;
    }

    public Space getEngineSpaces()
    {
        Space engine1 = StateManager.instance.spaceGrid.getGrid()[9, 5];

        foreach (GameUnit gu in engine1.getOccupants())
        {
            if (gu != null && gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_ENGINE) return engine1;
        }

        Space engine2 = StateManager.instance.spaceGrid.getGrid()[0, 1];

        foreach (GameUnit gu in engine2.getOccupants())
        {
            if (gu != null && gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_ENGINE) return engine2;
        }

        Space engine3 = StateManager.instance.spaceGrid.getGrid()[8, 0];

        foreach (GameUnit gu in engine3.getOccupants())
        {
            if (gu != null && gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_ENGINE) return engine3;
        }

        Space engine4 = StateManager.instance.spaceGrid.getGrid()[2, 7];

        foreach (GameUnit gu in engine4.getOccupants())
        {
            if (gu != null && gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_ENGINE) return engine4;
        }

        return engine1;
    }

    public void exitVehicle()
    {
        if (movedAmbulance != null)
        {
            this.movedAmbulance = null;
            GameConsole.instance.UpdateFeedback("Exited ambulance successfully");
        }
        else if (movedEngine != null)
        {
            this.movedEngine = null;
            GameConsole.instance.UpdateFeedback("Exited engine successfully");
        }
        else
        {
            GameConsole.instance.UpdateFeedback("You are not riding a vehicle");

        }
    }

    public Victim getVictim()
    {
        return this.carriedVictim;
    }

    public void setVictim(Victim v)
    {
        this.carriedVictim = v;
    }

    public Hazmat getHazmat() {
        return carriedHazmat;
    }

    public void setHazmat(Hazmat h) {
        carriedHazmat = h;
    }

    public static void selectSpecialist()
    {
        isWaitingForInput = true;
        isSelectingSpecialist = true;
        string optionsToUser = "";
        for (int i = 0; i < GameManager.GM.freeSpecialistIndex.Length; i++)
        {
            if (GameManager.GM.freeSpecialistIndex[i] != 0)
            {
                optionsToUser = optionsToUser + "Press " + i + " for " + GameManager.GM.availableSpecialists[i] + ". ";
            }
        }
        GameConsole.instance.UpdateFeedback(optionsToUser);

    }
    public List<Space> GetQuadrant(Space s)
    {
        Space[,] spaceGrid = StateManager.instance.spaceGrid.getGrid();
        List<Space> spacesInQuadrant = new List<Space>();
        int col = s.indexX;
        int row = s.indexY;
        if(col == 8 && row == 0)
        {
            //first row
            spacesInQuadrant.Add(spaceGrid[5, 1]);
            spacesInQuadrant.Add(spaceGrid[6, 1]);
            spacesInQuadrant.Add(spaceGrid[7, 1]);
            spacesInQuadrant.Add(spaceGrid[8, 1]);

            //second row
            spacesInQuadrant.Add(spaceGrid[5, 2]);
            spacesInQuadrant.Add(spaceGrid[6, 2]);
            spacesInQuadrant.Add(spaceGrid[7, 2]);
            spacesInQuadrant.Add(spaceGrid[8, 2]);

            //third row
            spacesInQuadrant.Add(spaceGrid[5, 3]);
            spacesInQuadrant.Add(spaceGrid[6, 3]);
            spacesInQuadrant.Add(spaceGrid[7, 3]);
            spacesInQuadrant.Add(spaceGrid[8, 3]);
        }
        else if(col == 0 && row == 1)
        {
            //first row
            spacesInQuadrant.Add(spaceGrid[1, 1]);
            spacesInQuadrant.Add(spaceGrid[2, 1]);
            spacesInQuadrant.Add(spaceGrid[3, 1]);
            spacesInQuadrant.Add(spaceGrid[4, 1]);

            //second row
            spacesInQuadrant.Add(spaceGrid[1, 2]);
            spacesInQuadrant.Add(spaceGrid[2, 2]);
            spacesInQuadrant.Add(spaceGrid[3, 2]);
            spacesInQuadrant.Add(spaceGrid[4, 2]);

            //third row
            spacesInQuadrant.Add(spaceGrid[1, 3]);
            spacesInQuadrant.Add(spaceGrid[2, 3]);
            spacesInQuadrant.Add(spaceGrid[3, 3]);
            spacesInQuadrant.Add(spaceGrid[4, 3]);
        }
        else if (col == 9 && row == 5)
        {
            //first row
            spacesInQuadrant.Add(spaceGrid[5, 4]);
            spacesInQuadrant.Add(spaceGrid[6, 4]);
            spacesInQuadrant.Add(spaceGrid[7, 4]);
            spacesInQuadrant.Add(spaceGrid[8, 4]);

            //second row
            spacesInQuadrant.Add(spaceGrid[5, 5]);
            spacesInQuadrant.Add(spaceGrid[6, 5]);
            spacesInQuadrant.Add(spaceGrid[7, 5]);
            spacesInQuadrant.Add(spaceGrid[8, 5]);

            //third row
            spacesInQuadrant.Add(spaceGrid[5, 6]);
            spacesInQuadrant.Add(spaceGrid[6, 6]);
            spacesInQuadrant.Add(spaceGrid[7, 6]);
            spacesInQuadrant.Add(spaceGrid[8, 6]);
        }
        else if (col == 2 && row == 7)
        {
            //first row
            spacesInQuadrant.Add(spaceGrid[1, 4]);
            spacesInQuadrant.Add(spaceGrid[2, 4]);
            spacesInQuadrant.Add(spaceGrid[3, 4]);
            spacesInQuadrant.Add(spaceGrid[4, 4]);

            //second row
            spacesInQuadrant.Add(spaceGrid[1, 5]);
            spacesInQuadrant.Add(spaceGrid[2, 5]);
            spacesInQuadrant.Add(spaceGrid[3, 5]);
            spacesInQuadrant.Add(spaceGrid[4, 5]);

            //third row
            spacesInQuadrant.Add(spaceGrid[1, 6]);
            spacesInQuadrant.Add(spaceGrid[2, 6]);
            spacesInQuadrant.Add(spaceGrid[3, 6]);
            spacesInQuadrant.Add(spaceGrid[4, 6]);
        }
        return spacesInQuadrant;
    }
    public void rollDiceInQuadrant(List<Space> quadrant, int col,int row)
    {
        bool outsideRed = true;
        bool outsideBlack = true;

        foreach(Space s in quadrant)
        {
            if (s.indexX == col)
                outsideBlack = false;
            if (s.indexY == row)
                outsideRed = false;
        }

        if (outsideRed)
        {
            GameManager.redDice = 7 - row;
        }
        if (outsideBlack)
        {
            GameManager.blackDice = 9 - col;
        }
        Debug.Log("Deck Gun Black dice is: " + GameManager.blackDice + ", Deck gun red dice is " + GameManager.redDice);

    }
    public void DriverReroll(int number)
    {
        string message = "";
        if(number == 1 && !driverRerolledBlackDice)
        {
            driverRerolledBlackDice = true;
            Space currentSpace = this.getCurrentSpace();
            List<Space> quadSpaces = GetQuadrant(currentSpace);

            GameManager.RerollBlackDie();
            rollDiceInQuadrant(quadSpaces, GameManager.blackDice, GameManager.redDice);
            if (driverRerolledRedDice)
            {
                fireDeckGun();
                return;
            }
            message += "Target Space is at " + GameManager.blackDice + ", " + GameManager.redDice + "\n Press 2 to reroll the red die. " +
                "Press 4 to keep what you have.";
            GameConsole.instance.UpdateFeedback(message);

        }
        else if(number == 2 && !driverRerolledRedDice)
        {
            driverRerolledRedDice = true;
            Space currentSpace = this.getCurrentSpace();
            List<Space> quadSpaces = GetQuadrant(currentSpace);

            GameManager.RerollRedDie();
            rollDiceInQuadrant(quadSpaces, GameManager.blackDice, GameManager.redDice);
            if (driverRerolledBlackDice)
            {
                fireDeckGun();
                return;
            }
            message += "Target Space is at " + GameManager.blackDice + ", " + GameManager.redDice + "\n Press 1 to reroll the " +
                    "black die. Press 4 to keep what you have.";
            GameConsole.instance.UpdateFeedback(message);

        }
        else if(number == 3 && !driverRerolledBlackDice && !driverRerolledRedDice)
        {
            driverRerolledRedDice = true;
            driverRerolledBlackDice = true;
            isWaitingForInput = false;
            isFiringDeckGun = false;
            Space currentSpace = this.getCurrentSpace();
            List<Space> quadSpaces = GetQuadrant(currentSpace);

            GameManager.rollDice();
            rollDiceInQuadrant(quadSpaces, GameManager.blackDice, GameManager.redDice);
            fireDeckGun();
        }
        else
        {
            GameConsole.instance.UpdateFeedback("Invalid input.\n");
            if (driverRerolledRedDice)
                GameConsole.instance.UpdateFeedback("Target Space is at " + GameManager.blackDice + ", " + GameManager.redDice + "\n Press 1 to reroll the " +
                    "black die. Press 4 to keep what you have.");
            else if(driverRerolledBlackDice)
                GameConsole.instance.UpdateFeedback("Target Space is at " + GameManager.blackDice + ", " + GameManager.redDice + "\n Press 2 to reroll the red die. " +
                "Press 4 to keep what you have.");
        }
    }
    public void CallDeckGun()
    {
        Specialist s = this.GetSpecialist();
        int numAP = this.getAP();
        if(s == Specialist.DriverOperator && numAP < 2 || s != Specialist.DriverOperator && numAP < 4)
        {
            Debug.Log("Insufficient AP");
            GameConsole.instance.UpdateFeedback("Insufficient AP");
        }
        else
        {
            Space currentSpace = this.getCurrentSpace();
            List<Space> quadSpaces = GetQuadrant(currentSpace);
            bool isOnEngineSpace = false;
            bool containsFiremen = false;

            //FireFighters can only fire deck gun when in the same space as enigne.
            foreach (GameUnit gu in currentSpace.getOccupants())
            {
                if (gu != null && gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_ENGINE)
                    isOnEngineSpace = true;
            }

            if (!isOnEngineSpace)
            {
                GameConsole.instance.UpdateFeedback("You must be on the engine space to fire the deck gun!");
                return;
            }

            Dictionary<int, Space> x = StateManager.instance.firemanCurrentSpaces;

            //deck gun can only be fired at a quadrant where no firefighter is present.
            foreach (Space space in quadSpaces)
            {
                foreach (KeyValuePair<int, Space> firemanSpace in StateManager.instance.firemanCurrentSpaces)
                {
                    if (space.indexX == firemanSpace.Value.indexX && space.indexY == firemanSpace.Value.indexY)
                        containsFiremen = true;
                }
            }

            if (containsFiremen)
            {
                GameConsole.instance.UpdateFeedback("You cannot fire deck gun on a quadrant that has a fireman");
                return;
            }

            //rolling dice.
            if (s == Specialist.DriverOperator)
            {
                GameManager.rollDice();
                rollDiceInQuadrant(quadSpaces,GameManager.blackDice,GameManager.redDice);

                decrementAP(2);

                GameConsole.instance.UpdateFeedback("Target Space is at " + GameManager.blackDice + ", " + GameManager.redDice + "\n Press 1 to reroll the " +
                	"black die. Press 2 to reroll the red die. Press 3 to reroll both. Press 4 to keep what you have.");
                isWaitingForInput = true;
                isFiringDeckGun = true;
            }
            else
            {
                GameManager.rollDice();
                rollDiceInQuadrant(quadSpaces, GameManager.blackDice, GameManager.redDice);
                fireDeckGun();
                decrementAP(4);
            }
            FiremanUI.instance.SetAP(this.getAP());
        }
    }
    public void dispose()
    {
        int numAP = getAP();

        if (numAP >= 2 && this.spec == Specialist.HazmatTechinician)
        {
            this.setAP(numAP - 2);
            FiremanUI.instance.SetAP(this.getAP());

            //Remove a Hazmat from the Firefighter’s space and place in the rescued spot: 2 AP
            Space curr = this.getCurrentSpace();
            List<GameUnit> gameUnits = curr.getOccupants();
            GameUnit hazmat = null;
            foreach (GameUnit gu in gameUnits)
            {
                if (gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_HAZMAT)
                {
                    hazmat = gu;
                    break;
                }
            }
            Vector3 position = new Vector3(curr.worldPosition.x, curr.worldPosition.y, -5);
            gameUnits.Remove(hazmat);
            Destroy(hazmat.physicalObject);
            Destroy(hazmat);
            GameConsole.instance.UpdateFeedback("Removed hazmat successfully.");
            return;
        }
        else if (numAP < 2 && this.spec == Specialist.HazmatTechinician)
        {
            Debug.Log("Not enough AP!");  //Used to show the player why he can’t perform an action in case of failure
            GameConsole.instance.UpdateFeedback("Not enough AP!");
        }
    }
    public void fireDeckGun()
    {
        driverRerolledRedDice = false;
        driverRerolledBlackDice = false;
        GameConsole.instance.UpdateFeedback("Target Space is at " + GameManager.blackDice + ", " + GameManager.redDice + ". Splashing over adjacent spaces");
        Space targetSpace = StateManager.instance.spaceGrid.getGrid()[GameManager.blackDice, GameManager.redDice];
        List<GameUnit> targetSpaceGameUnits = targetSpace.getOccupants();
        Space[] neighbors = StateManager.instance.spaceGrid.GetNeighbours(targetSpace);

        //removing the target space's game units
        foreach (GameUnit gu in targetSpaceGameUnits)
        {
            if (gu != null && gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_FIREMARKER)
            {
                object[] data = { targetSpace.indexX, targetSpace.indexY };
                PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.RemoveFireMarker, data, sendToAllOptions, SendOptions.SendReliable);
            }
            else if (gu != null && gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_SMOKEMARKER)
            {
                object[] data = { targetSpace.indexX, targetSpace.indexY };
                PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.RemoveSmokeMarker, data, sendToAllOptions, SendOptions.SendReliable);
            }
        }

        //removing adjacent game units
        foreach (Space space in neighbors)
        {
            if (space != null)
            {
                List<GameUnit> spaceGameUnits = space.getOccupants();

                foreach (GameUnit gameUnit in spaceGameUnits)
                {
                    if (gameUnit != null && gameUnit.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_FIREMARKER)
                    {
                        object[] data = { space.indexX, space.indexY };
                        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.RemoveFireMarker, data, sendToAllOptions, SendOptions.SendReliable);
                    }
                    else if (gameUnit != null && gameUnit.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_SMOKEMARKER)
                    {
                        object[] data = { space.indexX, space.indexY };
                        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.RemoveSmokeMarker, data, sendToAllOptions, SendOptions.SendReliable);
                    }
                }
            }
        }
    }
    public void CallAmbulance()
    {

        int numAP = getAP(); //returns the number of action points

        //Check if sufficient AP.
        if (numAP < 2)
        {
            Debug.Log("Not enough AP!");  //Used to show the player why he can’t perform an action in case of failure
            GameConsole.instance.UpdateFeedback("Not enough AP!");
        }
        else
        {
            //Build string to show.
            string optionsToUser = "";

            if (numAP < 4)
            {
                optionsToUser += "press 1 to move the ambulance clockwise, press 3 to move the ambulance counter-clockwise";
            }
            else
            {
                optionsToUser += "press 1 to move the ambulance clockwise, press 2 to move the ambulance to the opposite place, press 3 to move " +
                	"the ambulance counter-clockwise";
            }

            GameConsole.instance.UpdateFeedback(optionsToUser);
            isCallingAmbulance = true;
            isWaitingForInput = true;

        }
    }
    public void CallEngine()
    {

        int numAP = getAP(); //returns the number of action points

        //Check if sufficient AP.
        if (numAP < 2)
        {
            Debug.Log("Not enough AP!");  //Used to show the player why he can’t perform an action in case of failure
            GameConsole.instance.UpdateFeedback("Not enough AP!");
        }
        else
        {
            //Build string to show.
            string optionsToUser = "";

            if (numAP < 4)
            {
                optionsToUser += "press 1 to move the engine clockwise, press 3 to move the engine counter-clockwise";
            }
            else
            {
                optionsToUser += "press 1 to move the engine clockwise, press 2 to move the engine to the opposite place, press 3 to move " +
                    "the engine counter-clockwise";
            }

            GameConsole.instance.UpdateFeedback(optionsToUser);
            isCallingEngine = true;
            isWaitingForInput = true;

        }
    }
    public void extinguishFire()
    {
        int numAP = getAP(); //returns the number of action points
        isDoubleSpec = (this.spec == Specialist.RescueSpecialist || this.spec == Specialist.Paramedic);

        //Get current space and spacestatus. 

        Space current = this.getCurrentSpace();
        SpaceStatus currentSpaceStatus = current.getSpaceStatus();
        if(currentSpaceStatus == SpaceStatus.Fire)
        {
            if (numAP == 1)
            {
                GameConsole.instance.UpdateFeedback("You only have enough AP to extinguish at your location and safely end the turn.");
                if (this.spec == Specialist.CAFSFirefighter && this.extinguishAP >= 1)
                {
                    this.extinguishAP = extinguishAP - 1;
                    FiremanUI.instance.SetSpecialistAP(this.extinguishAP);
                }
                else
                {
                    this.setAP(numAP - 1);
                }
                    FiremanUI.instance.SetAP(this.getAP());
                    sendTurnFireMarkerToSmokeEvent(current);
                    return;
            }
            else if (numAP == 2)
            {
                GameConsole.instance.UpdateFeedback("You only have enough AP to extinguish at your location and safely end the turn.");
                if (this.spec == Specialist.CAFSFirefighter && this.extinguishAP >= 2)
                {
                    this.extinguishAP = extinguishAP - 2;
                    FiremanUI.instance.SetSpecialistAP(this.extinguishAP);
                    FiremanUI.instance.SetAP(this.getAP());
                    sendFireMarkerExtinguishEvent(current);
                }
                else if (isDoubleSpec && numAP >= 2)
                {
                    GameConsole.instance.UpdateFeedback("You don't have enough AP to extinguish at your location. Safely end the turn.");
                    sendTurnFireMarkerToSmokeEvent(current);
                    this.setAP(numAP - 2);
                    FiremanUI.instance.SetAP(this.getAP());
                }
                else
                {
                    this.setAP(numAP - 2);
                    FiremanUI.instance.SetAP(this.getAP());
                    sendFireMarkerExtinguishEvent(current);
                }

                return;

            }
            else if (numAP == 4)
            {
                GameConsole.instance.UpdateFeedback("You only have enough AP to extinguish at your location and safely end the turn.");
                if (isDoubleSpec && this.extinguishAP >= 4)
                {
                    this.extinguishAP = extinguishAP - 4;
                    FiremanUI.instance.SetSpecialistAP(this.extinguishAP);
                }
                else
                {
                    this.setAP(numAP - 4);
                }
                FiremanUI.instance.SetAP(this.getAP());
                sendFireMarkerExtinguishEvent(current);
                return;

            }
        }

        //Get neighbors and their spacestatus. 
        Space[] neighbors = StateManager.instance.spaceGrid.GetNeighbours(current);
        SpaceStatus[] neighborsStatuses = new SpaceStatus[4];

        for (int i = 0; i < neighbors.Length; i++)
        {
            if (neighbors[i] != null)
            {
                neighborsStatuses[i] = neighbors[i].getSpaceStatus();
            }

        }

        //Check if sufficient AP.
        if (numAP < 1)
        {
            Debug.Log("Not enough AP!");  //Used to show the player why he can’t perform an action in case of failure
            GameConsole.instance.UpdateFeedback("Not enough AP!");
        }
        else
        {
            //Get indices of all spaces accessible that are not safe (valid neighbors + current Space).
            ArrayList extinguishOptions = getUnsafeSpacesIndecies(currentSpaceStatus, neighborsStatuses);
            validInputOptions = extinguishOptions;

            //Build string to show.
            string optionsToUser = "";

            foreach (int index in extinguishOptions)
            {


                if (index == 0)
                {
                    optionsToUser += "Press 0 for Tile on Top ";
                }
                else if (index == 1)
                {
                    optionsToUser += " Press 1 for Tile to Your Right";
                }
                else if (index == 2)
                {
                    optionsToUser += " Press 2 for the Tile to the Bottom";
                }
                else if (index == 3)
                {
                    optionsToUser += " Press 3 for the Tile to Your Left";

                }
                else
                {
                    optionsToUser += " Press 4 for the current Tile";

                }
            }

            GameConsole.instance.UpdateFeedback(optionsToUser);

            isWaitingForInput = true;
            isExtinguishingFire = true;

        }
    }

    private ArrayList getUnsafeSpacesIndecies(SpaceStatus currentSpaceStatus, SpaceStatus[] neighborsStatuses)
    {
        ArrayList indices = new ArrayList();

        //Collect directions in which there is a smoke or fire marker.
        for (int i = 0; i < neighborsStatuses.Length; i++)
        {
            if (neighborsStatuses[i] != SpaceStatus.Safe)
            {
                indices.Add(i);
            }
        }

        //Check for current space. Current Space index will be 4.
        if (currentSpaceStatus != SpaceStatus.Safe)
        {
            indices.Add(4);
        }

        return indices;
    }



    public void chopWall()     {         int numAP = getAP(); //returns the number of action point
        Specialist s = this.spec;
         //Check if sufficient AP.         if (s != Specialist.RescueSpecialist && numAP < 2 || s == Specialist.RescueSpecialist && numAP < 1)
        {             Debug.Log("Not enough AP!");  //Used to show the player why he can’t perform an action in case of failure             GameConsole.instance.UpdateFeedback("Not enough AP!");         }         else         {             //Get indices of all spaces accessible that are not safe (valid neighbors + current Space).             ArrayList nearbyWalls = getNearbyWalls(this.getCurrentSpace());             validInputOptions = nearbyWalls;              //Build string to show.             string optionsToUser = "";              foreach (int index in nearbyWalls)             {                  if (index == 0)                 {                     optionsToUser += "Press 0 for the Wall on Top ";                 }                 else if (index == 1)                 {                     optionsToUser += " Press 1 for the Wall to Your Right";                 }                 else if (index == 2)                 {                     optionsToUser += " Press 2 for the Wall to the Bottom";                 }                 else if (index == 3)                 {                     optionsToUser += " Press 3 for the Wall to Your Left";                  }             }              GameConsole.instance.UpdateFeedback(optionsToUser);              isWaitingForInput = true;
            isChoppingWall = true;          }     }      private ArrayList getNearbyWalls(Space s)     {         ArrayList nearbyWalls = new ArrayList();         Wall[] wallArray = s.getWalls();          //Collect directions in which there is a wall         for (int i = 0; i < wallArray.Length; i++)         {             if (wallArray[i] != null)             {                 nearbyWalls.Add(i);             }         }         return nearbyWalls;     }

    public void treatVictim()
    {
        Space current = this.getCurrentSpace();

        if (treatedVictim != null)
        {
            GameConsole.instance.UpdateFeedback("You are already treating a victim!");
            return;
        }
        else
        {
            List<GameUnit> gameUnits = current.getOccupants();

            foreach (GameUnit gu in gameUnits)
            {
                //if has POI marker
                if (gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_POI)
                {
                    Victim v = gu.GetComponent<Victim>();
                    this.treatedVictim = v;
                    GameConsole.instance.UpdateFeedback("Treated victim successfully!");

                    object[] data = { this.currentSpace.indexX, this.currentSpace.indexY, PV.ViewID };
                    PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.UpdateTreatedVictimsState, data, sendToAllOptions, SendOptions.SendReliable);

                    return;
                }
            }
            GameConsole.instance.UpdateFeedback("There is no victim to be treated!");

        }
    } 
    public void carryVictim()       //if ambulance carrying victim: 0AP && Victim is carried by ambulance in experienced game TODO
    {
        //get current space
        Space current = this.getCurrentSpace();

        if (this.getVictim() != null)
        {
            GameConsole.instance.UpdateFeedback("You are already carrying a victim!");
            return;
        }
        else
        {
            List<GameUnit> gameUnits = current.getOccupants();

            foreach (GameUnit gu in gameUnits)
            {
                //if has POI marker
                if (gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_POI)
                {
                    Victim v = gu.GetComponent<Victim>();
                    this.setVictim(v);
                    GameConsole.instance.UpdateFeedback("Carried victim successfully!");

                    object[] data = { this.currentSpace.indexX, this.currentSpace.indexY, PV.ViewID };
                    PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.UpdateCarriedVictimsState, data, sendToAllOptions, SendOptions.SendReliable);

                    return;
                }
            }
            GameConsole.instance.UpdateFeedback("There is no victim to be carried!");
        }
    }
    public void carryHazmat() {
        Space current = this.getCurrentSpace();

        if (this.getHazmat() != null || this.getVictim() != null) {
            GameConsole.instance.UpdateFeedback("You are already carrying something!");
            return;
        }
        else {
            List<GameUnit> gameUnits = current.getOccupants();

            foreach (GameUnit gu in gameUnits) {
                //if has POI marker
                if (gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_HAZMAT) {
                    Hazmat h = gu.GetComponent<Hazmat>();
                    this.setHazmat(h);
                    GameConsole.instance.UpdateFeedback("Carried hazmat successfully!");

                    object[] data = { this.currentSpace.indexX, this.currentSpace.indexY, PV.ViewID };
                    PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.UpdateCarriedHazmatsState, data, sendToAllOptions, SendOptions.SendReliable);
                    return;
                }
            }
            GameConsole.instance.UpdateFeedback("There is no hazmat to be carried!");
        }
    }

    void driveAmbulance(int direction)
    {
        int xPos = 0;
        int yPos = 0;
        int newXPos = 0;
        int newYPos = 0;
        Ambulance h = this.getAmbulance();
        if (isOnAmbulance)
        {
            Space curr = this.getCurrentSpace();
            List<GameUnit> currGameUnits = curr.getOccupants();
            xPos = curr.indexX;
            yPos = curr.indexY;

            switch (direction)
            {

                case 1:
                    if (xPos == 0 && yPos == 5)
                    {
                        newXPos = 5;
                        newYPos = 0;
                    }
                    else if (xPos == 5 && yPos == 0)
                    {
                        newXPos = 9;
                        newYPos = 2;
                    }
                    else if (xPos == 9 && yPos == 2)
                    {
                        newXPos = 4;
                        newYPos = 7;
                    }
                    else if (xPos == 4 && yPos == 7)
                    {
                        newXPos = 0;
                        newYPos = 5;
                    }
                    break;
                case 3:
                    if (xPos == 0 && yPos == 5)
                    {
                        newXPos = 4;
                        newYPos = 7;
                    }
                    else if (xPos == 4 && yPos == 7)
                    {
                        newXPos = 9;
                        newYPos = 2;
                    }
                    else if (xPos == 9 && yPos == 2)
                    {
                        newXPos = 5;
                        newYPos = 0;
                    }
                    else if (xPos == 5 && yPos == 0)
                    {
                        newXPos = 0;
                        newYPos = 5;
                    }
                    break;
                case 2:
                    if (xPos == 0 && yPos == 5)
                    {
                        newXPos = 9;
                        newYPos = 2;
                    }
                    else if (xPos == 9 && yPos == 2)
                    {
                        newXPos = 0;
                        newYPos = 5;
                    }
                    else if (xPos == 5 && yPos == 0)
                    {
                        newXPos = 4;
                        newYPos = 7;
                    }
                    else if (xPos == 4 && yPos == 7)
                    {
                        newXPos = 5;
                        newYPos = 0;
                    }
                    break;
                default:
                    break;
            }

            Space destination = StateManager.instance.spaceGrid.getGrid()[newXPos, newYPos];
            Vector3 destinationPosition = new Vector3(destination.worldPosition.x, destination.worldPosition.y, -5);

            GameUnit ambulance = null;

            foreach(GameUnit gu in currGameUnits)
            {
                if(gu != null && gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_AMBULANCE)
                {
                    ambulance = gu;
                    break;
                }
            }

            List<Fireman> firemenInCurrentSpace = curr.getFiremen();

            foreach(Fireman fireman in firemenInCurrentSpace)
            {
                if(fireman.getAmbulance() != null)
                {
                    currGameUnits.Remove(fireman);
                    destination.addOccupant(fireman);

                    fireman.setCurrentSpace(destination);
                    fireman.GetComponent<Transform>().position = destinationPosition;
                }
            }

            //currGameUnits.Remove(this);
            currGameUnits.Remove(ambulance);

            //destination.addOccupant(this);
            destination.addOccupant(ambulance);

            //this.setCurrentSpace(destination);
            //this.GetComponent<Transform>().position = destinationPosition;
            if (h != null)
            {
                h.setCurrentSpace(destination);
                h.GetComponent<Transform>().position = destinationPosition;
            }
        }
        else
        {
            Space AmbulanceCurrentSpace = null;
            //find the space of the ambulance
            foreach (Space s in StateManager.instance.spaceGrid.getGrid())
            {
                foreach (GameUnit gu in s.getOccupants())
                {
                    if (gu != null && gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_AMBULANCE)
                    {
                        AmbulanceCurrentSpace = s;
                        break;
                    }
                }
            }


            if (AmbulanceCurrentSpace != null)
            {
                xPos = AmbulanceCurrentSpace.indexX;
                yPos = AmbulanceCurrentSpace.indexY;
            }


            switch (direction)
            {
                case 1:
                    if (xPos == 0 && yPos == 5)
                    {
                        newXPos = 5;
                        newYPos = 0;
                    }
                    else if (xPos == 5 && yPos == 0)
                    {
                        newXPos = 9;
                        newYPos = 2;
                    }
                    else if (xPos == 9 && yPos == 2)
                    {
                        newXPos = 4;
                        newYPos = 7;
                    }
                    else if (xPos == 4 && yPos == 7)
                    {
                        newXPos = 0;
                        newYPos = 5;
                    }
                    break;
                case 3:
                    if (xPos == 0 && yPos == 5)
                    {
                        newXPos = 4;
                        newYPos = 7;
                    }
                    else if (xPos == 4 && yPos == 7)
                    {
                        newXPos = 9;
                        newYPos = 2;
                    }
                    else if (xPos == 9 && yPos == 2)
                    {
                        newXPos = 5;
                        newYPos = 0;
                    }
                    else if (xPos == 5 && yPos == 0)
                    {
                        newXPos = 0;
                        newYPos = 5;
                    }
                    break;
                case 2:
                    if (xPos == 0 && yPos == 5)
                    {
                        newXPos = 9;
                        newYPos = 2;
                    }
                    else if (xPos == 9 && yPos == 2)
                    {
                        newXPos = 0;
                        newYPos = 5;
                    }
                    else if (xPos == 5 && yPos == 0)
                    {
                        newXPos = 4;
                        newYPos = 7;
                    }
                    else if (xPos == 4 && yPos == 7)
                    {
                        newXPos = 5;
                        newYPos = 0;
                    }
                    break;
                default:
                    break;
            }

            Space destination = StateManager.instance.spaceGrid.getGrid()[newXPos, newYPos];
            Vector3 destinationPosition = new Vector3(destination.worldPosition.x, destination.worldPosition.y, -5);
            List<GameUnit> currGameUnits = AmbulanceCurrentSpace.getOccupants();

            GameUnit ambulance = null;

            foreach (GameUnit gu in currGameUnits)
            {
                if (gu != null && gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_AMBULANCE)
                {
                    ambulance = gu;
                    break;
                }
            }

            List<Fireman> firemenInCurrentSpace = AmbulanceCurrentSpace.getFiremen();

            foreach (Fireman fireman in firemenInCurrentSpace)
            {
                if (fireman.getAmbulance() != null)
                {
                    currGameUnits.Remove(fireman);
                    destination.addOccupant(fireman);

                    fireman.setCurrentSpace(destination);
                    fireman.GetComponent<Transform>().position = destinationPosition;
                }
            }


            currGameUnits.Remove(ambulance);

            destination.addOccupant(ambulance);

            if (ambulance != null)
            {
                ambulance.setCurrentSpace(destination);
                ambulance.GetComponent<Transform>().position = destinationPosition;
            }
        
        }
    }

    void driveEngine(int direction)
    {
        //fireman has to be on the same space with engine
        int xPos = 0;
        int yPos = 0;
        int newXPos = 0;
        int newYPos = 0;
        Engine n = this.getEngine();
        if (isOnEngine)
        {
            Space curr = this.getCurrentSpace();
            List<GameUnit> currGameUnits = curr.getOccupants();
            xPos = curr.indexX;
            yPos = curr.indexY;

            switch (direction)
            {

                case 1:
                    if (xPos == 9 && yPos == 5)
                    {
                        newXPos = 2;
                        newYPos = 7;
                    }
                    else if (xPos == 2 && yPos == 7)
                    {
                        newXPos = 0;
                        newYPos = 1;
                    }
                    else if (xPos == 0 && yPos == 1)
                    {
                        newXPos = 8;
                        newYPos = 0;
                    }
                    else if (xPos == 8 && yPos == 0)
                    {
                        newXPos = 9;
                        newYPos = 5;
                    }
                    break;
                case 3:
                    if (xPos == 9 && yPos == 5)
                    {
                        newXPos = 8;
                        newYPos = 0;
                    }
                    else if (xPos == 8 && yPos == 0)
                    {
                        newXPos = 0;
                        newYPos = 1;
                    }
                    else if (xPos == 0 && yPos == 1)
                    {
                        newXPos = 2;
                        newYPos = 7;
                    }
                    else if (xPos == 2 && yPos == 7)
                    {
                        newXPos = 9;
                        newYPos = 5;
                    }
                    break;
                case 2:
                    if (xPos == 9 && yPos == 5)
                    {
                        newXPos = 0;
                        newYPos = 1;
                    }
                    else if (xPos == 0 && yPos == 1)
                    {
                        newXPos = 9;
                        newYPos = 5;
                    }
                    else if (xPos == 8 && yPos == 0)
                    {
                        newXPos = 2;
                        newYPos = 7;
                    }
                    else if (xPos == 2 && yPos == 7)
                    {
                        newXPos = 8;
                        newYPos = 0;
                    }
                    break;
                default:
                    break;
            }

            Space destination = StateManager.instance.spaceGrid.getGrid()[newXPos, newYPos];
            Vector3 destinationPosition = new Vector3(destination.worldPosition.x, destination.worldPosition.y, -5);

            GameUnit engine = null;

            foreach (GameUnit gu in currGameUnits)
            {
                if (gu != null && gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_ENGINE)
                {
                    engine = gu;
                    break;
                }
            }

            List<Fireman> firemenInCurrentSpace = curr.getFiremen();

            foreach (Fireman fireman in firemenInCurrentSpace)
            {
                if (fireman.getEngine() != null)
                {
                    currGameUnits.Remove(fireman);
                    destination.addOccupant(fireman);

                    fireman.setCurrentSpace(destination);
                    fireman.GetComponent<Transform>().position = destinationPosition;
                }
            }

            //currGameUnits.Remove(this);
            currGameUnits.Remove(engine);

            //destination.addOccupant(this);
            destination.addOccupant(engine);

            //this.setCurrentSpace(destination);
            //this.GetComponent<Transform>().position = destinationPosition;
            if (n != null)
            {
                n.setCurrentSpace(destination);
                n.GetComponent<Transform>().position = destinationPosition;
            }
        }
        else
        {
            GameConsole.instance.UpdateFeedback("You have to be on engine to drive!");
            return;
        }
    }
    void rideAmbulance() 
    {
        //get current space
        Space current = this.getCurrentSpace();

        if (this.getAmbulance() != null)
        {
            GameConsole.instance.UpdateFeedback("You are already on an ambulance!");
            return;
        }
        else
        {
            List<GameUnit> gameUnits = current.getOccupants();

            foreach (GameUnit gu in gameUnits)
            {
                if (gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_AMBULANCE)
                {
                    Ambulance h = gu.GetComponent<Ambulance>();
                    this.setAmbulance(h);
                    GameConsole.instance.UpdateFeedback("Riding ambulance successfully!");
                    isOnAmbulance = true;
                    return;
                }
            }
            GameConsole.instance.UpdateFeedback("There is no ambulance!");
        }
    }

    void rideEngine()
    {
        //get current space
        Space current = this.getCurrentSpace();

        if (this.getEngine() != null)
        {
            GameConsole.instance.UpdateFeedback("You are already on an engine!");
            return;
        }
        else
        {
            List<GameUnit> gameUnits = current.getOccupants();

            foreach (GameUnit gu in gameUnits)
            {
                if (gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_ENGINE)
                {
                    Engine n = gu.GetComponent<Engine>();
                    this.setEngine(n);
                    GameConsole.instance.UpdateFeedback("Riding engine successfully!");
                    isOnEngine = true;
                    return;
                }
            }
            GameConsole.instance.UpdateFeedback("There is no engine!");
        }

    }

    private void move(Hazmat hazmat, Space curr, Space dst) {
        SpaceStatus destinationSpaceStatus = dst.getSpaceStatus();

        SpaceKind destinationSpaceKind = dst.getSpaceKind();
        Vector3 newPosition = new Vector3(dst.worldPosition.x, dst.worldPosition.y, -10);

        if ((destinationSpaceStatus == SpaceStatus.Safe && destinationSpaceKind == SpaceKind.Indoor) || destinationSpaceStatus == SpaceStatus.Smoke) {
            moveFirefighter(curr, dst, 2, true);

            //update carried hazmat positions across the network
            object[] data = { curr.indexX, curr.indexY, dst.indexX, dst.indexY, PV.ViewID };
            PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.MoveCarriedHazmat, data, sendToAllOptions, SendOptions.SendReliable);
        }
        else if (destinationSpaceKind == SpaceKind.Outdoor) {     //carry victim outside the building
            moveFirefighter(curr, dst, 2, true);
            this.setHazmat(null);

            object[] data = { curr.indexX, curr.indexY, PV.ViewID };
            PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.RemoveHazmat, data, sendToAllOptions, SendOptions.SendReliable);
            GameConsole.instance.UpdateFeedback("You have successfully exposed of a hazmat");
        }
        else //Fire
        {
            //can not carry victim
            GameConsole.instance.UpdateFeedback("Cannot carry a hazmat onto fire!");
            return;
        }
    }


    private void move(Victim v, Space curr, Space dst) {
        SpaceStatus destinationSpaceStatus = dst.getSpaceStatus();

        SpaceKind destinationSpaceKind = dst.getSpaceKind();
        Vector3 newPosition = new Vector3(dst.worldPosition.x, dst.worldPosition.y, -10);

        bool isAmbulanceOnDest = false;

        foreach (GameUnit gu in dst.getOccupants())
        {
            if (gu != null && gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_AMBULANCE)
            {
                isAmbulanceOnDest = true;
                break;
            }
        }
        Victim carried = this.carriedVictim;
        Victim treated = this.treatedVictim;

        if ((GameManager.GM.isFamilyGame && destinationSpaceKind == SpaceKind.Outdoor) 
        || (!GameManager.GM.isFamilyGame && isAmbulanceOnDest)) {     //carry victim outside the building

            if(carried == v){
                if(this.spec == Specialist.RescueDog)
                {
                    if (AP >= 4)
                    {
                        moveFirefighter(curr, dst, 4, true);
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("Not enough AP");
                    }
                }
                else
                {
                    if (AP >= 2)
                    {
                        moveFirefighter(curr, dst, 2, true);
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("Not enough AP");
                    }
                }
                this.setVictim(null);
            }
            if(treated == v)
            {
                moveFirefighter(curr, dst, 0, true); 
                this.treatedVictim = null;
            }

            object[] data = { curr.indexX, curr.indexY, PV.ViewID, true};
            PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.RemoveVictim, data, sendToAllOptions, SendOptions.SendReliable);
            PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.RemoveTreatedVictim, data, sendToAllOptions, SendOptions.SendReliable);

            GameConsole.instance.UpdateFeedback("You have successfully rescued a victim");

            //check if we won the game.
            if (GameManager.savedVictims >= 7) {
                //check for a perfect game
                if (GameManager.savedVictims == 10) {
                    GameManager.GameWon();
                    GameObject.Find("/Canvas/GameWonUIPanel/ContinuePlayingButton").SetActive(false);
                }
                if (!GameWonUI.isCalled) {
                    GameManager.GameWon();
                }
            }
            return;
        }
        else if ((destinationSpaceStatus == SpaceStatus.Safe && destinationSpaceKind == SpaceKind.Indoor)
                    || destinationSpaceStatus == SpaceStatus.Smoke
                    || (!GameManager.GM.isFamilyGame && destinationSpaceKind == SpaceKind.Outdoor)) {
            if (carried == v)
            {
                if (this.spec == Specialist.RescueDog)
                {
                    if (AP >= 4)
                    {
                        moveFirefighter(curr, dst, 4, true);
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("Not enough AP");
                    }
                }
                else
                {
                    if (AP >= 2)
                    {
                        moveFirefighter(curr, dst, 2, true);
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("Not enough AP");
                    }
                }
            }
            if (treated == v)
            {
                moveFirefighter(curr, dst, 0, true);
            }

            //update carried victim positions across the network
            object[] data = { curr.indexX, curr.indexY, dst.indexX, dst.indexY, PV.ViewID };
            PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.MoveCarriedVictim, data, sendToAllOptions, SendOptions.SendReliable);
            PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.MoveTreatedVictim, data, sendToAllOptions, SendOptions.SendReliable);
        }
        else //Fire
        {
            //can not carry victim
            GameConsole.instance.UpdateFeedback("Cannot carry a victim onto fire!");
            return;
        }
    }

    public void moveRescueDog(int direction) {
        Space curr = this.getCurrentSpace();
        Space[] neighbors = StateManager.instance.spaceGrid.GetRescueDogNeighbours(curr);
        Space destination = neighbors[direction];

        if (destination == null)
        {
            GameConsole.instance.UpdateFeedback("Invalid move. Please try again");
            return;
        }

        if(destination.getSpaceStatus() == SpaceStatus.Fire)
        {
            GameConsole.instance.UpdateFeedback("Rescue dog cannot move to a fire!");
            return;
        }

        Vector3 newPosition = new Vector3(destination.worldPosition.x, destination.worldPosition.y, -10);
        Space newSpace = StateManager.instance.spaceGrid.WorldPointToSpace(newPosition);

        if(AP < 1)
        {
            GameConsole.instance.UpdateFeedback("Not enough AP to move dawg");
        }
        else
        {
            moveFirefighter(curr, destination, 1, true);
        }

    }


    public static void moveFirefighter(Fireman fireman, Space curr, Space dst) {
        fireman.moveFirefighter(curr, dst, 0, false);
    }
    
    private void moveFirefighter(Space curr, Space dst, int apCost, bool isMyOwn) {
        Vector3 newPosition = new Vector3(dst.worldPosition.x, dst.worldPosition.y, -10);

        this.setCurrentSpace(dst);

        if (this.spec == Specialist.RescueSpecialist && this.moveAP >= apCost)
        {
            this.moveAP = this.moveAP - apCost;
            if (isMyOwn)
            {
                FiremanUI.instance.SetSpecialistAP(this.moveAP);
            }
        }
        else
        {
            this.decrementAP(apCost);
        }
        
        this.GetComponent<Transform>().position = newPosition;

        dst.addOccupant(this);
        curr.removeOccupant(this);

        object[] data = new object[] {dst.indexX, dst.indexY, PV.ViewID };
        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.UpdateSpaceReferenceToFireman, data, sendToAllOptions, SendOptions.SendReliable);


        if (isMyOwn) {
            FiremanUI.instance.SetAP(this.AP);
            //PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.UpdateSpaceReferenceToFireman, data, sendToAllOptions, SendOptions.SendReliable);
        }
    }

    public void move(int direction) {

        int ap = this.getAP();
        Victim v = this.getVictim();
        Victim t = treatedVictim;
        Hazmat hazmat = this.getHazmat();
        Ambulance a = this.getAmbulance();
        Engine n = this.getEngine();
        Space curr = this.getCurrentSpace();
        Space[] neighbors = StateManager.instance.spaceGrid.GetNeighbours(curr);
        Space destination = neighbors[direction];

        if (destination == null) {
            GameConsole.instance.UpdateFeedback("Invalid move. Please try again");
            return;
        }

        if(this.spec == Specialist.RescueDog && destination.getSpaceStatus() == SpaceStatus.Fire)
        {
            GameConsole.instance.UpdateFeedback("Rescue Dog cannot move to a place with fire");
            return;
        }

        SpaceStatus sp = destination.getSpaceStatus();

        Vector3 newPosition = new Vector3(destination.worldPosition.x, destination.worldPosition.y, -10);
        Space newSpace = StateManager.instance.spaceGrid.WorldPointToSpace(newPosition);

        if (sp == SpaceStatus.Fire && !(this.spec == Specialist.RescueDog)) 
        {
            if (ap >= 3 && v == null && t == null) //&&f has enough to move
            {
                moveFirefighter(curr, destination, 2, true);
            }
            else if(ap >= 3 && (v != null || t != null)) //cannot carry a victim into a fire
            {
                GameConsole.instance.UpdateFeedback("Cannot carry a victim into a fire");
                return;
            }
            else {
                GameConsole.instance.UpdateFeedback("Insufficient AP");
                return;
            }
        }
        else if(movedEngine != null || movedAmbulance != null) {
                GameConsole.instance.UpdateFeedback("You cannot move while being in the vehicle. Exit the vehicle by pressing X");
                return;
        }
        else {


            if (v == null && hazmat == null && ap >= 1) //t does not have to be null because moving a treated victim does not incur AP costs
            {
                moveFirefighter(curr, destination, 1, true);

                //flip poi
                List<GameUnit> gameUnits = destination.getOccupants();
                foreach (GameUnit gu in gameUnits)
                {
                    if (gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_POI)
                    {
                        if (gu.GetComponent<POI>().getIsFlipped() == false)
                        {
                            GameManager.FlipPOI(destination);
                            break;
                        }
                    }
                }
            }

            //else if (v != null && ap >= 2)//if the fireman is carrying a victim
            //{
            //    this.move(v, curr, destination);
            //}
            //else if (hazmat != null && ap >= 2) {
            //    this.move(hazmat, curr, destination);
            //}
            //else {
            //    GameConsole.instance.UpdateFeedback("Insufficient AP 4");
            //    return;
            //}


            if (v != null)//if the fireman is carrying a victim
            {
                if(this.spec == Specialist.RescueDog && ap>=4)
                {
                    this.move(v, curr, destination);

                    //flip poi
                    List<GameUnit> gameUnits = destination.getOccupants();
                    foreach (GameUnit gu in gameUnits)
                    {
                        if (gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_POI)
                        {
                            if (gu.GetComponent<POI>().getIsFlipped() == false)
                            {
                                GameManager.FlipPOI(destination);
                                break;
                            }
                        }
                    }
                }
                else if(ap >= 2)
                {
                    this.move(v, curr, destination);

                    //flip poi
                    List<GameUnit> gameUnits = destination.getOccupants();
                    foreach (GameUnit gu in gameUnits)
                    {
                        if (gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_POI)
                        {
                            if (gu.GetComponent<POI>().getIsFlipped() == false)
                            {
                                GameManager.FlipPOI(destination);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    GameConsole.instance.UpdateFeedback("Insufficient AP");
                    return;
                }
            }
            else if (hazmat != null) {
                if (ap >= 2)
                {
                    this.move(hazmat, curr, destination);
                    //flip poi
                    List<GameUnit> gameUnits = destination.getOccupants();
                    foreach (GameUnit gu in gameUnits)
                    {
                        if (gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_POI)
                        {
                            if (gu.GetComponent<POI>().getIsFlipped() == false)
                            {
                                GameManager.FlipPOI(destination);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    GameConsole.instance.UpdateFeedback("Insufficient AP");
                    return;
                }
            }

            if (t != null){
                if(ap >= 1)
                {
                    this.move(t, curr, destination);

                    //flip poi
                    List<GameUnit> gameUnits = destination.getOccupants();
                    foreach (GameUnit gu in gameUnits)
                    {
                        if (gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_POI)
                        {
                            if (gu.GetComponent<POI>().getIsFlipped() == false)
                            {
                                GameManager.FlipPOI(destination);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    GameConsole.instance.UpdateFeedback("Insufficient AP");
                    return;

                }
            }
        }
    }

    private ArrayList getNearbyDamagedWalls(Space s)
    {
        ArrayList nearbyWalls = new ArrayList();
        Wall[] wallArray = s.getWalls();

        //Collect directions in which there is a wall
        for (int i = 0; i < wallArray.Length; i++)
        {
            if (wallArray[i] != null && wallArray[i].getWallStatus() == WallStatus.Damaged)
            {
                nearbyWalls.Add(i);
            }
        }
        return nearbyWalls;
    }


    public void squeeze()
    {
        if (this.getVictim() == null)
        {
            int numAP = getAP(); //returns the number of action point

            //Check if sufficient AP.
            if (numAP < 2)
            {
                Debug.Log("Not enough AP!");  //Used to show the player why he can’t perform an action in case of failure
                GameConsole.instance.UpdateFeedback("Not enough AP!");
            }
            else
            {
                //Get indices of all spaces accessible that are not safe (valid neighbors + current Space).
                ArrayList nearbyDamagedWalls = getNearbyDamagedWalls(this.getCurrentSpace());
                validInputOptions = nearbyDamagedWalls;

                //Build string to show.
                string optionsToUser = "";

                foreach (int index in nearbyDamagedWalls)
                {

                    if (index == 0)
                    {
                        optionsToUser += "Press 0 to squeeze though the Wall on Top ";
                    }
                    else if (index == 1)
                    {
                        optionsToUser += " Press 1 to squeeze though the Wall to Your Right";
                    }
                    else if (index == 2)
                    {
                        optionsToUser += " Press 2 to squeeze though the Wall to the Bottom";
                    }
                    else if (index == 3)
                    {
                        optionsToUser += " Press 3 to squeeze though the Wall to Your Left";

                    }
                }

                GameConsole.instance.UpdateFeedback(optionsToUser);

                isWaitingForInput = true;
                isSqueezing = true;
            }
        }
        else
        {
            GameConsole.instance.UpdateFeedback("You cannot squeeze through a wall with a victim!");
        }
    }


    private void sendFireMarkerExtinguishEvent(Space targetSpace)
    {
        int targetX = targetSpace.indexX;
        int targetY = targetSpace.indexY;

        object[] data = new object[] { targetSpace.indexX, targetSpace.indexY };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.RemoveFireMarker, data, GameManager.sendToAllOptions, SendOptions.SendReliable);
    }

    private void sendSmokeMarkerExtinguishEvent(Space targetSpace)
    {
        int targetX = targetSpace.indexX;
        int targetY = targetSpace.indexY;

        object[] data = new object[] { targetSpace.indexX, targetSpace.indexY };
    

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.RemoveSmokeMarker, data, GameManager.sendToAllOptions, SendOptions.SendReliable);
        
    }

    private void sendTurnFireMarkerToSmokeEvent(Space targetSpace)
    {
        object[] dataRemoveFireMarker = new object[] { targetSpace.indexX, targetSpace.indexY };
        object[] dataAdvanceSmokeMarker = new object[] { targetSpace.worldPosition, targetSpace.indexX, targetSpace.indexY };
        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.RemoveFireMarker, dataRemoveFireMarker, GameManager.sendToAllOptions, SendOptions.SendReliable);
        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.AdvanceSmokeMarker, dataAdvanceSmokeMarker, GameManager.sendToAllOptions, SendOptions.SendReliable);
    }


    private void sendChopWallEvent(Space targetSpace, int direction)
    {
        int indexX = targetSpace.indexX;
        int indexY = targetSpace.indexY;

        object[] data = new object[] { indexX, indexY, direction };
        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.ChopWall, data, GameManager.sendToAllOptions, SendOptions.SendReliable);
    }

    private void sendDriveAmbulanceEvent(int direction)
    {
        object[] data = { direction , PV.ViewID };
        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.DriveAmbulance, data, sendToAllOptions, SendOptions.SendReliable);
    }
    private void sendDriveEngineEvent(int direction)
    {
        object[] data = { direction, PV.ViewID };
        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.DriveEngine, data, sendToAllOptions, SendOptions.SendReliable);
    }
    private void sendRideAmbulanceEvent(int direction)
    {
        object[] data = { direction, PV.ViewID };
        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.RideAmbulance, data, sendToAllOptions, SendOptions.SendReliable);
    }
    private void sendRideEngineEvent(int direction)
    {
        object[] data = { direction, PV.ViewID };
        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.RideEngine, data, sendToAllOptions, SendOptions.SendReliable);
    }

    private void sendChangeCrewEvent(int[] updatedIndexList)
    {
        object[] data = new object[] { updatedIndexList };
        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.ChangeCrew, data, GameManager.sendToAllOptions, SendOptions.SendReliable);
    }

    public void endTurn()
    {
        SpaceStatus currentSpaceStatus = currentSpace.getSpaceStatus();
        if(currentSpaceStatus == SpaceStatus.Fire)
        {
            GameConsole.instance.UpdateFeedback("You cannot end your turn on a Fire Location");
            return;
        }
        restoreAP();
        GameManager.advanceFire();

        if (GameManager.GM.isFamilyGame)
        {
            GameManager.replenishPOI();
        }
        else
        {
            GameManager.replenishPOIExperienced();
        }
        GameManager.IncrementTurn();
    }


    private void restoreAP()
    {

        int currentNumAP = this.getAP();
        int newAP;

        if (this.spec == Specialist.FamilyGame)
        {
            newAP = Mathf.Min(currentNumAP + 4, 8);
            this.setAP(newAP);
            startOfTurnAP = newAP;
            FiremanUI.instance.SetAP(newAP);
            this.commandAP = 0;
            this.extinguishAP = 0;
            this.moveAP = 0;
            FiremanUI.instance.SetSpecialistAP(0);
        }
        else if (this.spec == Specialist.Paramedic)
        {
            newAP = Mathf.Min(currentNumAP + 4, 8);
            this.setAP(newAP);
            startOfTurnAP = newAP;
            FiremanUI.instance.SetAP(this.AP);
            this.commandAP = 0;
            this.extinguishAP = 0;
            this.moveAP = 0;
            FiremanUI.instance.SetSpecialistAP(0);
        }
        else if (this.spec == Specialist.FireCaptain)
        {
            newAP = Mathf.Min(currentNumAP + 4, 8);
            this.setAP(newAP);
            startOfTurnAP = newAP;
            FiremanUI.instance.SetAP(this.AP);
            this.commandAP = 2;
            this.extinguishAP = 0;
            this.moveAP = 0;
            FiremanUI.instance.SetSpecialistAP(2);
        }
        else if (this.spec == Specialist.ImagingTechnician)
        {
            newAP = Mathf.Min(currentNumAP + 4, 8);
            this.setAP(newAP);
            startOfTurnAP = newAP;
            FiremanUI.instance.SetAP(this.AP);
            this.commandAP = 0;
            this.extinguishAP = 0;
            this.moveAP = 0;
            FiremanUI.instance.SetSpecialistAP(0);
        }
        else if (this.spec == Specialist.CAFSFirefighter)
        {
            newAP = Mathf.Min(currentNumAP + 3, 8);
            this.setAP(newAP);
            startOfTurnAP = newAP;
            FiremanUI.instance.SetAP(this.AP);
            this.commandAP = 0;
            this.extinguishAP = 3;
            this.moveAP = 0;
            FiremanUI.instance.SetSpecialistAP(3);
        }
        else if (this.spec == Specialist.HazmatTechinician)
        {
            newAP = Mathf.Min(currentNumAP + 4, 8);
            this.setAP(newAP);
            startOfTurnAP = newAP;
            FiremanUI.instance.SetAP(this.AP);
            this.commandAP = 0;
            this.extinguishAP = 0;
            this.moveAP = 0;
            FiremanUI.instance.SetSpecialistAP(0);
        }
        else if (this.spec == Specialist.Generalist)
        {
            newAP = Mathf.Min(currentNumAP + 5, 8);
            this.setAP(newAP);
            startOfTurnAP = newAP;
            FiremanUI.instance.SetAP(this.AP);
            this.commandAP = 0;
            this.extinguishAP = 0;
            this.moveAP = 0;
            FiremanUI.instance.SetSpecialistAP(0);
        }
        else if (this.spec == Specialist.RescueSpecialist)
        {
            newAP = Mathf.Min(currentNumAP + 4, 8);
            this.setAP(newAP);
            startOfTurnAP = newAP;
            FiremanUI.instance.SetAP(this.AP);
            this.commandAP = 0;
            this.extinguishAP = 0;
            this.moveAP = 3;
            FiremanUI.instance.SetSpecialistAP(3);
        }
        else if (this.spec == Specialist.DriverOperator)
        {
            newAP = Mathf.Min(currentNumAP + 4, 8);
            this.setAP(newAP);
            startOfTurnAP = newAP;
            FiremanUI.instance.SetAP(this.AP);
            this.commandAP = 0;
            this.extinguishAP = 0;
            this.moveAP = 0;
            FiremanUI.instance.SetSpecialistAP(0);
        }
        else if (this.spec == Specialist.Veteran) //TODO
        {
            newAP = Mathf.Min(currentNumAP + 4, 8);
            this.setAP(newAP);
            startOfTurnAP = newAP;
            FiremanUI.instance.SetAP(this.AP);
            this.commandAP = 0;
            this.extinguishAP = 0;
            this.moveAP = 0;
            FiremanUI.instance.SetSpecialistAP(0);
        }
        else if (this.spec == Specialist.RescueDog) 
        {
            newAP = Mathf.Min(currentNumAP + 12, 16);
            this.setAP(newAP);
            startOfTurnAP = newAP;
            FiremanUI.instance.SetAP(this.AP);
            this.commandAP = 0;
            this.extinguishAP = 0;
            this.moveAP = 0;
            FiremanUI.instance.SetSpecialistAP(0);
        }
    }

    private void decrementChopWallAP()
    {
        Specialist s = this.GetSpecialist();
        if (s == Specialist.RescueSpecialist)
        {
            this.setAP(this.getAP() - 1);
        }
        else
        {
            this.setAP(this.getAP() - 2);
        }
    }

    private void decrementRemoveSmokeAP()
    {
        Specialist s = this.GetSpecialist();
        if (s == Specialist.RescueSpecialist || s == Specialist.Paramedic)
        {
            this.setAP(this.getAP() - 2); //double AP
        }
        else
        {
            if(this.spec == Specialist.CAFSFirefighter && extinguishAP >= 1)
            {
                extinguishAP--;
                FiremanUI.instance.SetSpecialistAP(extinguishAP);
            }
            else
            {
                this.setAP(this.getAP() - 1);
            }
        }
    }

    private void decrementFireToSmokeAP()
    {
        Specialist s = this.GetSpecialist();
        if (s == Specialist.RescueSpecialist || s == Specialist.Paramedic)
        {
            this.setAP(this.getAP() - 2); //double AP
        }
        else
        {
            if (this.spec == Specialist.CAFSFirefighter && extinguishAP >= 1)
            {
                extinguishAP--;
                FiremanUI.instance.SetSpecialistAP(extinguishAP);
            }
            else
            {
                this.setAP(this.getAP() - 1);
            }
        }
    }

    private void decrementRemoveFireAP()
    {
        Specialist s = this.GetSpecialist();
        if (s == Specialist.RescueSpecialist || s == Specialist.Paramedic)
        {
            this.setAP(this.getAP() - 4); //double AP
        }
        else
        {
            if (this.spec == Specialist.CAFSFirefighter && extinguishAP >= 2)
            {
                extinguishAP = extinguishAP - 2;
                FiremanUI.instance.SetSpecialistAP(extinguishAP);
            }
            else
            {
                this.setAP(this.getAP() - 2);
            }
        }
    }


    public void changeCrew() //return the index of the specialist we want
    {
        if(this.getAP() - startOfTurnAP != 0)
        {
            GameConsole.instance.UpdateFeedback("Crew change can only be the first move of the turn!");
            return;
        }
        if (this.getAP() >= 2)
        {

            string optionsToUser =  "";
            Specialist[] test = GameManager.GM.availableSpecialists;

            for (int i = 0; i< GameManager.GM.freeSpecialistIndex.Length; i++)
            {
                if (GameManager.GM.freeSpecialistIndex[i] != 0)
                {
                    optionsToUser = optionsToUser + "Press " + i + " for " + GameManager.GM.availableSpecialists[i] + ". ";
                }
            }

            GameConsole.instance.UpdateFeedback(optionsToUser);
            isWaitingForInput = true;
            isChangingCrew = true;
        }
    }

    public void FlipPOI () {
        Debug.Log("Flip");
        string[] mylist = new string[] {
            "man POI", "woman POI", "false alarm", "dog POI"
        };
        Debug.Log("Length of array is :" + mylist.Length);
        Space curr = this.getCurrentSpace();
        List<GameUnit> gameUnits = curr.getOccupants();
        GameUnit questionMark = null;
        foreach (GameUnit gu in gameUnits)
        {
            if(gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_POI){
                questionMark = gu;
                break;
            }
        }
        Vector3 position = new Vector3(curr.worldPosition.x,curr.worldPosition.y,-5);
        int r;

        while(true)
        {   
            r = Random.Range(0, mylist.Length - 1);
            if(string.Compare(mylist[r],"false alarm") == 0 && GameManager.NumFA <= 0)
                continue;
            else
            {
                if (GameManager.numVictim <= 0)
                    continue;
            }
            break;
        }

        string POIname = mylist[r];
        if(string.Compare(POIname,"false alarm") == 0)
        {
            GameManager.NumFA--;
            gameUnits.Remove(questionMark);
            Destroy(questionMark.physicalObject);
            Destroy(questionMark);
            GameConsole.instance.UpdateFeedback("It was a false alarm!");
            Debug.Log("After revealing FalseAlarm, numFa is: " + GameManager.NumFA);
            GameManager.numOfActivePOI--;
            return;
        }
        else
        {
            GameConsole.instance.UpdateFeedback("It was a Victim!");
            GameManager.numVictim--;
            Debug.Log("After revealing Victim, numVictim is: " + GameManager.numVictim);
        }
        //Instiate Object
        GameObject poi = Instantiate (Resources.Load ("PhotonPrefabs/Prefabs/POIs/" + POIname ) as GameObject);

        poi.GetComponent<POI>().setPOIKind(POIKind.Victim);
        poi.GetComponent<POI>().setIsFlipped(true);
        poi.GetComponent<Transform>().position = position;
        poi.GetComponent<GameUnit>().setCurrentSpace(curr);
        poi.GetComponent<GameUnit>().setType(FlashPointGameConstants.GAMEUNIT_TYPE_POI);
        poi.GetComponent<GameUnit>().setPhysicalObject(poi);

        //NOT DONE : poi.GetComponent<Victim>().setPhysicalObject(poi)
        gameUnits.Remove(questionMark);
        curr.addOccupant(poi.GetComponent<GameUnit>());
        Destroy(questionMark.physicalObject);
        Destroy(questionMark);
        
    }

    public void identifyPOI()
    {
        if(!GameManager.GM.isFamilyGame && this.spec == Specialist.ImagingTechnician)
        {
            GameConsole.instance.UpdateFeedback("Click anywhere on the board to flip a POI.");
            isWaitingForInput = true;
            isIdentifyingPOI = true;
        }
        else
        {
            GameConsole.instance.UpdateFeedback("You can't do this move! You have to be an imaging technician.");
        }
    }

    private ArrayList getNearbySpaces(Space s)
    {
        ArrayList nearbySpaces = new ArrayList();
        Space[] neighbors = StateManager.instance.spaceGrid.GetNeighbours(this.getCurrentSpace());

        //Collect directions in which there is a wall
        for (int i = 0; i < neighbors.Length; i++)
        {
            if (neighbors[i] != null)
            {
                nearbySpaces.Add(i);
            }
        }
        return nearbySpaces;
    }


    public void revealPOI()
    {
        if (!GameManager.GM.isFamilyGame && this.spec == Specialist.RescueDog)
        {
            //Get indices of all spaces accessible that are not safe (valid neighbors + current Space).
            ArrayList nearbySpaces = getNearbySpaces(this.getCurrentSpace());
            validInputOptions = nearbySpaces;

            //Build string to show.
            string optionsToUser = "";

            foreach (int index in nearbySpaces)
            {

                if (index == 0)
                {
                    optionsToUser += "Press 0 for the Space on Top ";
                }
                else if (index == 1)
                {
                    optionsToUser += " Press 1 for the Space to Your Right";
                }
                else if (index == 2)
                {
                    optionsToUser += " Press 2 for the Space to the Bottom";
                }
                else if (index == 3)
                {
                    optionsToUser += " Press 3 for the Space to Your Left";

                }
            }

            GameConsole.instance.UpdateFeedback(optionsToUser);

            isWaitingForInput = true;
            isRevealingPOI = true;
        }
        else
        {
            GameConsole.instance.UpdateFeedback("You can't do this move! You have to be a Rescue Dog.");
        }
    }

    public void command()
    {
        if (!GameManager.GM.isFamilyGame && this.spec == Specialist.FireCaptain)
        {
            GameConsole.instance.UpdateFeedback("Click on any firefighter to command it.");
            isWaitingForInput = true;
            isClickingFirefighter = true;
        }
        else
        {
            GameConsole.instance.UpdateFeedback("You can't do this move! You have to be a fire captain.");
        }
    }

    //  =============== NETWORK SYNCRONIZATION SECTION ===============
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

        //Move = 5
        if (evCode == (byte)PhotonEventCodes.Move) {
            Debug.Log("move on event!");
            object[] data = eventData.CustomData as object[];
            int direction = (int)data[1];

            if ((int)data[0] == PV.ViewID) {
                move(direction);
            }

        }
        else if(evCode == (byte)PhotonEventCodes.MoveRescueDog)
        {
            object[] data = eventData.CustomData as object[];
            int direction = (int)data[1];
            if ((int)data[0] == PV.ViewID)
            {
                moveRescueDog(direction);
            }
        }
        else if (evCode == (byte)PhotonEventCodes.PickSpecialist) {
            GameManager.GM.Turn = 1;
            GameManager.GameStatus = FlashPointGameConstants.GAME_STATUS_PICK_SPECIALIST;
            GameManager.GM.DisplayPlayerTurn();
            GameUI.instance.AddGameState(GameManager.GameStatus);
            selectSpecialist();
        }
        else if (evCode == (byte)PhotonEventCodes.UpdateCarriedVictimsState) { //0: indexX, 1: indexY, 2: index in state dictionary/fireman unique network id
            object[] dataReceived = eventData.CustomData as object[];
            int indexX = (int)dataReceived[0];
            int indexY = (int)dataReceived[1];
            int firemanId = (int)dataReceived[2];

            Space space = StateManager.instance.spaceGrid.grid[indexX, indexY];
            Victim victim = null;
            foreach (GameUnit gu in space.getOccupants()) {
                //TODO check if victim is carried by another fireman after drop functionality is implemented
                if (gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_POI) {
                    Victim v = gu.GetComponent<Victim>();
                    victim = v;
                }
            }

            Dictionary<int, Victim> d = StateManager.instance.firemanCarriedVictims;
            if (d.ContainsKey(firemanId)) {
                d[firemanId] = victim;
            }
            else d.Add(firemanId, victim);

        }
        else if (evCode == (byte)PhotonEventCodes.UpdateTreatedVictimsState)
        { //0: indexX, 1: indexY, 2: index in state dictionary/fireman unique network id
            object[] dataReceived = eventData.CustomData as object[];
            int indexX = (int)dataReceived[0];
            int indexY = (int)dataReceived[1];
            int firemanId = (int)dataReceived[2];

            Space space = StateManager.instance.spaceGrid.grid[indexX, indexY];
            Victim victim = null;
            foreach (GameUnit gu in space.getOccupants())
            {
                if (gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_POI)
                {
                    Victim v = gu.GetComponent<Victim>();
                    victim = v;
                }
            }

            Dictionary<int, Victim> d = StateManager.instance.firemanTreatedVictims;
            if (d.ContainsKey(firemanId))
            {
                d[firemanId] = victim;
            }
            else d.Add(firemanId, victim);

        }

        else if (evCode == (byte)PhotonEventCodes.UpdateCarriedHazmatsState) { //0: indexX, 1: indexY, 2: index in state dictionary/fireman unique network id
            object[] dataReceived = eventData.CustomData as object[];
            int indexX = (int)dataReceived[0];
            int indexY = (int)dataReceived[1];
            int firemanId = (int)dataReceived[2];

            Space space = StateManager.instance.spaceGrid.grid[indexX, indexY];
            Hazmat hazmat = null;
            foreach (GameUnit gu in space.getOccupants()) {
                //TODO check if victim is carried by another fireman after drop functionality is implemented
                if (gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_HAZMAT) {
                    Hazmat h = gu.GetComponent<Hazmat>();
                    hazmat = h;
                }
            }

            Dictionary<int, Hazmat> d = StateManager.instance.firemanCarriedHazmats;
            if (d.ContainsKey(firemanId)) {
                d[firemanId] = hazmat;
            }
            else d.Add(firemanId, hazmat);

        }
        //0: current space X, 1: current space Y, 2: destination X, 3: dst Y, 4: fireman PV.ViewId
        else if(evCode == (byte) PhotonEventCodes.MoveCarriedVictim) {
            object[] dataReceived = eventData.CustomData as object[];
            Space curr = StateManager.instance.spaceGrid.grid[(int)dataReceived[0], (int)dataReceived[1]];
            Space dst = StateManager.instance.spaceGrid.grid[(int)dataReceived[2], (int)dataReceived[3]];
            int firemanId = (int)dataReceived[4];

            Victim victim = StateManager.instance.firemanTreatedVictims[firemanId];
            //victim.carried = true;

            //update victim andspace references
            victim.setCurrentSpace(dst);
            victim.transform.position = new Vector3(dst.worldPosition.x, dst.worldPosition.y, -10);

            dst.addOccupant(victim);
            curr.removeOccupant(victim);
        }
        else if (evCode == (byte)PhotonEventCodes.MoveTreatedVictim)
        {
            object[] dataReceived = eventData.CustomData as object[];
            Space curr = StateManager.instance.spaceGrid.grid[(int)dataReceived[0], (int)dataReceived[1]];
            Space dst = StateManager.instance.spaceGrid.grid[(int)dataReceived[2], (int)dataReceived[3]];
            int firemanId = (int)dataReceived[4];

            Victim victim = StateManager.instance.firemanCarriedVictims[firemanId];
            //victim.carried = true;

            //update victim andspace references
            victim.setCurrentSpace(dst);
            victim.transform.position = new Vector3(dst.worldPosition.x, dst.worldPosition.y, -10);

            dst.addOccupant(victim);
            curr.removeOccupant(victim);
        }
        else if (evCode == (byte)PhotonEventCodes.MoveCarriedHazmat) {
            object[] dataReceived = eventData.CustomData as object[];
            Space curr = StateManager.instance.spaceGrid.grid[(int)dataReceived[0], (int)dataReceived[1]];
            Space dst = StateManager.instance.spaceGrid.grid[(int)dataReceived[2], (int)dataReceived[3]];
            int firemanId = (int)dataReceived[4];

            Hazmat hazmat = StateManager.instance.firemanCarriedHazmats[firemanId];
            //hazmat.carried = true;

            //update victim andspace references
            hazmat.setCurrentSpace(dst);
            hazmat.transform.position = new Vector3(dst.worldPosition.x, dst.worldPosition.y, -10);

            dst.addOccupant(hazmat);
            curr.removeOccupant(hazmat);
        }
        //0: indexX, 1: indexY, 2: fireman PV.viewId, 3: true for saved, false for lost
        else if (evCode == (byte) PhotonEventCodes.RemoveVictim) {
            //parse data
            object[] dataReceived = eventData.CustomData as object[];
            Space curr = StateManager.instance.spaceGrid.grid[(int)dataReceived[0], (int)dataReceived[1]];
            int firemanId = (int)dataReceived[2];
            bool saved = (bool) dataReceived[3];

            Victim victim = StateManager.instance.firemanCarriedVictims[firemanId];

            //update game states
            curr.removeOccupant(victim);
            Destroy(victim.gameObject);
            Destroy(victim);
            StateManager.instance.firemanCarriedVictims.Remove(firemanId);

            //update UI
            if(saved) {
                GameManager.numOfActivePOI--;
                GameManager.savedVictims++;
                GameUI.instance.AddSavedVictim();
            }
            else {
                GameManager.numOfActivePOI--;
                GameManager.lostVictims++;
                GameUI.instance.AddLostVictim();
                GameManager.numVictim--;
                GameConsole.instance.UpdateFeedback("A victim just perished.");
            }

        }
        else if (evCode == (byte)PhotonEventCodes.RemoveTreatedVictim)
        {
            //parse data
            object[] dataReceived = eventData.CustomData as object[];
            Space curr = StateManager.instance.spaceGrid.grid[(int)dataReceived[0], (int)dataReceived[1]];
            int firemanId = (int)dataReceived[2];
            bool saved = (bool)dataReceived[3];

            Victim victim = StateManager.instance.firemanTreatedVictims[firemanId];

            //update game states
            curr.removeOccupant(victim);
            Destroy(victim.gameObject);
            Destroy(victim);
            StateManager.instance.firemanTreatedVictims.Remove(firemanId);

            //update UI
            if (saved)
            {
                GameManager.numOfActivePOI--;
                GameManager.savedVictims++;
                GameUI.instance.AddSavedVictim();
            }
            else
            {
                GameManager.numOfActivePOI--;
                GameManager.lostVictims++;
                GameUI.instance.AddLostVictim();
                GameManager.numVictim--;
                GameConsole.instance.UpdateFeedback("A victim just perished.");
            }

        }
        else if (evCode == (byte)PhotonEventCodes.RemoveHazmat) {
            //parse data
            object[] dataReceived = eventData.CustomData as object[];
            Space curr = StateManager.instance.spaceGrid.grid[(int)dataReceived[0], (int)dataReceived[1]];
            int firemanId = (int)dataReceived[2];

            Hazmat hazmat = StateManager.instance.firemanCarriedHazmats[firemanId];

            //update game states
            curr.removeOccupant(hazmat);
            Destroy(hazmat.gameObject);
            Destroy(hazmat);
            StateManager.instance.firemanCarriedHazmats.Remove(firemanId);

            //update UI if any
        }
        else if (evCode == (byte)PhotonEventCodes.UpdateSpaceReferenceToFireman) { 
            object[] dataReceived = eventData.CustomData as object[];
            Space space = StateManager.instance.spaceGrid.grid[(int)dataReceived[0], (int)dataReceived[1]];
            int firemanId = (int)dataReceived[2];

            Dictionary<int, Space> d = StateManager.instance.firemanCurrentSpaces;
            if (d.ContainsKey(firemanId))
            {
                d[firemanId] = space;
            }
            else d.Add(firemanId, space);

        }
        else if (evCode == (byte)PhotonEventCodes.DriveAmbulance) {
            object[] dataReceived = eventData.CustomData as object[];

            int direction = (int)dataReceived[0];
            int viewId = (int)dataReceived[1];
            if (viewId == PV.ViewID)
            {
                driveAmbulance(direction);
            }
        }
        else if (evCode == (byte)PhotonEventCodes.DriveEngine) {
            object[] dataReceived = eventData.CustomData as object[];

            int direction = (int)dataReceived[0];
            int viewId = (int)dataReceived[1];
            if (viewId == PV.ViewID)
            {
                driveEngine(direction);
            }
        }
        else if (evCode == (byte)PhotonEventCodes.RideEngine) {
            object[] dataReceived = eventData.CustomData as object[];
            int viewId = (int)dataReceived[1];
            if (viewId == PV.ViewID)
            {
                rideEngine();
            }

        }
        else if (evCode == (byte)PhotonEventCodes.RideAmbulance) {
            object[] dataReceived = eventData.CustomData as object[]; 
            int viewId = (int)dataReceived[1];
            if (viewId == PV.ViewID)
            {
                rideAmbulance();
            }
        }
    }
}

