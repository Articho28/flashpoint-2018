using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PhotonEventCodes
{
    placeFireFighter = 0,
    IncrementTurn = 1,
    PlaceInitialFireFighter = 2,
    PlacePOI = 3,
    PlaceInitialFireMarker = 4,
    Move = 5,
    Door= 6,
    AdvanceFireMarker = 7,
    AdvanceSmokeMarker = 8,
    BoardSetup = 9,
    RemoveFireMarker = 10,
    RemoveSmokeMarker = 11,
    ChopWall = 12
}
