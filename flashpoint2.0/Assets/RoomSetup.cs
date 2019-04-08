using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSetup : MonoBehaviour
{

    public static RoomSetup RM;

    private bool isFamilyGame;
    private int experiencedModeDifficultyIndex;

    // Start is called before the first frame update

    private void Awake()
    {
        if(RM == null)
        {
            RM = this;
        }
        else
        {
            if (RM != this)
            {
                Destroy(RM);
                RM = this;
            }
        }
        experiencedModeDifficultyIndex = -1;
        DontDestroyOnLoad(this);
    }


    public bool getIsFamilyGame()
    {
        return isFamilyGame;
    }

    public int getExperiencedModeDifficultyIndex()
    {
        return experiencedModeDifficultyIndex;
    }

    public void setIsFamilyGame(bool value)
    {
        isFamilyGame = value;
    }

    public void setExperiencedModeDifficultyIndex(int index)
    {
        if (index > -1 && index < 3)
        {
            experiencedModeDifficultyIndex = index;
        }
        else
        {
            throw new System.ArgumentException("Invalid index : \n Select 0 for Recruit, 1 for Veteran, and 2 for Heroic");
        }
    }

}
