using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Game : MonoBehaviour
{
    static GameState gState;
    int numOfPlayers;
    Rules rules;
    HashSet<Space> newSpaces;
    HashSet<Wall> newWall;
    HashSet<Door> newDoors;
    HashSet<Fireman> newFiremen;
    HashSet<Victim> newVictims;
    HashSet<FalseAlarm> newFalseAlarms;
    static int blackDice;
    static int redDice;
    static int bldgDamageCounter;
    static int savedVictimCounter;
    static int lostVictimCounter;
    static int activePOICounter;
    static bool won;

    /*public Game create(int numPlayers, Player firstP)
    {
        //TODO
    }*/

    public void setFiremanStartingSpace(Fireman f)
    {
//        Space startingSpace = new Space(); //TODO change this to get clicked tile
//        if (startingSpace.getSpaceKind() == SpaceKind.Outdoor)
//        {
//            f.setCurrentSpace(startingSpace);
//        }
    }

    public static GameState getGameState() {
        return gState;
    }


    public static void setGameState(GameState newGState)
    {
        gState = newGState;
    }

    public Rules getRules()
    {
        return this.rules;
    }

    public void setRules(Rules newRules)
    {
        this.rules = newRules;
    }

    public int getNumPlayers()
    {
        return this.numOfPlayers;
    }

    //TODO get functions for all the HASHSETS

    public static void rollDice()
    {
        System.Random r = new System.Random();
        blackDice = r.Next(1, 9);
        redDice = r.Next(1, 7);
    }

    public static int getBlackDice()
    {
        return blackDice;
    }

    public static int getRedDice()
    {
        return redDice;
    }

    public static int getBuildingDamage()
    {
        return bldgDamageCounter;
    }

    public static void incrementBuildingDamage()
    {
        bldgDamageCounter++;
    }

    public static int getNumSavedVictims()
    {
        return savedVictimCounter;
    }

    public static void incrementNumSavedVictims()
    {
        savedVictimCounter++;
    }

    public static int getNumLostVictims()
    {
        return lostVictimCounter;
    }

    public static void incrementNumLostVictims()
    {
        lostVictimCounter++;
    }

    public static bool getGameWon()
    {
        return won;
    }

    public static void setGameWon(Boolean newWon)
    {
        won = newWon;
    }

    public static int getActivePOIs()
    {
        //TODO
        return 0;
    }

    public static void placeRandomPOIs()
    {
        //TODO
    }

    public static void resolveFirePlacement(Space s)
    {
        //TODO
    }

    public static void removeFalseAlarm(POI fa)
    {
        //TODO
    }



    public void advanceFire()
    {
        Game.rollDice();
        Space rollSpace = StateManager.instance.spaceGrid.getGrid()[Game.getRedDice() - 1, Game.getBlackDice() - 1];
        SpaceStatus ss = rollSpace.getSpaceStatus();
        Space[] neighbours = StateManager.instance.spaceGrid.GetNeighbours(rollSpace);

        if (ss == SpaceStatus.Safe) rollSpace.setSpaceStatus(SpaceStatus.Smoke);
        else if (ss == SpaceStatus.Smoke) rollSpace.setSpaceStatus(SpaceStatus.Fire);
        else if (ss == SpaceStatus.Fire)
        {
            Game.explosion(rollSpace, "Place Fire");
            return;
        }

        if (rollSpace.getSpaceStatus() == SpaceStatus.Smoke)
        {
            foreach (Space n in neighbours)
            {
                if (n.getSpaceStatus() == SpaceStatus.Fire)
                {
                    rollSpace.setSpaceStatus(SpaceStatus.Fire);
                }
            }
        }
    }

    public static void explosion (Space s, string action){

        Space[] neighbours = StateManager.instance.spaceGrid.GetNeighbours(s);
        for (int i = 0; i < 4; i++) {
            SpaceStatus ss = neighbours[i].getSpaceStatus();
            if (ss == SpaceStatus.Fire)
            {
                Shockwave(neighbours[i], i);
            }
            else if (ss == SpaceStatus.Safe || ss == SpaceStatus.Smoke)
            {
                s.setSpaceStatus(SpaceStatus.Fire);
            }
        }

        Wall[] walls = s.getWalls();
        foreach (Wall w in walls)
        {
            w.addDamage();
            Game.incrementBuildingDamage();
        }

        Door[] doors = s.getDoors();
        foreach(Door d in doors)
        {
            //d.Destroy(); TODO
        }

    }

    public static void Shockwave(Space s, int direction)
    {
        Space[] neighbours = StateManager.instance.spaceGrid.GetNeighbours(s);
        if (neighbours[direction] == null) //this would mean there's a wall or closed door
        {
            Door[] doors = s.getDoors();
            if (doors[direction] != null && doors[direction].getDoorStatus() == DoorStatus.Closed)
            {
                //doors[direction].Destroy(); TODO
                return;
            }
            Wall[] walls = s.getWalls();
            if (walls[direction] != null && walls[direction].getWallStatus() != WallStatus.Destroyed)
            {
                walls[direction].addDamage();
                return;
            }
        }
        SpaceStatus ss = neighbours[direction].getSpaceStatus();
        if(ss != SpaceStatus.Fire)
        {
            neighbours[direction].setSpaceStatus(SpaceStatus.Fire);
            return;
        }
        //plz don't trust this recursion LOL
        Shockwave(neighbours[direction], direction); //recursive call
    }

    public static void Flashover()
    {
        //while: there is no Smoke Adjacent to Fire at the end of a turn
        //flip any Smoke marker in a space Adjacent to Fire to Fire (Remember: Smoke Adjacent to Fire = Fire)
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                //while: there is no Smoke Adjacent to Fire at the end of a turn
                //flip any Smoke marker in a space Adjacent to Fire to Fire (Remember: Smoke Adjacent to Fire = Fire)
                Space curr = StateManager.instance.spaceGrid.getGrid()[i, j];
                Space[] neighbours = StateManager.instance.spaceGrid.GetNeighbours(curr);
                foreach (Space n in neighbours)
                {
                    if (curr.getSpaceStatus() == SpaceStatus.Fire && n.getSpaceStatus() == SpaceStatus.Smoke)
                    {
                        n.setSpaceStatus(SpaceStatus.Fire);
                    }
                }

                List<GameUnit> occupants = curr.getOccupants();
                foreach (GameUnit occ in occupants)
                {
                    Space occSpace = occ.getCurrentSpace();
                    SpaceStatus ss = occSpace.getSpaceStatus();
                    if (ss == SpaceStatus.Fire)
                    {
                        //Any Firefighters in a space with Fire are Knocked Down
                        //make a function call --> KnockedDown
                        if (occ is Fireman)
                        {

                            //((Fireman)occ).KnockedDown();
                        }
                        //Any Victims or POI in a space with Fire are Lost
                        //Place the POI or Victim marker on the Lost space at the edge of the board
                        //if: a POI was not identified
                        //reveal (flip over) the POI then place on the Lost spot
                        if (occ is POI)
                        {
                            if (((POI)occ).getPOIKind() == POIKind.Victim)
                            {
                                ((Victim)occ).setVictimStatus(VictimStatus.Lost);
                                Game.incrementNumLostVictims();
                                //TODO place the victims on the lost area
                            }
                        }
                    }
                }
                //Remove any Fire markers that were placed outside of the building
                if(curr.getSpaceKind() == SpaceKind.Outdoor)
                {
                    if(curr.getSpaceStatus() == SpaceStatus.Fire)
                    {
                        curr.setSpaceStatus(SpaceStatus.Safe);
                    }
                }
            }
        }
    }


    public void endTurn()
        {
            Game.rollDice();
            Space s = StateManager.instance.spaceGrid.getGrid()[Game.getRedDice() - 1, Game.getBlackDice() - 1];
            Space[] neighbours = StateManager.instance.spaceGrid.GetNeighbours(s);
            Boolean onFire = false;
            foreach(Space n in neighbours)
            {
                if (n.getSpaceStatus() == SpaceStatus.Fire) onFire = true;
            }
            if (s.getSpaceStatus() == SpaceStatus.Safe && onFire == false)
            {
                s.setSpaceStatus(SpaceStatus.Smoke);
            }
            else if (s.getSpaceStatus() == SpaceStatus.Smoke || onFire == true)
            {
                s.setSpaceStatus(SpaceStatus.Fire);
                Game.resolveFirePlacement(s);
            }
            else
            {
                for(int i=0; i<4; i++)
                {
                    Game.Shockwave(s, i); //HUH??????
                }
                Game.Flashover();
            }
            int numPOI = Game.getActivePOIs();
            while(numPOI < 3)
            {
                Game.rollDice();
                Game.placeRandomPOIs();
                numPOI = Game.getActivePOIs();
        }
            int endBuildingDamage = Game.getBuildingDamage();
            int numVictimLost = Game.getNumLostVictims();
            if (endBuildingDamage >= 24 || numVictimLost >= 4)
            {
                Game.setGameState(GameState.Completed);
                Game.setGameWon(false);
            }
        }
} 
