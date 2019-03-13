using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PhotonPlayer : MonoBehaviour
{

    public string playerName;
    private PhotonView PV;
    public GameObject myAvatar;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            string tempName = PhotonNetwork.LocalPlayer.NickName;
            Debug.Log(tempName +  " sees my Photon View.");
        }
    }

    public void Initialize(string name)
    {
        playerName = name;
    }


}
