

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireman : GameUnit
{
    int AP;
    int savedAP;
    FMStatus status;
    Victim carriedVictim;

    public void createFM(int numOfPlayers)
    {
        //TODO
    }

    public int getAP() {
        return AP;
    }

    public void setAP(int newAP)
    {
        AP = newAP;
    }

    public int getSavedAP()
    {
        return AP;
    }

    public void setSavedAP(int newSavedAP)
    {
        if(newSavedAP <=4) AP = newSavedAP;
    }

    public void decrementAP(int amount)
    {
        this.AP -= amount;
    }

    public void setAPStartTurn()
    {
        AP = getSavedAP()+4;
    }

    public FMStatus getStatus()
    {
        return this.status;
    }

    public void setStatus(FMStatus newStatus)
    {
        this.status = newStatus;
    }

    public Victim getVictim()
    {
        return this.carriedVictim;
    }

    public void setVictim(Victim v)
    {
        this.carriedVictim = v;
    }

    public void deassociateVictim()
    {
        this.carriedVictim = null;
    }

    public void extinguishFire(Space destination, Action a)
    {
        int numAP = getAP(); //returns the number of action points

        if (numAP < 1)
        {
            Debug.Log("Not enough AP!");  //Used to show the player why he can’t perform an action in case of failure
        }
        else
        {
            if (a == Action.FlipFire)
            {     //if the player chooses to "Flip Fire"
                destination.setSpaceStatus(SpaceStatus.Smoke);          //sets the SpaceStatus to Smoke
                decrementAP(1);                 //sets the number of action points of the calling firefighter
            }
            else if (a == Action.RemoveFire)
            {          //if the player chooses "Remove Fire"
                destination.setSpaceStatus(SpaceStatus.Safe);                       //sets the SpaceStatus to Smoke
                decrementAP(2);                             //sets the number of action points of the calling firefighter
            }
            else if (a == Action.RemoveSmoke)
            {     //if the player chooses to "Remove Smoke"
                destination.setSpaceStatus(SpaceStatus.Safe);                   //sets the SpaceStatus to Safe
                decrementAP(1);                         //sets the number of action points of the calling firefighter
            }
            else
            {
                Debug.Log("Nothing to extinguish here!"); //Used to show the player why he can’t perform an action in case of failure
            }
        }
    }

    public void chopWall(Wall wall)
    {
        if (wall.addDamage() && AP >= 2) AP -= 2;
    }

    public void move(Space destination)
    {
        //TODO NEED TO KNOW IF F HAS ENOUGH AP TO MOVE TO A SAFE SPACE
        int ap = this.getAP();
        Victim v = this.getVictim();
        bool reachable = true; //destination.isReachable(); //TODO
        SpaceStatus sp = destination.getSpaceStatus();

        if (reachable)
        {
            if (sp == SpaceStatus.Fire)
            {
                if (ap >= 2 && v == null) //&&f has enough to move
                {
                    this.setCurrentSpace(destination);
                    this.decrementAP(2);
                }
                else
                {
                    //displayActionResult("ERROR"); // TODO say what the error is with if stattements
                }
            }
            else
            {
                if (v == null)
                {
                    this.setCurrentSpace(destination);
                    this.decrementAP(1);
                }
                else //if the fireman is carrying a victim
                {
                    this.setCurrentSpace(destination);
                    this.decrementAP(2);
                }
            }
        }

        //after the move

        List<GameUnit> occ = destination.getOccupants();
        foreach (GameUnit gu in occ)
        {
            if(gu is POI)
            {
                POIKind gukind = ((POI)gu).getPOIKind();
                if (gukind == POIKind.FalseAlarm)
                {
                    //TODO remove false alarm
                }
            }
        }

        if (v != null && destination.getSpaceKind() == SpaceKind.Outdoor)
        {
            v.setVictimStatus(VictimStatus.Rescued);
            Game.incrementNumSavedVictims();
            this.deassociateVictim();
            if (Game.getNumSavedVictims() >= 7)
            {
                Game.setGameWon(true);
                Game.setGameState(GameState.Completed);
            }
        }


    }


    public void KnockedDown()
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

    public void openDoor()
    {
        if (getAP() >= 1)
        {
            decrementAP(1);
            Door[] doors = this.getCurrentSpace().getDoors();
            foreach (Door d in doors)
            {
                d.setDoorStatus(DoorStatus.Open);
            }
            string doorObjectPath = "Board/doorCol45";
            GameObject.Find(doorObjectPath).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("PhotonPrefabs/open door");
        }
    }

    public void closeDoor()
    {
        if (getAP() >= 1)
        {
            decrementAP(1);
            Door[] doors = this.getCurrentSpace().getDoors();
            foreach (Door d in doors)
            {
                d.setDoorStatus(DoorStatus.Closed);
            }
            string doorObjectPath = "Board/doorCol45";
            GameObject.Find(doorObjectPath).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("PhotonPrefabs/closed door");
        }
    }


} 
