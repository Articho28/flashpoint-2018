﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class FirefighterMovement : MonoBehaviourPun
{

    private CharacterController myCC;
    private Transform initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        myCC = GetComponent<CharacterController>();
        initialPosition = base.photonView.GetComponent<Transform>();
    }


    private void Update()
    {

        if (base.photonView.IsMine && GameManager.GM.Turn == PhotonNetwork.LocalPlayer.ActorNumber && GameManager.GameStatus ==
        FlashPointGameConstants.GAME_STATUS_INITIALPLACEMENT)
        {
            if (Input.GetMouseButtonDown(0))
            {
                placeFireFighter();
                GameManager.GM.IncrementTurn();
            }
        }
    }



    public void placeFireFighter()
    {
        Space UserTargetInitialSpace = UserInputManager.instance.getLastSpaceClicked();
        Vector3 position = UserTargetInitialSpace.worldPosition;
        SpaceKind kind = UserTargetInitialSpace.getSpaceKind();


        //the data that will be transferred
        object[] datas = new object[] { base.photonView.ViewID, kind, position };

        Photon.Realtime.RaiseEventOptions options = new Photon.Realtime.RaiseEventOptions()
        {
            CachingOption = Photon.Realtime.EventCaching.DoNotCache,
            Receivers = Photon.Realtime.ReceiverGroup.All
        };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.placeFireFighter, datas, options, SendOptions.SendUnreliable);
    }


    //  =============== NETWORK SYNCRONIZATION SECTION ===============
    public void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    public void OnEvent(EventData eventData)
    {
        byte evCode = eventData.Code;

        //0: placing a firefighter
        if (evCode == (byte) PhotonEventCodes.placeFireFighter)
        {
            object[] data = eventData.CustomData as object[];

            if (data.Length == 3)
            {
                Debug.Log(data[0] + " is the viewID");
                if ((int)data[0] == base.photonView.ViewID)
                {
                    SpaceKind kind = (SpaceKind)data[1];
                    Vector3 position = (Vector3)data[2];
                    if (kind == SpaceKind.Outdoor)
                    {
                        initialPosition.position = position;
                    }
                }
            }
        }
    }

}