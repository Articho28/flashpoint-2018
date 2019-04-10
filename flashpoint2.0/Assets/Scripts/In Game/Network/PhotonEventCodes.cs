﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PhotonEventCodes
{
    FireFighterPlacedProperly = 0,
    IncrementTurn = 1,
    PlaceInitialFireFighter = 2,
    PlacePOI = 3,
    PlaceInitialFireMarker = 4,
    Move = 5,
    Door = 6,
    AdvanceFireMarker = 7,
    AdvanceSmokeMarker = 8,
    BoardSetup = 9,
    RemoveFireMarker = 10,
    RemoveSmokeMarker = 11,
    ChopWall = 12,
    ResolveFlashOvers = 13,
    ResolveExplosion = 14,
    PlaceHazmats = 15,
    PlaceInitialHotSpot = 16,
    FlipPOI = 17,
    ReplenishPOI = 18,
    PlaceVehicles = 19,
    KnockdownFireman = 20,
    CachePlayerNames = 21,
    PlaceInitialAmbulance = 22,
    PlaceInitialEngine = 23,
    PickSpecialist = 24,
    PlaceAmbulanceParkingSpot = 25,
    PlaceEngineParkingSpot = 26,
    EndTurn = 27,
    ChangeCrew = 28,
    SpecialistIsPicked = 29,
    DriveAmbulance = 30,
    DriveEngine = 31,
    RideAmbulance = 32,
    RideEngine = 33,
    UpdateCarriedVictimsState = 34,
    UpdateCarriedHazmatsState = 35,
    MoveCarriedVictim = 36,
    MoveCarriedHazmat = 37,
    RemoveVictim = 38,
    RemoveHazmat = 39,
    InitializePOI = 40,
    UpdateSpaceReferenceToFireman = 41,
    UpdateTreatedVictimsState = 42,
    MoveTreatedVictim = 43,
    RemoveTreatedVictim = 44,
    SendRoomOptions = 45,
    PlaceFireMarker = 46,
    ResolveInitialExplosionsExperienced = 47,
    FlareUp = 48,
    PlaceExtraHotSpots = 49,
    MoveRescueDog = 50,
    MoveCommand = 51
}
