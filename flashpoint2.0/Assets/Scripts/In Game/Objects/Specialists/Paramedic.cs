using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paramedic : Fireman
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    //4 AP per turn
    //Treat: Resuscitate a Victim: 1 AP
    //Place a Heal marker under the Treated Victim to denote this change in status.
    //A Treated Victim can be Moved by any Fireghter without having to Carry them.
    //Moving with a Treated Victim incurs no AP penalty.
    //A Firefighter can only lead one Treated Victim at a time. 
    //They may Carry a Victim and lead a Treated Victim concurrently at the normal Movement cost to Carry a Victim (2 AP). 
    //A Treated Victim may still not be Moved into a Fire space.
    //The Paramedic pays double AP costs to Extinguish Fire and/or Smoke.
}
