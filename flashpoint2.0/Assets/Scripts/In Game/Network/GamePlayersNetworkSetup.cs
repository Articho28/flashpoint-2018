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
    public ArrayList photonPlayers;

    private void OnEnable()
    {
        if (GamePlayersNetworkSetup.GS == null)
        {
            GamePlayersNetworkSetup.GS = this;
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
            entry.GetComponent<PhotonPlayer>().Initialize(PhotonNetwork.LocalPlayer.NickName);
            photonPlayers.Add(entry);
        }
              
    }
}
