using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space {
    public Vector3 worldPosition;
    public int indexX;
    public int indexY;
    public SpaceStatus status;
    public SpaceKind kind;

    Wall[] walls;
    Door[] doors;
    List<GameUnit> occupants;



    public Space(Vector2 _worldPos, bool _isOutside, int _indexX, int _indexY) {
        worldPosition = new Vector3(_worldPos.x, _worldPos.y, 0);
        indexX = _indexX;
        indexY = _indexY;
        walls = new Wall[4];
        doors = new Door[4];
        status = SpaceStatus.Safe;
        kind = (_isOutside) ? SpaceKind.Outdoor : SpaceKind.Indoor;

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
        return this.kind;
    }

    public void setSpaceKind(SpaceKind newKind)
    {
        this.kind = newKind;
    }

    public Door[] getDoors()
    {
        return this.doors;
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
} 
