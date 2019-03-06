using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonLobby : MonoBehaviour
{


    public static PhotonLobby lobby;

    public GameObject connectToServerButton;
    public GameObject connectToServerText;
    public GameObject cancelButton;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Showing button.");
        connectToServerText.SetActive(false);
        connectToServerButton.SetActive(true);
        cancelButton.SetActive(false);
    }

    public void OnConnectToServerClicked()
    {
        Debug.Log("Connecting to server clicked.");
        connectToServerButton.SetActive(false);
        connectToServerText.SetActive(true);
        cancelButton.SetActive(true);
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
