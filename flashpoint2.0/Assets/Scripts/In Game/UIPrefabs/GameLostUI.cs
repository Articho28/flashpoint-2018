using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameLostUI : MonoBehaviour
{
    public Text GameLostText;
    public Button GameLostButton;
    // Start is called before the first frame update
    void Start()
    {
        GameLostText = GetComponent<Text>();
        GameLostButton = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void returnToLobby()
    {
        Debug.Log("RETURNING TO LOBBY");
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("StartScreen");
    }

  
}
