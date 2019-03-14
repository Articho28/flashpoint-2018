using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : EdgeObstacleObject
{
    public WallStatus status;

    //constructor
    public Wall()
    {
        this.status = WallStatus.Intact;
    }

    //wall status getter 
    public WallStatus getWallStatus()
    {
        return this.status;
    }

    //damage the wall
    public bool addDamage() 
    {
        if (status == WallStatus.Intact) {
            status = WallStatus.Damaged;
            Game.incrementBuildingDamage();
            return true;
        }
        else if (status == WallStatus.Damaged) {
            status = WallStatus.Destroyed;
            Game.incrementBuildingDamage();
            return true;
        }

        return false;
    }

} 

