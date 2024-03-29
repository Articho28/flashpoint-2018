﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI instance; //singleton

    public Text gameStateText;
    private string gameState;
    public Text savedVictimText;
    private int savedVictim;
    public Text lostVictimText;
    private int lostVictim;
    public Text damageText;
    private int damage;
    public Text activePOIText;
    private int activePOI;
    public Text playerTurnNameText;
    private string playerTurnName;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        gameState = "gs";
        UpdateGameState();
        savedVictim = 0;
        UpdateSavedVictim();
        lostVictim = 0;
        UpdateLostVictim();
        damage = 0;
        UpdateDamage();
        activePOI = 0;
        UpdateActivePOI();
        playerTurnName = "";
    }

    private void UpdateGameState()
    {
        gameStateText.text = "GameState: \n" + gameState;
    }

    private void UpdateSavedVictim()
    {
        savedVictimText.text = "Saved Victims: \n" + savedVictim;
    }

    private void UpdateLostVictim()
    {
        lostVictimText.text = "Lost Victims: \n" + lostVictim;
    }

    private void UpdateDamage()
    {
        damageText.text = "Damage: \n" + damage;
    }

    private void UpdateActivePOI()
    {
        activePOIText.text = "Active POIs: \n" + activePOI;
    }

    public void AddGameState(string newGameState)
    {
        gameState = newGameState;
        UpdateGameState();

    }

    public void AddSavedVictim()
    {
        savedVictim++;
        if (GameManager.savedVictims >= 7)
        {
            //check for a perfect game
            if (GameManager.savedVictims == 10)
            {
                GameManager.GameWon();
                GameObject.Find("/Canvas/GameWonUIPanel/ContinuePlayingButton").SetActive(false);
                GameObject.Find("InGame").GetComponent<AudioSource>().mute = true;
                GameObject.Find("SoundManager").GetComponent<AudioScript>().playSound(0);
            }
            if (!GameWonUI.isCalled)
            {
                GameManager.GameWon();
                GameObject.Find("InGame").GetComponent<AudioSource>().mute = true;
                GameObject.Find("SoundManager").GetComponent<AudioScript>().playSound(0);

            }
        }
        UpdateSavedVictim();
    }

    public void AddLostVictim()
    {
        lostVictim++;
        if (lostVictim >=4)
        {
            GameManager.GameLost();
            GameObject.Find("InGame").GetComponent<AudioSource>().mute = true;
            GameObject.Find("SoundManager").GetComponent<AudioScript>().playSound(1);
        }
        UpdateLostVictim();
    }

    public void AddDamage(int newDamage)
    {
        damage += newDamage;
        if (damage >= 24)
        {
            GameManager.GameLost();
            GameObject.Find("InGame").GetComponent<AudioSource>().mute = true;
            GameObject.Find("SoundManager").GetComponent<AudioScript>().playSound(1);
        }
        UpdateDamage();
    }

    public void DecrementActivePOI()
    {
        activePOI--;
        UpdateActivePOI();
    }

    public void UpdatePlayerTurnName(string name)
    {
        playerTurnNameText.text = "Player's Turn: \n" + name;
    }

}