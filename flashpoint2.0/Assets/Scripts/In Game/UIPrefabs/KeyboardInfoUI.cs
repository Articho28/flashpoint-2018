using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardInfoUI : MonoBehaviour
{
    public static KeyboardInfoUI instance; //singleton

    public Text keyboardInfo;
    public Text keyboardInfoExperienced;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        keyboardInfo.text = "Move: direction arrows\n" + "Chop wall: C\n" + "Open/close door: D\n" + "Extinguish fire: E\n" +
            "Flip POI: F\n" + "Carry victim: V\n" + "End Turn: Q";

        if (!GameManager.GM.isFamilyGame)
        {
            keyboardInfoExperienced.text = "Deck gun: G\n" + "Drive vehicle: H\n" + "Crew changes: W\n" +
                "Remove hazmat: Z\n" + "\t(ONLY H. T.)";
        }
    }

    private void UpdateKeyboardInfo()
    {

    }
}
