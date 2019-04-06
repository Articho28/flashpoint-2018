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
    FMStatus status;
    Victim carriedVictim;
    Ambulance movedAmbulance;
    Engine movedEngine;
    private PhotonView PV;
    private bool isWaitingForInput;
    private bool isExtinguishingFire;
    private bool isChoppingWall;
    private bool isSelectingExtinguishOption;
    private bool isSelectingSpecialist;
    private bool isChangingCrew;
    public ArrayList validInputOptions;
    Space locationArgument;
    Specialist spec;

    public static Photon.Realtime.RaiseEventOptions sendToAllOptions = new Photon.Realtime.RaiseEventOptions()
    {
        CachingOption = Photon.Realtime.EventCaching.DoNotCache,
        Receivers = Photon.Realtime.ReceiverGroup.All
    };

    void Start()
    {
        AP = 4;
        savedAP = 0;
        carriedVictim = null;
        PV = GetComponent<PhotonView>();
        isWaitingForInput = false;
        isExtinguishingFire = false;
        validInputOptions = new ArrayList();
        isChoppingWall = false;
        isSelectingExtinguishOption = false;
        isChangingCrew = false;
        isSelectingSpecialist = false;
    }

    void Update()
    {


        if (PV.IsMine && GameManager.GM.Turn == PhotonNetwork.LocalPlayer.ActorNumber && GameManager.GameStatus ==
       FlashPointGameConstants.GAME_STATUS_PLAY_GAME)
        {
            //ADDITIONAL KEYS IN EXPERIENCED GAME
            //Fire the Deck Gun "G"
            //Drive vehicle "H"
            //Crew Change "W"

            if (!GameManager.GM.isFamilyGame)
            {
                if (Input.GetKeyDown(KeyCode.G))
                {
                    //deckGun(); TODO 
                }
                else if (Input.GetKeyDown(KeyCode.H))
                {
                    //driveAmbulance(); TODO
                }
                else if (Input.GetKeyDown(KeyCode.T))
                {
                    //driveEngine(); TODO
                }
                else if (Input.GetKeyDown(KeyCode.R))
                {
                    //if getAmbulance TODO
                        rideAmbulance();
                    //if getEngine TODO
                        //rideEngine();
                }
                else if (Input.GetKeyDown(KeyCode.X))
                {
                    deassociateAmbulance(); //if fireman riding ambulance TODO
                    deassociateEngine(); //if fireman riding engine TODO
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
                object[] data = { PV.ViewID, 0 };
                PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.Move, data, sendToAllOptions, SendOptions.SendReliable);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                object[] data = { PV.ViewID, 2 };
                PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.Move, data, sendToAllOptions, SendOptions.SendReliable);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                object[] data = { PV.ViewID , 1 };
                PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.Move, data, sendToAllOptions, SendOptions.SendReliable);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                object[] data = { PV.ViewID, 3 };
                PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.Move, data, sendToAllOptions, SendOptions.SendReliable);
            }
            else if (Input.GetKeyDown(KeyCode.D)) //open/close door
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

            else if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Extinguish Fire Detected");
                extinguishFire();
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                Debug.Log("Chop Wall Detected");
                chopWall();
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                //if (!GameManager.GM.isFamilyGame)
                //{
                    Debug.Log("Crew Change Detected");
                    changeCrew();
                //}
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
                            this.setAP(numAP - 1);
                            FiremanUI.instance.SetAP(this.getAP());
                            sendSmokeMarkerExtinguishEvent(targetSpace);
                        }
                        else
                        {
                            if (numAP < 2)
                            {
                                GameConsole.instance.UpdateFeedback("Turning Fire To Smoke...");
                                this.setAP(numAP - 1);
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
                        this.setAP(this.getAP() - 2);
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
                        GameManager.GM.freeSpecialistIndex[0] = 0;
                        //TODO UPDATE THE UI TO DISPLAY THE NEW SPECIALIST
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

                        sendChangeCrewEvent(GameManager.GM.freeSpecialistIndex);
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isChangingCrew = true;
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
                            this.setAP(numAP - 1);
                            FiremanUI.instance.SetAP(this.getAP());
                            sendSmokeMarkerExtinguishEvent(targetSpace);
                        }
                        else
                        {
                            if (numAP < 2)
                            {
                                GameConsole.instance.UpdateFeedback("Turning Fire To Smoke...");
                                this.setAP(numAP - 1);
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
                        this.setAP(this.getAP() - 2);
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
                        GameManager.GM.freeSpecialistIndex[1] = 0;
                        //TODO UPDATE THE UI TO DISPLAY THE NEW SPECIALIST
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

                        sendChangeCrewEvent(GameManager.GM.freeSpecialistIndex);
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isChangingCrew = true;
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
                            this.setAP(numAP - 1);
                            FiremanUI.instance.SetAP(this.getAP());
                            sendSmokeMarkerExtinguishEvent(targetSpace);
                        }
                        else
                        {
                            if (numAP < 2)
                            {
                                GameConsole.instance.UpdateFeedback("Turning Fire To Smoke...");
                                this.setAP(numAP - 1);
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
                        this.setAP(this.getAP() - 2);
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
                        GameManager.GM.freeSpecialistIndex[2] = 0;
                        //TODO UPDATE THE UI TO DISPLAY THE NEW SPECIALIST
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

                        sendChangeCrewEvent(GameManager.GM.freeSpecialistIndex);
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isChangingCrew = true;
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
                            this.setAP(numAP - 1);
                            FiremanUI.instance.SetAP(this.getAP());
                            sendSmokeMarkerExtinguishEvent(targetSpace);
                        }
                        else
                        {
                            if (numAP < 2)
                            {
                                GameConsole.instance.UpdateFeedback("Turning Fire To Smoke...");
                                this.setAP(numAP - 1);
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
                        this.setAP(this.getAP() - 2);
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
                        GameManager.GM.freeSpecialistIndex[3] = 0;
                        //TODO UPDATE THE UI TO DISPLAY THE NEW SPECIALIST
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

                        sendChangeCrewEvent(GameManager.GM.freeSpecialistIndex);
                    }
                    else
                    {
                        GameConsole.instance.UpdateFeedback("Not a valid input. \n" + oldMessage);
                        isWaitingForInput = true;
                        isChangingCrew = true;
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
                            this.setAP(numAP - 1);
                            FiremanUI.instance.SetAP(this.getAP());
                            sendSmokeMarkerExtinguishEvent(targetSpace);
                        }
                        else
                        {
                            if (numAP < 2)
                            {
                                GameConsole.instance.UpdateFeedback("Turning Fire To Smoke...");
                                this.setAP(numAP - 1);
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
                        GameManager.GM.freeSpecialistIndex[4] = 0;
                        //TODO UPDATE THE UI TO DISPLAY THE NEW SPECIALIST
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

                        sendChangeCrewEvent(GameManager.GM.freeSpecialistIndex);
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
                        GameManager.GM.freeSpecialistIndex[5] = 0;
                        //TODO UPDATE THE UI TO DISPLAY THE NEW SPECIALIST
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

                        sendChangeCrewEvent(GameManager.GM.freeSpecialistIndex);
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
                        GameManager.GM.freeSpecialistIndex[6] = 0;
                        //TODO UPDATE THE UI TO DISPLAY THE NEW SPECIALIST
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

                        sendChangeCrewEvent(GameManager.GM.freeSpecialistIndex);
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
                        GameManager.GM.freeSpecialistIndex[7] = 0;
                        //TODO UPDATE THE UI TO DISPLAY THE NEW SPECIALIST
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

                        sendChangeCrewEvent(GameManager.GM.freeSpecialistIndex);
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
                    this.setAP(this.getAP() - 2);
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
                    this.setAP(this.getAP() - 1);
                    FiremanUI.instance.SetAP(this.getAP());
                } 

            }
            else if (Input.GetKeyDown(KeyCode.V))
            {
                carryVictim();
            }
        }
       else if (PV.IsMine && GameManager.GM.Turn == PhotonNetwork.LocalPlayer.ActorNumber && GameManager.GameStatus ==
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
                        GameConsole.instance.UpdateFeedback("Paramedic is picked as Specialist.");
                        GameManager.IncrementTurn();
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
                        GameConsole.instance.UpdateFeedback("Fire Captain is picked as Specialist.");
                        GameManager.IncrementTurn();
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
                        GameConsole.instance.UpdateFeedback("Imaging Technician is picked as Specialist.");
                        GameManager.IncrementTurn();
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
                        GameConsole.instance.UpdateFeedback("CAFS Firefighter is picked as Specialist.");
                        GameManager.IncrementTurn();
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
                        GameConsole.instance.UpdateFeedback("Hazmat Technician is picked as Specialist.");
                        GameManager.IncrementTurn();
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
                        GameConsole.instance.UpdateFeedback("Generalist is picked as Specialist.");
                        GameManager.IncrementTurn();
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
                        GameConsole.instance.UpdateFeedback("Rescue Specialist is picked as Specialist.");
                        GameManager.IncrementTurn();
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
                        GameConsole.instance.UpdateFeedback("Driver Operator is picked as Specialist.");
                        GameManager.IncrementTurn();
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
        else if (GameManager.GM.Turn != PhotonNetwork.LocalPlayer.ActorNumber)
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

    public void newTurnAP()
    {

        if (this.spec == Specialist.FamilyGame)
        {
            this.AP = this.savedAP + 4;
            this.commandAP = 0;
            this.extinguishAP = 0;
            this.moveAP = 0;
        }
        else if (this.spec == Specialist.Paramedic)
        {
            this.AP = this.savedAP + 4;
            this.commandAP = 0;
            this.extinguishAP = 0;
            this.moveAP = 0;
        }
        else if (this.spec == Specialist.FireCaptain)
        {
            this.AP = this.savedAP + 4;
            this.commandAP = 2;
            this.extinguishAP = 0;
            this.moveAP = 0;
        }
        else if (this.spec == Specialist.ImagingTechnician)
        {
            this.AP = this.savedAP + 4;
            this.commandAP = 0;
            this.extinguishAP = 0;
            this.moveAP = 0;
        }
        else if (this.spec == Specialist.CAFSFirefighter)
        {
            this.AP = this.savedAP + 3;
            this.commandAP = 0;
            this.extinguishAP = 3;
            this.moveAP = 0;
        }
        else if (this.spec == Specialist.HazmatTechinician)
        {
            this.AP = this.savedAP + 4;
            this.commandAP = 0;
            this.extinguishAP = 0;
            this.moveAP = 0;
        }
        else if (this.spec == Specialist.Generalist)
        {
            this.AP = this.savedAP + 5;
            this.commandAP = 0;
            this.extinguishAP = 0;
            this.moveAP = 0;
        }
        else if (this.spec == Specialist.RescueSpecialist)
        {
            this.AP = this.savedAP + 4;
            this.commandAP = 0;
            this.extinguishAP = 0;
            this.moveAP = 3;
        }
        else if (this.spec == Specialist.DriverOperator)
        {
            this.AP = this.savedAP + 4;
            this.commandAP = 0;
            this.extinguishAP = 0;
            this.moveAP = 0;
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

    public void deassociateAmbulance()
    {
        this.movedAmbulance = null;
    }

    public Engine getEngine()
    {
        return this.movedEngine;
    }

    public void setEngine(Engine n)
    {
        this.movedEngine = n;
    }

    public void deassociateEngine()
    {
        this.movedEngine = null;
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
    public void selectSpecialist()
    {
        isWaitingForInput = true;
        isSelectingSpecialist = true;
        string optionsToUser = "";
        for (int i = 0; i < GameManager.GM.freeSpecialistIndex.Length; i++)
        {
            Debug.Log("iteration " + i);
            if (GameManager.GM.freeSpecialistIndex[i] != 0)
            {
                optionsToUser = optionsToUser + "Press " + i + " for " + GameManager.GM.availableSpecialists[i] + ". ";
                Debug.Log(optionsToUser);
            }

        }
    }    
    public void extinguishFire()
    {
        int numAP = getAP(); //returns the number of action points

        //Get current space and spacestatus. 

        Space current = this.getCurrentSpace();
        SpaceStatus currentSpaceStatus = current.getSpaceStatus();

        if (numAP == 1 && currentSpaceStatus == SpaceStatus.Fire)
        {
            GameConsole.instance.UpdateFeedback("You only have enough AP to extinguish at your location and safely end the turn.");
            this.setAP(numAP - 1);
            FiremanUI.instance.SetAP(this.getAP());
            sendTurnFireMarkerToSmokeEvent(current);
            return;
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
            Debug.Log("Not enough AP!");  //Used to show the player why he can’t perform an action in case of failure
            GameConsole.instance.UpdateFeedback("Not enough AP!");
        }
        else
        {
            //Get indices of all spaces accessible that are not safe (valid neighbors + current Space).
            ArrayList extinguishOptions = getUnsafeSpacesIndecies(currentSpaceStatus, neighborsStatuses);
            validInputOptions = extinguishOptions;

            //Build string to show.
            string optionsToUser = "";

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



    public void chopWall()     {         int numAP = getAP(); //returns the number of action points          //Check if sufficient AP.         if (numAP < 2)         {             Debug.Log("Not enough AP!");  //Used to show the player why he can’t perform an action in case of failure             GameConsole.instance.UpdateFeedback("Not enough AP!");         }         else         {             //Get indices of all spaces accessible that are not safe (valid neighbors + current Space).             ArrayList nearbyWalls = getNearbyWalls(this.getCurrentSpace());             validInputOptions = nearbyWalls;              //Build string to show.             string optionsToUser = "";              foreach (int index in nearbyWalls)             {                  if (index == 0)                 {                     optionsToUser += "Press 0 for the Wall on Top ";                 }                 else if (index == 1)                 {                     optionsToUser += " Press 1 for the Wall to Your Right";                 }                 else if (index == 2)                 {                     optionsToUser += " Press 2 for the Wall to the Bottom";                 }                 else if (index == 3)                 {                     optionsToUser += " Press 3 for the Wall to Your Left";                  }             }              GameConsole.instance.UpdateFeedback(optionsToUser);              isWaitingForInput = true;
            isChoppingWall = true;          }     }      private ArrayList getNearbyWalls(Space s)     {         ArrayList nearbyWalls = new ArrayList();         Wall[] wallArray = s.getWalls();          //Collect directions in which there is a wall         for (int i = 0; i < wallArray.Length; i++)         {             if (wallArray[i] != null)             {                 nearbyWalls.Add(i);             }         }         return nearbyWalls;     } 
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
                    return;
                }
            }
            GameConsole.instance.UpdateFeedback("There is no victim to be carried!");
        }
    }

    void driveAmbulance(Space targetSpace, int direction)
    {
        while (!GameManager.GM.isFamilyGame)
        {
            int ap = this.getAP();
            Space current = this.getCurrentSpace();
            this.getAmbulance();

            //get parkingspots

            //if fireman is in same space with ambulance
           
                //promt: right or left
                string optionsToUser = "";

                //foreach (int index in nearbyParkingSpots)
                //{
                //    if (index == 1)
                //    {
                //        optionsToUser += "Press 1 for Driving Clockwise";
                //    }
                //    else if (index == 3)
                //    {
                //        optionsToUser += " Press 2 for Driving Counter-Clockwise";
                //    }
                //}

                GameConsole.instance.UpdateFeedback(optionsToUser);

                //if prompt is right
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    //if opposite side: 4AP

                    //get the curr x/y index & 4 switch statements: 
                    int currentSpaceX = this.getCurrentSpace().indexX;
                    int currentSpaceY = this.getCurrentSpace().indexY;
                    object[] data = { currentSpaceX, currentSpaceY };
                    //AmbulanceParkingSpot[] parkingSpots = targetSpace.getParkingSpots();
                    int indexX = targetSpace.indexX;
                    int indexY = targetSpace.indexY;
                    //if (parkingSpots != null)
                    //{
                    //    for (int i = 0; i < 4; i++)
                    //    {
                    //        direction = i;
                    //        AmbulanceParkingSpot ps = parkingSpots[i];
                    //        if (ps != null)
                    //        {
                    //            //switch (direction)
                    //            //{
                    //            //    case 0:
                    //            //        //targetSpace.   (null, direction);
                    //            //        int northX = targetSpace.indexX;
                    //            //        int northY = targetSpace.indexY - 1;
                    //            //        if (northX <= 10 && northY <= 8)
                    //            //        {
                    //            //            Space northSpace = StateManager.instance.spaceGrid.grid[northX, northY];
                    //            //        }
                    //            //        break;
                    //            //}
                    //        }
                    //    }
                    //}
                }
                //if prompt is left
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {

                }
            }
        //else if fireman is not in same space with ambulance
        //iterate through entire grid 
        //get the ambulance
        //get index x - found ambulance in this space
        //promt user: right or left
        //if left SAME
        //if right SAME
    }

    void driveEngine()
    {
        //fireman has to be on the same space with engine
        //TODO
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
                    return;
                }
            }
            GameConsole.instance.UpdateFeedback("There is no ambulance!");
        }

        //fireman that are in the Vehicle Parking Spot of a Vehicle while it is being Driven can Ride the Vehicle for 0AP
        //any number of firemen may Ride a Vehicle at one time
        //exit
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
                    return;
                }
            }
            GameConsole.instance.UpdateFeedback("There is no engine!");
        }
    }

    public void move(int direction) {

        //TODO NEED TO KNOW IF F HAS ENOUGH AP TO MOVE TO A SAFE SPACE
        int ap = this.getAP();
        Victim v = this.getVictim();
        Ambulance h = this.getAmbulance();
        Engine n = this.getEngine();
        Space curr = this.getCurrentSpace();
        Space[] neighbors = StateManager.instance.spaceGrid.GetNeighbours(curr);
        Space destination = neighbors[direction];

        if (destination == null) {
            GameConsole.instance.UpdateFeedback("Invalid move. Please try again");
            return;
        }

        SpaceStatus sp = destination.getSpaceStatus();

        Vector3 newPosition = new Vector3(destination.worldPosition.x, destination.worldPosition.y, -10);
        Space newSpace = StateManager.instance.spaceGrid.WorldPointToSpace(newPosition);

        if (sp == SpaceStatus.Fire) {
            if (ap >= 3 && v == null) //&&f has enough to move
            {
                this.setCurrentSpace(newSpace);
                this.decrementAP(2);
                newSpace.addOccupant(this);
                FiremanUI.instance.SetAP(this.AP);
                this.GetComponent<Transform>().position = newPosition;
            }
            else {
                GameConsole.instance.UpdateFeedback("Insufficient AP");
                return;
            }
        }
        else {
            if (v == null && ap >= 1) {
                this.setCurrentSpace(newSpace);
                this.decrementAP(1);
                newSpace.addOccupant(this);
                FiremanUI.instance.SetAP(this.AP);
                GameConsole.instance.UpdateFeedback("You have successfully moved");
                this.GetComponent<Transform>().position = newPosition;
                List<GameUnit> gameUnits = destination.getOccupants();
                foreach (GameUnit gu in gameUnits) {
                    if (gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_POI) {
                        if (gu.GetComponent<POI>().getIsFlipped() == false) {
                            GameManager.FlipPOI(destination);
                            break;
                        }
                    }
                }
            }
            else if (!GameManager.GM.isFamilyGame && h != null && ap >= 2)//if the fireman riding the ambulance
            {
                Kind destinationKind = destination.getKind();
                if (destinationKind == Kind.AmbulanceParkingSpot)
                {
                    //ride ambulance
                    this.setCurrentSpace(destination);
                    v.setCurrentSpace(destination);
                    this.decrementAP(2);
                    FiremanUI.instance.SetAP(this.AP);
                    this.GetComponent<Transform>().position = newPosition;
                    v.GetComponent<Transform>().position = newPosition;

                    //removing the ambulance from the current space
                    List<GameUnit> currentGameUnits = curr.getOccupants();
                    GameUnit ambulance = null;
                    foreach (GameUnit gu in currentGameUnits)
                    {
                        if (gu != null && gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_AMBULANCE)
                        {
                            ambulance = gu;
                            break;
                        }
                    }
                    currentGameUnits.Remove(ambulance);
                    destination.addOccupant(ambulance);

                    GameConsole.instance.UpdateFeedback("You have successfully moved with the ambulance");

                    //if fireman wants to exit the ambulance TODO
                    deassociateAmbulance();
                }
            }
            else if (!GameManager.GM.isFamilyGame && n != null && ap >= 2)//if the fireman riding the engine
            {
                Kind destinationKind = destination.getKind();
                if (destinationKind == Kind.AmbulanceParkingSpot)
                {
                    //ride engine
                    this.setCurrentSpace(destination);
                    v.setCurrentSpace(destination);
                    this.decrementAP(2);
                    FiremanUI.instance.SetAP(this.AP);
                    this.GetComponent<Transform>().position = newPosition;
                    v.GetComponent<Transform>().position = newPosition;

                    //removing the engine from the current space
                    List<GameUnit> currentGameUnits = curr.getOccupants();
                    GameUnit engine = null;
                    foreach (GameUnit gu in currentGameUnits)
                    {
                        if (gu != null && gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_ENGINE)
                        {
                            engine = gu;
                            break;
                        }
                    }
                    currentGameUnits.Remove(engine);
                    destination.addOccupant(engine);

                    GameConsole.instance.UpdateFeedback("You have successfully moved with the ambulance");

                    //if fireman wants to exit the engine TODO
                    deassociateEngine();
                }
            }
            else if (v != null && ap >= 2)//if the fireman is carrying a victim
            {
                SpaceStatus destinationSpaceStatus = destination.getSpaceStatus();

                SpaceKind destinationSpaceKind = destination.getSpaceKind();


                if ((destinationSpaceStatus == SpaceStatus.Safe && destinationSpaceKind == SpaceKind.Indoor) || destinationSpaceStatus == SpaceStatus.Smoke) {
                    //carry victim

                    this.setCurrentSpace(newSpace);
                    v.setCurrentSpace(newSpace);
                    this.decrementAP(2);
                    FiremanUI.instance.SetAP(this.AP);
                    this.GetComponent<Transform>().position = newPosition;
                    v.GetComponent<Transform>().position = newPosition;

                    newSpace.addOccupant(this);
                    //removing the victim from the current space.
                    List<GameUnit> currentGameUnits = curr.getOccupants();
                    GameUnit victim = null;
                    foreach (GameUnit gu in currentGameUnits) {
                        if (gu != null && gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_POI) {
                            victim = gu;
                            break;
                        }
                    }
                    currentGameUnits.Remove(victim);
                    destination.addOccupant(victim);


                    GameConsole.instance.UpdateFeedback("You have successfully moved with a victim");
                    //if has POI marker
                    List<GameUnit> destinationGameUnits = destination.getOccupants();
                    foreach (GameUnit gu in destinationGameUnits) {
                        if (gu != null && gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_POI) {
                            if (gu.GetComponent<POI>().getIsFlipped() == false) {
                                GameManager.FlipPOI(destination);
                                break;
                            }
                        }
                    }
                }
                else if (destinationSpaceKind == SpaceKind.Outdoor) {
                    //carry victim outside the building
                    this.setCurrentSpace(newSpace);
                    newSpace.addOccupant(this);
                    this.decrementAP(2);
                    this.GetComponent<Transform>().position = newPosition;

                    //change victim status to rescued
                    v.setVictimStatus(VictimStatus.Rescued);
                    GameManager.savedVictims++;
                    GameUI.instance.AddSavedVictim();

                    List<GameUnit> gameUnits = curr.getOccupants();
                    GameUnit victim = null;
                    foreach (GameUnit gu in gameUnits) {
                        if (gu != null && gu.getType() == FlashPointGameConstants.GAMEUNIT_TYPE_POI) {
                            victim = gu;
                            break;
                        }
                    }
                    gameUnits.Remove(victim);
                    Destroy(victim.physicalObject);
                    Destroy(victim);
                    deassociateVictim();
                    GameManager.numOfActivePOI--;
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
                else //Fire
                {
                    //can not carry victim
                    GameConsole.instance.UpdateFeedback("Cannot carry a victim onto fire!");
                    return;
                }
            }
            else {
                GameConsole.instance.UpdateFeedback("Insufficient AP");
                return;
            }
        }


                //after the move TODO??

        //        List<GameUnit> occ = destination.getOccupants();
        //foreach (GameUnit gu in occ)
        //{
        //    if (gu is POI)
        //    {
        //        POIKind gukind = ((POI)gu).getPOIKind();
        //        if (gukind == POIKind.FalseAlarm)
        //        {
        //            //TODO remove false alarm
        //        }
        //    }
        //}
        
        //if (v != null && destination.getSpaceKind() == SpaceKind.Outdoor)
        //{
        //    v.setVictimStatus(VictimStatus.Rescued);
        //    //place victim marker on the rescued space 

        //    Game.incrementNumSavedVictims();
        //    GameUI.instance.AddSavedVictim();
        //    this.deassociateVictim();
        //    if (Game.getNumSavedVictims() >= 7)
        //    {
        //        Game.setGameWon(true);
        //        Game.setGameState(GameState.Completed);
        //        GameUI.instance.AddGameState("Completed");
        //    }
        //}
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
        GameManager.replenishPOI();
        GameManager.IncrementTurn();

    }


    private void restoreAP()
    {
       
        int currentNumAP = this.getAP();
        int newAP = Mathf.Min(currentNumAP + 4, 8);

        this.setAP(newAP);
        FiremanUI.instance.SetAP(newAP);
    }


    public void changeCrew() //return the index of the specialist we want
    {
        if (this.getAP() >= 2)
        {

            string optionsToUser =  "";
            Specialist[] test = GameManager.GM.availableSpecialists;

            for (int i = 0; i< GameManager.GM.freeSpecialistIndex.Length; i++)
            {
                Debug.Log("iteration " + i);
                if (GameManager.GM.freeSpecialistIndex[i] != 0)
                {
                    optionsToUser = optionsToUser + "Press " + i + " for " + GameManager.GM.availableSpecialists[i] + ". ";
                    Debug.Log(optionsToUser);
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
            int direction = (int)data[1];

            if ((int)data[0] == PV.ViewID)
            {
               move(direction);
            }
        
        }
        else if (evCode == (byte)PhotonEventCodes.PickSpecialist)
        {
            GameManager.GM.Turn = 1;
            GameManager.GameStatus = FlashPointGameConstants.GAME_STATUS_PICK_SPECIALIST;
            GameManager.GM.DisplayPlayerTurn();
            GameUI.instance.AddGameState(GameManager.GameStatus);
            selectSpecialist();
        }
    }
}

