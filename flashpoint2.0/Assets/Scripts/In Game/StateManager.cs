﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateManager : MonoBehaviour {
    #region Singleton
    public static StateManager instance;

    private void Awake() {
        instance = this;
    }

    #endregion

    [SerializeField] GameObject spaceGridObject;
    SpaceGrid grid;

    // Use this for initialization
    void Start() {
        grid = spaceGridObject.GetComponent<SpaceGrid>();
    }

}