using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ToConnectionScreen : MonoBehaviour
{

    //Variables necessary for multiplayer settings.
    public int numPlayers;
    public InputField gameRoomInputField;
    public GameObject errorMessage;

    //Used to hide error message at first.
    public void Start()
    {
        errorMessage.SetActive(false);
    }



    //Functions for each button. Checks if room name is given.
    public void OnOnePlayerClicked()
    {
        if (RoomNameIsProvided())
        {
            numPlayers = 1;
            SwitchScenes();
        }
        else
        {
            errorMessage.SetActive(true);
            Debug.Log("Room name not provided.");
        }

    }
    public void OnTwoPlayerClicked()
    {
        if (RoomNameIsProvided())
        {
            numPlayers = 2;
            SwitchScenes();
        }
        else
        {
            errorMessage.SetActive(true);
            Debug.Log("Room name not provided.");

        }

    }
    public void OnThreePlayerClicked()
    {
        if (RoomNameIsProvided())
        {
            numPlayers = 3;
            SwitchScenes();
        }
        else
        {
            errorMessage.SetActive(true);
            Debug.Log("Room name not provided.");
        }

    }
    public void OnFourPlayerClicked()
    {
        if(RoomNameIsProvided())
        {
            numPlayers = 4;
            SwitchScenes();
        }
        else
        {
            errorMessage.SetActive(true);
            Debug.Log("Room name not provided.");
        }

    }
    public void OnFivePlayerClicked()
    {
        if (RoomNameIsProvided())
        {
            numPlayers = 5;
            SwitchScenes();
        }
        else
        {
            errorMessage.SetActive(true);
            Debug.Log("Room name not provided.");
        }

    }
    public void OnSixPlayerClicked()
    {
        if (RoomNameIsProvided())
        {
            numPlayers = 6;
            SwitchScenes();
        }
        else
        {
            errorMessage.SetActive(true);
            Debug.Log("Room name not provided.");
        }

    }

    //Switches to next scene after setting up multiplayer settings.
    public void SwitchScenes()
    {
        SetMultiplayerSetting();
        SceneManager.LoadScene("ServerConnection", LoadSceneMode.Single);
    }

    //Checks that input field is not empty.
    bool RoomNameIsProvided()
    {

        if (gameRoomInputField.text == "")
        {
            Debug.Log("No text entered.");
            return false;
        }
        else
        {
            Debug.Log("Found text.");
            Debug.Log(gameRoomInputField.text);
            return true;
        }
    }

    //Sets up multiplayer settings.
    void SetMultiplayerSetting()
    {
        MultiplayerSetting.MS.SetFamilyGame(true);
        MultiplayerSetting.MS.SetNumberOfPlayers(numPlayers);
        MultiplayerSetting.MS.SetGameRoomName(gameRoomInputField.text.ToString());
    }
}
