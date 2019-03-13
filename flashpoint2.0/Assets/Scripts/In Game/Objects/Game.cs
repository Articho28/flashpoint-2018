using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Game : MonoBehaviour
{
    GameState gState;
    int numOfPlayers;
    Rules rules;
    HashSet<Space> newSpaces;
    HashSet<Wall> newWall;
    HashSet<Door> newDoors;
    HashSet<ParkingSpot> newParkingSpots;
    HashSet<Fireman> newFiremen;
    HashSet<Victim> newVictims;
    HashSet<FalseAlarm> newFalseAlarms;
    int blackDice;
    int redDice;
    int bldgDamageCounter;
    int savedVictimCounter;
    int lostVictimCounter;

    /*public Game create(int numPlayers, Player firstP)
    {
        //TODO
    }*/

    /*public void setFiremanStartingSpace(Fireman f)
    {
        Space startingSpace = new Space();
        if(startingSpace.getSpaceKind() == Outdoor)
        {
            f.setCurrentSpace(startingSpace);
        }
    }*/

    public GameState getGameState() {
        return this.gState;
    }


    public void setGameState(GameState newGState)
    {
        this.gState = newGState;
    }

    public Rules getRules()
    {
        return this.rules;
    }

    public void setRules(Rules newRules)
    {
        this.rules = newRules;
    }

    public int getNumPlayers()
    {
        return this.numOfPlayers;
    }

    //TODO get functions for all the HASHSETS

    public void rollDice()
    {
        System.Random r = new System.Random();
        this.blackDice = r.Next(1,9);
        this.redDice = r.Next(1, 7);
    }

    public int getBlackDice()
    {
        return this.blackDice;
    }

    public int getRedDice()
    {
        return this.redDice;
    }

    public int getBuildingDamage()
    {
        return this.bldgDamageCounter;
    }

    public void incrementBuildingDamage()
    {
        this.bldgDamageCounter++;
    }

    public int getNumSavedVictims()
    {
        return this.savedVictimCounter;
    }

    public void incrementNumSavedVictims()
    {
        this.savedVictimCounter++;
    }

    public int getNumLostVictims()
    {
        return this.lostVictimCounter;
    }

    public void incrementNumLostVictims()
    {
        this.lostVictimCounter++;
    }

    /*public int getActivePOIs()
    {
        //TODO
    }

    public void placeRandomPOIs()
    {
        //TODO
    }

    public void resolveFirePlacement(Space s)
    {
        //TODO
    }

    public void resolveShockwave()
    {
        //TODO
    }

    public void resolveFlashover()
    {
        //TODO
    }*/
}
