using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;


public class PhotonLobby : MonoBehaviourPunCallbacks
{


    public static PhotonLobby lobby;

    public GameObject connectToServerButton;
    public GameObject connectToServerText;
    public GameObject cancelButton;
    public GameObject connectionSuccessfulText;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Showing button.");
        connectToServerText.SetActive(false);
        connectToServerButton.SetActive(true);
        cancelButton.SetActive(false);
        connectionSuccessfulText.SetActive(false);
    }

    public void OnConnectToServerClicked()
    {
        Debug.Log("Connecting to server clicked.");
        connectToServerButton.SetActive(false);
        connectToServerText.SetActive(true);
        cancelButton.SetActive(true);
        Debug.Log("Connecting to server...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to server successfully!");
        connectToServerText.SetActive(false);
        connectionSuccessfulText.SetActive(true);

    }


    public void OnCancelClicked()
    {
        Debug.Log("Cancel Button Clicked");
        cancelButton.SetActive(false);
        connectToServerText.SetActive(false);
        connectToServerButton.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
