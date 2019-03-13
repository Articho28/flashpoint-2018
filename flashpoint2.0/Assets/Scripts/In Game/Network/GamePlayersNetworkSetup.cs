﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class GamePlayersNetworkSetup : MonoBehaviour
{

    private static GamePlayersNetworkSetup GS;
    public string status;
    public ArrayList players;

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

        if (PhotonNetwork.IsMasterClient)
        {
            foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
            {
                Debug.Log("Master is looking at " + p.NickName);
                if (PhotonNetwork.IsConnected)
                {
                    GameObject entry = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs",
              "PhotonPlayers",
                  "PhotonPlayer"),
              transform.position,
              Quaternion.identity, 0);
                    entry.GetComponent<PhotonPlayer>().Initialize(PhotonNetwork.LocalPlayer.NickName);

                }

            }
        }

       
    }
}
