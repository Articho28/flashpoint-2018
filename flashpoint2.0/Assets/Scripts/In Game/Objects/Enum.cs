using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FMStatus
{
    FamilyFireFighter,
    ExperiencedFirefighter
}

public enum WallStatus
{
    Intact,
    Damaged,
    Destroyed
}

public enum POIKind
{
    Victim,
    FalseAlarm
}

public enum VictimStatus
{
    Unconscious,
    Treated,
    Rescued,
    Lost
}

public enum SpaceKind
{
    Indoor,
    Outdoor
}

public enum Kind
{
    Empty,
    AmbulanceParkingSpot,
    EngineParkingSpot,
    Victim,
    FalseAlarm
}

public enum Rules
{
    FamilyMode,
    ExperiencedMode
}

public enum GameState
{
    ReadyToJoin,
    NotReadyToJoin,
    KnockedDownPlacement,
    Completed
}

public enum DoorStatus
{
    Open,
    Closed,
    Destroyed
}

public enum SpaceStatus
{
    Safe,
    Smoke,
    Fire
}

public enum PlayerStatus
{
    Ready,
    NotReady
}

public enum Direction
{
    North,
    East,
    West,
    South
}

public enum Difficulty
{
    Recruit,
    Veteran,
    Heroic
} 

public enum Specialist
{
    FamilyGame,
    Paramedic, //0
    FireCaptain, //1
    ImagingTechnician, //2
    CAFSFirefighter, //3
    HazmatTechinician, //4
    Generalist, //5
    RescueSpecialist, //6
    DriverOperator //7
}