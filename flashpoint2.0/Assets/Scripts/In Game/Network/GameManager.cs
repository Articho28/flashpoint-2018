using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{

    public static GameManager GM;
    public static string GameStatus;
    public static int Turn;


    public void Awake()
    {
        if (GM == null)
        {
            GM = this;
            GameStatus = FlashPointGameConstants.GAME_STATUS_SPAWNING_PREFABS;
        }
        else
        {
            if(GM != this)
            {
                Destroy(GM);
                GM = this;
            }
        }
        Debug.Log("The GameManager was created.");
        Debug.Log("The gamestatus is at first:  " + GameStatus);
    }




    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
       if (GameStatus == FlashPointGameConstants.GAME_STATUS_SPAWNING_PREFABS)
        {
            //Nothing to do here, just wait for the spawning to be done.
        }
        else if (GameStatus == FlashPointGameConstants.GAME_STATUS_INITIALPLACEMENT)
        {
            //Implement Place firefighter function.
            while (Turn < PhotonNetwork.CountOfPlayers + 1) 
            {
                PlaceInitialFireFighter(Turn);
                Turn++;
            }
            Debug.Log("Everyone should have chosen a location.");
           
        }
    }

    public void OnAllPrefabsSpawned()
    {
        Debug.Log("Turn is " + Turn);
        Debug.Log("Game status is " + GameStatus);
    }

    public bool PlaceInitialFireFighter(int Player)
    {
        Debug.Log("It's " + PhotonNetwork.PlayerList[Player - 1].NickName + " 's turn to place his firefighter!");
        return true;
    }
}
