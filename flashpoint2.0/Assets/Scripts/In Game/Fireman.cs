using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireman : GameUnit
{
    int actionPoints;

    public int getActionPoints() {
        return actionPoints;
    }

    public void chopWall(Wall wall) {
        if (wall.addDamage() && actionPoints >= 2) actionPoints -= 2;
    }
}
