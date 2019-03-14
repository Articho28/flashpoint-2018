using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;

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
              new Vector3(0,0,-5),
               Quaternion.identity, 0);
            myAvatar.SetActive(false);
        }
    }




}
