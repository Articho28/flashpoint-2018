using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : EdgeObstacleObject
{
    public DoorStatus status;
    GameObject physicalObject;

    //constructor
    public Door(DoorStatus status)
    {
        this.status = status;
    }

    //door status getter
    public DoorStatus getDoorStatus()
    {
        return this.status;
    }

    //door status setter
    public void setDoorStatus(DoorStatus status)
    {
        this.status = status;
    }

    public void setPhysicalObject(GameObject avatar)
    {
        this.physicalObject = avatar;
    }
}
