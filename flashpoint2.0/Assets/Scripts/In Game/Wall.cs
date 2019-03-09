using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : EdgeObstacleObject
{
    WallStatus status;
    int damageMarker;

    //constructor
    public Wall()
    {
        this.damageMarker = 0;
    }

    //wall status getter 
    public WallStatus getWallStatus()
    {
        return this.status;
    }

    //wall status setter
    public void setWallStatus(WallStatus status)
    {
        this.status = status;
    }

    //function that increments the damage marker
    public void incrementDamageMarker()
    {
        this.damageMarker++;
    }

}
