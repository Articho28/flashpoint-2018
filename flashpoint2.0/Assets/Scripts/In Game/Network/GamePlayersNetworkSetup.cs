using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using ExitGames.Client.Photon;


public class GamePlayersNetworkSetup : MonoBehaviour
{

    private static GamePlayersNetworkSetup GS;
    public string status;
    [SerializeField]
    public ArrayList photonPlayers;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if (GamePlayersNetworkSetup.GS == null)
        {
            GamePlayersNetworkSetup.GS = this;
        }
        else
        {
            if (GamePlayersNetworkSetup.GS != this)
            {
                Destroy(GamePlayersNetworkSetup.GS);
                GamePlayersNetworkSetup.GS = this;
            }
        }
    }

   
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("This is Player " + PhotonNetwork.LocalPlayer.NickName);
            GameObject entry = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs",
               "PhotonPlayers",
                   "PhotonPlayer"),
               transform.position,
               Quaternion.identity, 0);
        }
    }

    void Update()
    {

    }
}
