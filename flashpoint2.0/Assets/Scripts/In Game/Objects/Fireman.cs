

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using System;

public class Fireman : GameUnit
{
    int AP;
    int savedAP;
    FMStatus status;
    Victim carriedVictim;
    private PhotonView PV;
    private bool isWaitingForInput;
    ArrayList validInputOptions;

    void Start()
    {
        AP = 4;
        savedAP = 0;
        carriedVictim = null;
        PV = GetComponent<PhotonView>();
        isWaitingForInput = false;
        validInputOptions = new ArrayList();
    }

    public int getAP()
    {
        return AP;
    }

    public void setAP(int newAP)
    {
        AP = newAP;
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

    public Victim getVictim()
    {
        return this.carriedVictim;
    }

    public void setVictim(Victim v)
    {
        this.carriedVictim = v;
    }

    public void deassociateVictim()
    {
        this.carriedVictim = null;
    }

    public void extinguishFire()
    {
        int numAP = getAP(); //returns the number of action points

        //Get current space and spacestatus. 

        Space current = this.getCurrentSpace();
        SpaceStatus currentSpaceStatus = current.getSpaceStatus();


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
            Debug.Log("Not enough AP!");  //Used to show the player why he can’t perform an action in case of failure
            GameConsole.instance.UpdateFeedback("Not enough AP!");
        }
        else
        {
            //Get indices of all spaces accessible that are not safe (valid neighbors + current Space).
            ArrayList extinguishOptions = getUnsafeSpacesIndecies(currentSpaceStatus, neighborsStatuses);
            validInputOptions = extinguishOptions;

            //Build string to show.
            String optionsToUser = "";

            foreach (int index in extinguishOptions) {

               
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

            /*
            if (curentSpaceStatus != SpaceStatus.Safe)

            if (a == Action.FlipFire)
            {     //if the player chooses to "Flip Fire"
                destination.setSpaceStatus(SpaceStatus.Smoke);          //sets the SpaceStatus to Smoke
                decrementAP(1);                 //sets the number of action points of the calling firefighter
            }
            else if (a == Action.RemoveFire)
            {          //if the player chooses "Remove Fire"
                destination.setSpaceStatus(SpaceStatus.Safe);                       //sets the SpaceStatus to Smoke
                decrementAP(2);                             //sets the number of action points of the calling firefighter
            }
            else if (a == Action.RemoveSmoke)
            {     //if the player chooses to "Remove Smoke"
                destination.setSpaceStatus(SpaceStatus.Safe);                   //sets the SpaceStatus to Safe
                decrementAP(1);                         //sets the number of action points of the calling firefighter
            }
            else
            {
                Debug.Log("Nothing to extinguish here!"); //Used to show the player why he can’t perform an action in case of failure
            }
                */

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


    public void chopWall(Wall wall)
    {
        if (wall.addDamage() && AP >= 2) AP -= 2;
    }

    public void move(int direction)
    {
        //TODO NEED TO KNOW IF F HAS ENOUGH AP TO MOVE TO A SAFE SPACE
        int ap = this.getAP();
        Victim v = this.getVictim();
        bool reachable = true; //destination.isReachable(); //TODO
        Space curr = this.getCurrentSpace();
        Debug.Log("Index X is " + curr.indexX + " and Index Y is " + curr.indexY);
        Space[] neighbors = StateManager.instance.spaceGrid.GetNeighbours(curr);
        //foreach (Space s in neighbors)
        //{
        //    if (s != null)
        //    {
        //        Debug.Log("not null yeayeay");
        //    }
        //}
        Space destination = neighbors[direction];

        if (destination == null)
        {
            GameConsole.instance.UpdateFeedback("Invalid move. Please try again");
            return;
        }



        SpaceStatus sp = destination.getSpaceStatus();

        if (reachable)
        {
            if (sp == SpaceStatus.Fire)
            {
                if (ap >= 2 && v == null) //&&f has enough to move
                {
                    Debug.Log(ap);
                    Debug.Log(this.transform.position);
                    this.setCurrentSpace(destination);
                    this.decrementAP(2);
                    Debug.Log(ap);
                    Debug.Log(this.transform.position);
                }
                else
                {
                    GameConsole.instance.UpdateFeedback("Insufficient AP");
                    return;
                }
            }
            else
            {
                if (v == null && ap >=1)
                {
                    this.setCurrentSpace(destination);
                    this.decrementAP(1);
                    FiremanUI.instance.SetAP(this.AP);
                    GameConsole.instance.UpdateFeedback("You have successfully moved");
                    Vector3 newPosition = new Vector3(destination.worldPosition.x, destination.worldPosition.y, -10);
                    this.GetComponent<Transform>().position = newPosition;

                }
                else if (v != null && ap >=2)//if the fireman is carrying a victim
                {
                    this.setCurrentSpace(destination);
                    this.decrementAP(2);
                }
                else
                {
                    GameConsole.instance.UpdateFeedback("Insufficient AP");
                    return;
                }
            }
        }

        //after the move TODO??

        List<GameUnit> occ = destination.getOccupants();
        foreach (GameUnit gu in occ)
        {
            if (gu is POI)
            {
                POIKind gukind = ((POI)gu).getPOIKind();
                if (gukind == POIKind.FalseAlarm)
                {
                    //TODO remove false alarm
                }
            }
        }

        if (v != null && destination.getSpaceKind() == SpaceKind.Outdoor)
        {
            v.setVictimStatus(VictimStatus.Rescued);
            Game.incrementNumSavedVictims();
            GameUI.instance.AddSavedVictim();
            this.deassociateVictim();
            if (Game.getNumSavedVictims() >= 7)
            {
                Game.setGameWon(true);
                Game.setGameState(GameState.Completed);
                GameUI.instance.AddGameState("Completed");
            }
        }


    }


    public void KnockedDown()
    {
        //A Firefighter is Knocked Down when Fire advances into their space; this could be from an explosion or being in a Smoke
        //filled space that ignites

        //if: KnockedDown
        //take the Firefighter from its space
        //place it on the closest (as the crow flies) Ambulance Parking Spot outside the building
        //if: two Parking Spots are equally distant, choose one


        //Leave the Fire marker in the space

        //if: the KnockedDown Firefighter was carrying a Victim
        //Victim is Lost --> Place the Victim marker on the Lost space at the edge of the board
        //make a function call to VictimLoss
    }

    public void openDoor()
    {
        Debug.Log("open door");
        if (getAP() >= 1)
        {
            decrementAP(1);
            Door[] doors = this.getCurrentSpace().getDoors();
            foreach (Door d in doors)
            {
                d.setDoorStatus(DoorStatus.Open);
            }
            string doorObjectPath = "Board/doorCol45";
            GameObject.Find(doorObjectPath).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("PhotonPrefabs/open door");
        }
    }

    public void closeDoor()
    {
        Debug.Log("close door");
        if (getAP() >= 1)
        {
            decrementAP(1);
            Door[] doors = this.getCurrentSpace().getDoors();
            foreach (Door d in doors)
            {
                d.setDoorStatus(DoorStatus.Closed);
            }
            string doorObjectPath = "Board/doorCol45";
            GameObject.Find(doorObjectPath).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("PhotonPrefabs/closed door");
        }
    }

    void Update()
    {
       

        if (PV.IsMine && GameManager.GM.Turn == PhotonNetwork.LocalPlayer.ActorNumber && GameManager.GameStatus ==
       FlashPointGameConstants.GAME_STATUS_PLAY_GAME)
        {

            //MOVE: ARROWS WITH DIRECTION
            //OPEN/CLOSE DOOR: "D"
            //CHOP WALL "C"
            //END TURN "Q"
            //EXTINGUISH FIRE/SMOKE "E" + Number.

            //NORTH = 0; EAST = 1; SOUTH = 2; WEST = 3
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                this.move(0);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                this.move(2);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                this.move(1);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                this.move(3);
            }
            else if (Input.GetKeyDown(KeyCode.D)) //open/close door
            {
                int doorDir = 4;//forbidden value
                Door[] doors = this.getCurrentSpace().getDoors();
                Debug.Log(this.getCurrentSpace().worldPosition);
                for (int i = 0; i < 4; i++)
                {
                    Debug.Log(doors[i]);
                    if (doors[i] != null)
                    {
                        doorDir = i;
                    }
                }
                if (doorDir >= 0 && doorDir <= 3)
                {
                    if (doors[doorDir].getDoorStatus() == DoorStatus.Open)
                    {
                        this.closeDoor();
                    }
                    else if (doors[doorDir].getDoorStatus() == DoorStatus.Closed)
                    {
                        this.openDoor();
                    }
                }
                else
                {
                    Debug.Log("there are no doors near the space you're on!");
                }

            }

            else if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Extinguish Fire Detected");
                extinguishFire();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                if(isWaitingForInput)
                {
                    Debug.Log("Input 0 Received");
                    isWaitingForInput = false;
                    if (validInputOptions.Contains(0))
                    {
                        Debug.Log("This is a valid extinguish option.");
                        GameConsole.instance.UpdateFeedback("Removing fire.");
                        validInputOptions = new ArrayList();
                    }
                    else
                    {
                        string oldMessage = GameConsole.instance.FeedbackText.text;
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (isWaitingForInput)
                {
                    Debug.Log("Input 1 Received");
                    isWaitingForInput = false;
                    if (validInputOptions.Contains(1))
                    {
                        Debug.Log("This is a valid extinguish option.");
                        GameConsole.instance.UpdateFeedback("Removing fire.");
                        validInputOptions = new ArrayList();
                    }
                    else
                    {
                        string oldMessage = GameConsole.instance.FeedbackText.text;
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                    }

                }
            }

            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (isWaitingForInput)
                {
                    Debug.Log("Input 2 Received");
                    isWaitingForInput = false;
                    if (validInputOptions.Contains(2))
                    {
                        Debug.Log("This is a valid extinguish option.");
                        GameConsole.instance.UpdateFeedback("Removing fire.");
                        validInputOptions = new ArrayList();

                        Space targetSpace = StateManager.instance.spaceGrid.getNeighborInDirection(this.currentSpace, 2);
                        int targetX = targetSpace.indexX;
                        int targetY = targetSpace.indexY;
                        //object[] data = new object[] { targetSpace.worldPosition, targetSpace.indexX, targetSpace.indexY };


                        List<GameUnit> spaceOccupants  = targetSpace.getOccupants();
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
                            spaceOccupants.Remove(targetMarker);
                            Destroy(targetMarker.physicalObject);
                            Destroy(targetMarker);
                        }

                    }
                    else
                    {
                        string oldMessage = GameConsole.instance.FeedbackText.text;
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (isWaitingForInput)
                {
                    Debug.Log("Input 3 Received");
                    isWaitingForInput = false;
                    if (validInputOptions.Contains(3))
                    {
                        Debug.Log("This is a valid extinguish option.");
                        GameConsole.instance.UpdateFeedback("Removing fire.");
                        validInputOptions = new ArrayList();
                    }
                    else
                    {
                        string oldMessage = GameConsole.instance.FeedbackText.text;
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;

                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (isWaitingForInput)
                {
                    Debug.Log("Input 4 Received");
                    isWaitingForInput = false;
                    if (validInputOptions.Contains(4))
                    {
                        Debug.Log("This is a valid extinguish option.");
                        GameConsole.instance.UpdateFeedback("Removing fire.");
                        validInputOptions = new ArrayList();
                    }
                    else
                    {
                        string oldMessage = GameConsole.instance.FeedbackText.text;
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;

                    }
                }
            }


            else if (Input.GetKeyDown(KeyCode.Q))
            {
                endTurn();
            }
        }
    }

    public void endTurn()
    {
        restoreAP();
        GameManager.advanceFire();
        GameManager.IncrementTurn();
    }


    private void restoreAP()
    {
        int currentNumAP = this.getAP();
        if (this.savedAP < 4 && currentNumAP > 0)
        {
            do
            {
                this.savedAP++;
                currentNumAP--;
            } while (currentNumAP > 0 && this.savedAP < 4);
        }
        this.setAP(4);
        FiremanUI.instance.SetAP(4);
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
        if (evCode == (byte)PhotonEventCodes.Move)
        {
            object[] data = eventData.CustomData as object[];

            if (data.Length == 3)
            {
                if ((int)data[0] == PV.ViewID)
                {
                    //do stuff here
                }
            }
        }
        //Door = 6
        if (evCode == (byte)PhotonEventCodes.Door)
        {
            object[] data = eventData.CustomData as object[];

        }
    }
} 
