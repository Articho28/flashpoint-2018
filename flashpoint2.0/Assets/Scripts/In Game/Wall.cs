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

    //damage the wall
    public bool addDamage() 
    {
        if (status == WallStatus.Intact) {
            status = WallStatus.Damaged;
            return true;
        }
        else if (status == WallStatus.Damaged) {
            status = WallStatus.Destroyed;
            return true;
        }

        return false;
    }

    //function that increments the damage marker
    public void incrementDamageMarker()
    {
        this.damageMarker++;
    }

}
