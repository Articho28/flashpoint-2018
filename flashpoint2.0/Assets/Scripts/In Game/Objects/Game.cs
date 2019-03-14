using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Game : MonoBehaviour
{
    static GameState gState;
    int numOfPlayers;
    Rules rules;
    HashSet<Space> newSpaces;
    HashSet<Wall> newWall;
    HashSet<Door> newDoors;
    HashSet<ParkingSpot> newParkingSpots;
    HashSet<Fireman> newFiremen;
    HashSet<Victim> newVictims;
    HashSet<FalseAlarm> newFalseAlarms;
    static int blackDice;
    static int redDice;
    static int bldgDamageCounter;
    static int savedVictimCounter;
    static int lostVictimCounter;
    static int activePOICounter;
    static bool won;

    /*public Game create(int numPlayers, Player firstP)
    {
        //TODO
    }*/

    public void setFiremanStartingSpace(Fireman f)
    {
        Space startingSpace = new Space(); //TODO change this to get clicked tile
        if(startingSpace.getSpaceKind() == SpaceKind.Outdoor)
        {
            f.setCurrentSpace(startingSpace);
        }
    }

    public static GameState getGameState() {
        return gState;
    }


    public static void setGameState(GameState newGState)
    {
        gState = newGState;
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

    public static void rollDice()
    {
        System.Random r = new System.Random();
        blackDice = r.Next(1,9);
        redDice = r.Next(1, 7);
    }

    public static int getBlackDice()
    {
        return blackDice;
    }

    public static int getRedDice()
    {
        return redDice;
    }

    public static int getBuildingDamage()
    {
        return bldgDamageCounter;
    }

    public static void incrementBuildingDamage()
    {
        bldgDamageCounter++;
    }

    public static int getNumSavedVictims()
    {
        return savedVictimCounter;
    }

    public static void incrementNumSavedVictims()
    {
        savedVictimCounter++;
    }

    public static int getNumLostVictims()
    {
        return lostVictimCounter;
    }

    public static void incrementNumLostVictims()
    {
        lostVictimCounter++;
    }

    public static bool getGameWon()
    {
        return won;
    }

    public static void setGameWon(Boolean newWon)
    {
        won = newWon;
    }

    public static int getActivePOIs()
    {
        //TODO
        return 0;
    }

    public static void placeRandomPOIs()
    {
        //TODO
    }

    public static void resolveFirePlacement(Space s)
    {
        //TODO
    }

    public static void resolveShockwave(Space space, Space neighbour)
    {
        //TODO
    }

    public static void resolveFlashover()
    {
        //TODO
    }

    public static void removeFalseAlarm(POI fa)
    {

    }

    public void endTurn()
    {
        rollDice();
        Space s = new Space();//getSpace(Game.getRedDice(), Game.getBlackDice()); TODO function
        List<Space> neighbours = StateManager.instance.spaceGrid.GetNeighbours(s);
        Boolean onFire = false;
        foreach(Space n in neighbours){
            if (n.getSpaceStatus() == SpaceStatus.Fire) onFire = true;
        }
        if (s.getSpaceStatus() == SpaceStatus.Safe && onFire == false)
        {
            s.setSpaceStatus(SpaceStatus.Smoke);
        }
        else if (s.getSpaceStatus() == SpaceStatus.Smoke || onFire == true)
        {
            s.setSpaceStatus(SpaceStatus.Fire);
            resolveFirePlacement(s);
        }
        else
        {
            foreach (Space n in neighbours)
            {
                resolveShockwave(s, n);
            }
            resolveFlashover();
        }
        int numPOI = getActivePOIs();
        while(numPOI < 3)
        {
            placeRandomPOIs();
        }
        int endBuildingDamage = getBuildingDamage();
        int numVictimLost = getNumLostVictims();
        if (endBuildingDamage >= 24 || numVictimLost >= 4)
        {
            setGameState(GameState.Completed);
            setGameWon(false);
        }
    }
}
