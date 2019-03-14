using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{

    public static GameManager GS;


    public void Awake()
    {
        if (GS == null)
        {
            GS = this;
        }
        else
        {
            if(GS != this)
            {
                Destroy(GS);
                GS = this;
            }
        }

        Debug.Log("The GameManager was created.");
    }




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
