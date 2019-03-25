using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class FiremanUI : MonoBehaviour
{
    //public static FiremanUI instance; //singleton

    public Text playerNameText;
    public Text APText;
    private int AP;


    void Start()
    {
        playerNameText.text = "Palyer Name : \n" + PhotonNetwork.LocalPlayer.NickName;
        AP = 0;
        UpdateAP();
    }



    private void UpdateAP()
    {
        APText.text = "AP: \n" + AP;
    }

    public void AddAP(int APtoAdd)
    {
        AP = APtoAdd + APtoAdd;
        UpdateAP();
    }
}