using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerSetting : MonoBehaviour
{

    public static MultiplayerSetting MS;
    public int numOfPlayers;
    public bool isFamilyGame;
    public string gameRoomName;

    private void Awake()
    {
        if(MultiplayerSetting.MS == null)
        {
            MultiplayerSetting.MS = this;
        }
        else
        {
            if(MultiplayerSetting.MS != this)
            {
                Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }
   
    public void SetNumberOfPlayers(int number)
    {
        numOfPlayers = number;
    }

    public void SetFamilyGame(bool isFamilyGame)
    {
        this.isFamilyGame = isFamilyGame;
    }
    public void SetGameRoomName(string name)
    {
        this.gameRoomName = name;
    }



}
