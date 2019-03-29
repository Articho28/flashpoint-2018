using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUnit : MonoBehaviour {
    protected Space currentSpace;
    public String type;
    public GameObject physicalObject;

    void Start()
    {
        //TODO set current space
        if (this is Fireman)
        {
            setCurrentSpace(new Space(new Vector2((float)-3.48, (float)-6.505), true, 0, 3));
        }
    }

    public Space getCurrentSpace()
    {
        Vector3 pos = this.GetComponent<Transform>().position;
        return StateManager.instance.spaceGrid.WorldPointToSpace(pos);
    }

    public void setCurrentSpace(Space space)
    {
        this.currentSpace = space;
    }
    public void setType(String type)
    {
        this.type = type;
    }

    public String getType()
    {
        return this.type;
    }

    public void setPhysicalObject(GameObject avatar)
    {
        this.physicalObject = avatar;
    }

    public GameObject getPhysicalObject()
    {
        return physicalObject;
    }

}
