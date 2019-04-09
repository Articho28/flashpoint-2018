using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameWonUI : MonoBehaviour
{
    public Text GameWonText;

    [Header("Return to Lobby")]
    public Button returnToLobbyButton;

    [Header("Continue Playing")]
    public Button continuePlayingButton;

    public static bool isCalled = false;
    // Start is called before the first frame update
    void Start()
    {
        GameWonText = GetComponent<Text>();
        returnToLobbyButton = GetComponent<Button>();
        continuePlayingButton = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void continuePlaying()
    {

        GameManager.GM.setActivePrefabs("won", false);
        isCalled = true;
        
    }

    public void returnToLobby()
    {
        Debug.Log("RETURNING TO LOBBY");
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("StartScene");

    }
}
