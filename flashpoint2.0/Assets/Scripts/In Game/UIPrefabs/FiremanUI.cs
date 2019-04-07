using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class FiremanUI : MonoBehaviour
{
    public static FiremanUI instance; //singleton

    public Text playerNameText;
    public Text specialistText; //TODO
    public Text APText;
    public Text SpecialistAPText;
    private string specialistName;
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
        specialistText.text = "Specialist : \n" + "-";
        AP = 0;
        UpdateAP();
    }

    private void UpdateAP()
    {
        APText.text = "AP: \n" + AP;
    }
    private void UpdateSpecialistAP()
    {
        if (!GameManager.GM.isFamilyGame)
        {
            SpecialistAPText.text = "Specialist AP: \n" + SpecialistAP;
        }
    }
    private void UpdateSpecialist()
    {
        if (!GameManager.GM.isFamilyGame)
        {
            specialistText.text = "Specialist: \n" + specialistName;
        }
    }

    public void SetAP(int newAP)
    {
        AP = newAP;
        UpdateAP();
    }

    public void SetSpecialistAP(int newSpecialistAP)
    {
        if (!GameManager.GM.isFamilyGame)
        {
            SpecialistAP = newSpecialistAP;
        }
        UpdateSpecialistAP();
    }

    public void SetSpecialist(Specialist newSpecialist)
    {
        specialistName = newSpecialist.ToString();
        UpdateSpecialist();
    }
}