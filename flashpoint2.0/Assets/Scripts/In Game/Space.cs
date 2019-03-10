using System.Collections;
using UnityEngine;

public class Space {
    public Vector3 worldPosition;
    public bool isOutside;
    public int indexX;
    public int indexY;
    SpaceStatus status;
    SpaceKind kind;
    Wall[] walls = new Wall[4];
    Door[] doors = new Door[4];

    //default constructor
    public Space()
    {
        worldPosition = new Vector3(0, 0, 0);
        isOutside = false;
        indexX = 0;
        indexY = 0;
        status = SpaceStatus.Safe;
        kind = SpaceKind.Indoor;

    }
    

    public Space(Vector2 _worldPos, bool _isOutside, int _indexX, int _indexY) {
        worldPosition = new Vector3(_worldPos.x, _worldPos.y, 0);
        isOutside = _isOutside;
        indexX = _indexX;
        indexY = _indexY;
    }

    //function to add walls at a certain index
    public void addWall(Wall wall, int index)
    {
        this.walls[index] = wall;
    }

    //function to add doors at a certain index
    public void addDoor(Door door, int index)
    {
        this.doors[index] = door;
    }

    public SpaceStatus getSpaceStatus()     {         return this.status;     }      public void setSpaceStatus(SpaceStatus newStatus)     {         this.status = newStatus;     }      public SpaceKind getSpaceKind()     {         return this.kind;     }      public void setSpaceKind(SpaceKind newKind)     {         this.kind = newKind;     }

    public Door getDoor()
    {
        foreach(Door door in doors)
        {
            if (door != null) return door;
        }
    }

    public Wall getWall(int index)
    {
        return this.walls[index];
    } 
}