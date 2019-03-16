using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FirefighterMovement : MonoBehaviour
{

    private PhotonView PV;
    private CharacterController myCC;
    private bool IsFinished;
    //private GameObject myAvatar;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        myCC = GetComponent<CharacterController>();
        //myAvatar = GetComponent<PhotonPlayer>().myAvatar;
        IsFinished = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (PV.IsMine && GameManager.Turn == PhotonNetwork.LocalPlayer.ActorNumber && GameManager.GameStatus == 
        FlashPointGameConstants.GAME_STATUS_INITIALPLACEMENT && !IsFinished)
        {
            IsFinished = PlaceFirefighter();
            GameManager.IncrementTurn();
        }

    }

    public bool PlaceFirefighter()
    {
        //Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is trying to Move! ");
        Space UserTargetInitialSpace = UserInputManager.instance.getLastSpaceClicked();
        this.GetComponent<Transform>().transform.position = UserTargetInitialSpace.worldPosition;
        return true;

    }
}
