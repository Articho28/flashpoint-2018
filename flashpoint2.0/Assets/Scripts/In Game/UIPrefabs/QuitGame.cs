﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void quitgame()
    {
        Application.Quit();
        Debug.Log("You pressed Quit Game");
    }
}