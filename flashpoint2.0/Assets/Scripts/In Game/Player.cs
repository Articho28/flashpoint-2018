using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player :MonoBehaviour
{
    PlayerStatus status;
    bool firstPlayer;
    Fireman f;

    //constructor
    public Player(PlayerStatus ps, bool fp, Fireman fireman){
        this.status = ps;
        this.firstPlayer = fp;
        this.f = fireman;
    }

    public PlayerStatus getStatus()
    {
        return this.status;
    }

    public void setStatus(PlayerStatus newstatus)
    {
        this.status = newstatus;
    }

    public bool getFirstPlayer()
    {
        return this.firstPlayer;
    }

    public void setFP(bool newFP)
    {
        this.firstPlayer = newFP;
    }

    public Fireman getFireman()
    {
        return this.f;
    }

    public void setFireman(Fireman newF)
    {
        this.f = newF;
    }

    public void setFiremanStartingSpace(Space newSpace)
    {
        this.f.space = newSpace;
    }

    public void endTurn()
    {
        //TODO
    }

    /*public WHAT selectKnockDownPlacement()
    {
    TODO   
    }*/

}
