using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerSetting : MonoBehaviour
{

    public static MultiplayerSetting MS;
    public int numOfPlayers;
    public bool IsFamilyGame;


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

    public void setNumberOfPlayers(int number)
    {
        numOfPlayers = number;
    }
    public void setFamilyGame(bool isFamilyGame)
    {
        this.IsFamilyGame = isFamilyGame;
    }


}
