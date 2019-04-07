using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space {
    public Vector3 worldPosition;
    public int indexX;
    public int indexY;
    public SpaceStatus status;
    public SpaceKind spaceKind;
    public Kind kind;

    [SerializeField]
    Wall[] walls;
    Door[] doors;
    public List<GameUnit> occupants = new List<GameUnit>();



    public Space(Vector2 _worldPos, bool _isOutside, int _indexX, int _indexY) {
        worldPosition = new Vector3(_worldPos.x, _worldPos.y, 0);
        indexX = _indexX;
        indexY = _indexY;
        walls = new Wall[4];
        doors = new Door[4];
        status = SpaceStatus.Safe;
        spaceKind = (_isOutside) ? SpaceKind.Outdoor : SpaceKind.Indoor;

    }

    //function to add walls at a certain index
    public void addWall(Wall wall, int index)
    {
        this.walls[index] = wall;
    }

    //function to add doors at a certain index
    public void addDoor(Door door, int index)
    {
        this.doors[index] = door;
    }

    public SpaceStatus getSpaceStatus()
    {
        return this.status;
    }

    public void setSpaceStatus(SpaceStatus newStatus)
    {
        this.status = newStatus;
    }

    public SpaceKind getSpaceKind()
    {
        return this.spaceKind;
    }

    public void setSpaceKind(SpaceKind newKind)
    {
        this.spaceKind = newKind;
    }

    public Kind getKind()
    {
        return this.kind;
    }

    public Door[] getDoors()
    {
        return this.doors;
    }

    public void setDoors(Door[] newDoors)
    {
        this.doors = newDoors;

    }

    public void setWalls(Wall[] newWalls)
    {
        this.walls = newWalls;
    }

    public Wall[] getWalls()
    {
        return this.walls;
    }
  
    public void addOccupant(GameUnit u)
    {
        occupants.Add(u);
    }

    public List<GameUnit> getOccupants()
    {
        return this.occupants;
 
    }

    public void removeOccupant(Fireman fireman) {
        for(int i = 0; i < occupants.Count; i++) {
            if (occupants[i].getType() == FlashPointGameConstants.GAMEUNIT_TYPE_FIREMAN
            && occupants[i].GetComponent<Fireman>().Equals(fireman)) {
                occupants.RemoveAt(i);
                break;
            }
        }
    }

    public void removeOccupant(Victim victim) {
        for (int i = 0; i < occupants.Count; i++) {
            if (occupants[i].getType() == FlashPointGameConstants.GAMEUNIT_TYPE_POI
            && occupants[i].GetComponent<Victim>().Equals(victim)) {
                occupants.RemoveAt(i);
                break;
            }
        }
    }

    public void removeOccupant(Hazmat hazmat) {
        for (int i = 0; i < occupants.Count; i++) {
            if (occupants[i].getType() == FlashPointGameConstants.GAMEUNIT_TYPE_HAZMAT
            && occupants[i].GetComponent<Hazmat>().Equals(hazmat)) {
                occupants.RemoveAt(i);
                break;
            }
        }
    }
} 
