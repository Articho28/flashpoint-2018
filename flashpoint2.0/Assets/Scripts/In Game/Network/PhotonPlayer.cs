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
            int myPlayer = PhotonNetwork.LocalPlayer.ActorNumber;
            Debug.Log(myPlayer + " is " + PhotonNetwork.LocalPlayer.NickName + " ActorNumber");
            myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs",
               "fireman",
                   "Firefighter" + myPlayer),
                   GamePlayersNetworkSetup.GS.initialPositions[myPlayer - 1],
               Quaternion.identity, 0);

            Hashtable props = new Hashtable
            {
                {"IsPlayerReadyToBePlaced", true }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }


}
