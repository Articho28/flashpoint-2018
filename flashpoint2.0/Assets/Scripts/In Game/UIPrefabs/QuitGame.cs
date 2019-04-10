using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class QuitGame : MonoBehaviour
{
    public void quitgame()
    {
        PhotonNetwork.LeaveRoom();
        //PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Lobby");
    }

}