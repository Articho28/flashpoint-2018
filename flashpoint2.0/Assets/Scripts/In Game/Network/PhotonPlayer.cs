using System.IO;
using Photon.Pun;
using UnityEngine;
using ExitGames.Client.Photon;


public class PhotonPlayer : MonoBehaviour
{

    private PhotonView PV;
    public GameObject myAvatar;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            int myPlayerNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs",
               "fireman",
                   "Firefighter" + myPlayerNumber),
                   GamePlayersNetworkSetup.GS.initialPositions[myPlayerNumber - 1],
               Quaternion.identity, 0);

            Hashtable props = new Hashtable
            {
                {FlashPointGameConstants.PLAYER_READY_FOR_PLACEMENT, true }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }


}
