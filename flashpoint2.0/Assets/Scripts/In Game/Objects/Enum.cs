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
    ParkingSpot,
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
    Paramedic,
    FireCaptain,
    ImagingTechnician,
    CAFSFirefighter,
    HazmatTechinician,
    Generalist,
    RescueSpecialist,
    DriverOperator
}