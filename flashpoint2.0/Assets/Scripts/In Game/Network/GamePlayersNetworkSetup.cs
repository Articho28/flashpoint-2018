using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using ExitGames.Client.Photon;


public class GamePlayersNetworkSetup : MonoBehaviourPunCallbacks
{

    public static GamePlayersNetworkSetup GS;
    public string status;

    [SerializeField]
    public Dictionary<int, GameObject> photonPlayersPrefabs;
    public Vector3[] initialPositions;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if (GamePlayersNetworkSetup.GS == null)
        {
            GamePlayersNetworkSetup.GS = this;
            initialPositions = new Vector3[PhotonNetwork.CountOfPlayers];
            Vector3 topPosition = new Vector3(-8.9f, 0.32f, 0);
            for (int i = 0; i < initialPositions.Length; i++)
            {
                initialPositions[i] = topPosition;
                topPosition = new Vector3(topPosition.x, topPosition.y - 0.92f, 0);
            }


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
            photonPlayersPrefabs.Add(PhotonNetwork.LocalPlayer.ActorNumber, entry);
        }
    }

    public void  OnPlayerLeftLobby(Photon.Realtime.Player otherPlayer)
    {
        photonPlayersPrefabs.Remove(otherPlayer.ActorNumber);
    }

}
