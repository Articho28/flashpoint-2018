using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireman : GameUnit
{
    int AP;
    FMStatus status;
    Victim carriedVictim;

    public void createFM(int numOfPlayers)
    {
        //TODO
    }

    public int getAP() {
        return AP;
    }

    public void setAP(int newAP)
    {
        AP = newAP;
    }

    public void decrementAP()
    {
        //TODO
    }

    public FMStatus getStatus()
    {
        return this.status;
    }

    public void setStatus(FMStatus newStatus)
    {
        this.status = newStatus;
    }

    public Victim getVictim()
    {
        return this.carriedVictim;
    }

    public void setVictim(Victim v)
    {
        this.carriedVictim = v;
    }

    public void deassociateVictim()
    {
        this.carriedVictim = null;
    }

    public void extinguishFire(Space destination)
    {
        //TODO
    }

    public void chopWall(Wall wall)
    {
        if (wall.addDamage() && AP >= 2) AP -= 2;
    }

    public void move(Space destination)
    {

    }

    //void setCurrentSpace() is already in GameUnit, but we put it in this class again in UML for M5
}
