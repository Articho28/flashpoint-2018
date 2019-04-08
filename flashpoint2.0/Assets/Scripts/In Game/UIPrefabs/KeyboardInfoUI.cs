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
            keyboardInfoExperienced.text = "EXPERIENCED GAME\n" + "Deck gun: G\n" + "Drive ambulance: H\n"+ "Identify POI: I\n" +
        "Carry Hazmat: M\n" + "Ride vehicle: R\n" + "Exit vehicle: X\n " + "Drive engine: T\n" 
                        + "Crew changes: W\n" + "Remove hazmat: Z\n";
        }
    }

    private void UpdateKeyboardInfo()
    {

    }
}
