﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NextTurn : MonoBehaviour {
    [SerializeField] Firefighter[] firefighters;


	// Use this for initialization
	void Start () {
        firefighters = FindObjectsOfType<Firefighter>();
        Debug.Log(firefighters.Length);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
