using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POI : GameUnit
{
    POIKind kind;
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

}
