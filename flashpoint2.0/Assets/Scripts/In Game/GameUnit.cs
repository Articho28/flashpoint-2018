﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUnit : MonoBehaviour {
    Space currentSpace;

    public Space getCurrentSpace() {
        return this.currentSpace;
    }

    public void setCurrentSpace(Space space) {
        this.currentSpace = space;
    }
}