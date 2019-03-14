using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireman : GameUnit
{
    int AP;
    int savedAP;
    FMStatus status;
    Victim carriedVictim;

    public void Start() {
        AP = 6;
        savedAP = 0;
        status = FMStatus.FamilyFireFighter;
        currentSpace = StateManager.instance.spaceGrid.getGrid()[0, 3];
    }

    public void createFM(int numOfPlayers)
    {
        //TODO
    }

    public int getAP() {
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

    public void setSavedAP(int newSavedAP)
    {
        AP = newSavedAP;
    }

    public void decrementAP(int amount)
    {
        this.AP -= amount;
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

    public void extinguishFire(Space destination)
    {
        //TODO
    }

    public void chopWall(Wall wall)
    {
        if (wall.addDamage() && AP >= 2) AP -= 2;
    }

    public void move(Space destination)
    {
        //TODO NEED TO KNOW IF F HAS ENOUGH AP TO MOVE TO A SAFE SPACE
        int ap = this.getAP();
        Victim v = this.getVictim();
        bool reachable = true; //destination.isReachable(); //TODO
        SpaceStatus sp = destination.getSpaceStatus();

        if (reachable)
        {
            if (sp == SpaceStatus.Fire)
            {
                if (ap >= 2 && v == null) //&&f has enough to move
                {
                    this.setCurrentSpace(destination);
                    this.decrementAP(2);
                }
                else
                {
                    //displayActionResult("ERROR"); // TODO say what the error is with if stattements
                }
            }
            else
            {
                if (v == null)
                {
                    this.setCurrentSpace(destination);
                    this.decrementAP(1);
                }
                else //if the fireman is carrying a victim
                {
                    this.setCurrentSpace(destination);
                    this.decrementAP(2);
                }
            }
        }

        //after the move

        List<GameUnit> occ = destination.getOccupants();
        foreach (GameUnit gu in occ)
        {
            if(gu is POI)
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
            this.deassociateVictim();
            if (Game.getNumSavedVictims() >= 7)
            {
                Game.setGameWon(true);
                Game.setGameState(GameState.Completed);
            }
        }


    }

    public void openDoor()
    {
        if (getAP() >= 1)
        {
            decrementAP(1);
            this.getCurrentSpace().getDoor().setDoorStatus(DoorStatus.Open);
            string doorObjectPath = "Board/doorCol45";
            GameObject.Find(doorObjectPath).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("PhotonPrefabs/open door");
        }
    }

    public void closeDoor()
    {
        if (getAP() >= 1)
        {
            decrementAP(1);
            this.getCurrentSpace().getDoor().setDoorStatus(DoorStatus.Closed);
            string doorObjectPath = "Board/doorCol45";
            GameObject.Find(doorObjectPath).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("PhotonPrefabs/closed door");
        }
    }

    //void setCurrentSpace() is already in GameUnit, but we put it in this class again in UML for M5
}
