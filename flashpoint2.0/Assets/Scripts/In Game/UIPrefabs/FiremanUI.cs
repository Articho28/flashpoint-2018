using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FiremanUI : MonoBehaviour
{
    public static FiremanUI instance; //singleton

    public Text turnText;
    private string turn;
    public Text APText;
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
        turn = "name of firefighter";
        UpdateTurn();
        AP = 0;
        UpdateAP();
    }

    private void UpdateTurn()
    {
        turnText.text = "Turn: \n" + turn;
    }

    private void UpdateAP()
    {
        APText.text = "AP: \n" + AP;
    }
    public void AddGameState(string playername)
    {
        turn = playername;
        UpdateTurn();
    }

    public void AddAP(int APtoAdd)
    {
        AP = APtoAdd + APtoAdd;
        UpdateAP();
    }
}