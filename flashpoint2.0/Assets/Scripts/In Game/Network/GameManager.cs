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
        Debug.Log("The gamestatus is " + GameStatus);
    }




    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnAllPrefabsSpawned()
    {
        Debug.Log("Turn is " + Turn);
        Debug.Log("Game statsu is " + GameStatus);
    }
}
