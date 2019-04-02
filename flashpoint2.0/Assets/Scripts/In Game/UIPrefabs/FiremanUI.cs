using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class FiremanUI : MonoBehaviour
{
    public static FiremanUI instance; //singleton

    public Text playerNameText;
    public Text APText;
    public Text SpecialistAPText;
    private int SpecialistAP;
    private int AP;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        playerNameText.text = "Player Name : \n" + PhotonNetwork.LocalPlayer.NickName;
        AP = 0;
        UpdateAP();
    }

    private void UpdateAP()
    {
        APText.text = "AP: \n" + AP;

        if (!GameManager.GM.isFamilyGame)
        {
            SpecialistAPText.text = "Specialist AP: \n" + SpecialistAP;
        }
    }

    public void SetAP(int newAP)
    {
        AP = newAP;
        UpdateAP();
    }
}