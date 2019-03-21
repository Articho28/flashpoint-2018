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
        IsFinished = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine && GameManager.Turn == PhotonNetwork.LocalPlayer.ActorNumber && GameManager.GameStatus == 
        FlashPointGameConstants.GAME_STATUS_INITIALPLACEMENT && !IsFinished)
        {
            IsFinished = PlaceFirefighter();
            //if the player has selected a starting position, increment Turn. otherwise, wait till he select a starting position
            if (IsFinished)
            {
                GameManager.IncrementTurn();
            }
        }

    }

    public bool PlaceFirefighter()
    {
        //Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is trying to Move! ");
        if (Input.GetMouseButtonDown(0))
        {
            Space UserTargetInitialSpace = UserInputManager.instance.getLastSpaceClicked();
            //check if the space is outside or not
            if (UserTargetInitialSpace.getSpaceKind() == SpaceKind.Outdoor)
            {
                PV.GetComponent<Transform>().position = UserTargetInitialSpace.worldPosition;
                return true;
            }
            Debug.Log("Must be an outdoor space!!");
            return false;

        }
        else
        {
            return false;
        }

    }
}
