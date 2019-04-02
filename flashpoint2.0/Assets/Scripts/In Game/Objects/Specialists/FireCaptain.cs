using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCaptain : Fireman
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //4 AP plus 2 free Command AP per turn
    //The Fire Captain can use their AP to command any other Firefighter to Move and/or Open/Close Doors on the Fire Captain’s turn.
    //Commanded Firefighters may be Carrying Victims, Hazmat, or Move with Treated Victims.
    //The Fire Captain spends AP at the cost that the Commanded Fire ghter would normally have paid for that movement.
    //Free Command AP cannot be saved.
    //No more than 1 Command AP may be spent on the CAFS Firefighter per turn.
}
