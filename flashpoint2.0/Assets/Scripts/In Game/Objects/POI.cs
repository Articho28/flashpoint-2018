using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POI : GameUnit
{
    POIKind kind;
    bool flipped = false;
    void Start()
    {
        setPOIKind(POIKind.Victim);
    }

    public void setPOIKind(POIKind newK)
    {
        this.kind = newK;
    }

    public POIKind getPOIKind()
    {
        return this.kind;
    }

    public POI getPOI()
    {
        return this;
    }
    public void setIsFlipped (bool flip)
    {
        this.flipped = flip;
    }
    public bool getIsFlipped()
    {
        return this.flipped;
    }
}
