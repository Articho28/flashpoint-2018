﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Wall : MonoBehaviour
{
    public WallStatus status;
    GameObject physicalObject;

    [SerializeField] Material intact;
    [SerializeField] Material broken;

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
            GameManager.GM.buildingDamage++;

            updateMaterial();
            return true;
        }
        else if (status == WallStatus.Damaged) {
            status = WallStatus.Destroyed;
            GameManager.GM.buildingDamage++;

            updateMaterial();
            return true;
        }

        return false;
    }

    private void updateMaterial() { 
        if(status == WallStatus.Intact) {
            this.GetComponent<MeshRenderer>().material = intact;
        }
        if (status == WallStatus.Damaged) {
            this.GetComponent<MeshRenderer>().material = broken;
        }
        else if(status == WallStatus.Destroyed) {
            Destroy(this);
            Destroy(this.physicalObject);
        }
    }

    public void setPhysicalObject(GameObject avatar)
    {
        this.physicalObject = avatar;
    }

} 

