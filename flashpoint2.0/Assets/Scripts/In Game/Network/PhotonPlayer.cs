﻿using System.IO;
using Photon.Pun;
using UnityEngine;
using ExitGames.Client.Photon;


public class PhotonPlayer : MonoBehaviour
{
    private PhotonView PV;
    public GameObject myAvatar;
    public int Id;
    public string PlayerName;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            int myPlayerNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Prefabs",
               "Fireman",
                   "Firefighter" + myPlayerNumber),
                   GamePlayersNetworkSetup.GS.initialPositions[myPlayerNumber - 1],
               Quaternion.identity, 0);
            myAvatar.GetComponent<GameUnit>().setPhysicalObject(myAvatar);
            myAvatar.GetComponent<GameUnit>().setType(FlashPointGameConstants.GAMEUNIT_TYPE_FIREMAN);

            Hashtable props = new Hashtable
            {
                {FlashPointGameConstants.PLAYER_READY_FOR_PLACEMENT, true }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

    }

    public void Initialize(int id, string name)
    {
        Id = id;
        PlayerName = name;
    }
}
