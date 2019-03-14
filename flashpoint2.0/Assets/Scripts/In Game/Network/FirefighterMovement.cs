using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FirefighterMovement : MonoBehaviour
{

    private PhotonView PV;
    private CharacterController myCC;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        myCC = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine && GameManager.Turn == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            PlaceFirefighter();
        }

    }

    public void PlaceFirefighter()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is trying to Move! ");
    }
}
