using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class chat : MonoBehaviourPun
{
    public ScrollRect scrollRect;
    public Button button;
    public InputField input;
    PhotonView PV;

    static Photon.Realtime.RaiseEventOptions sendToAllOptions = new Photon.Realtime.RaiseEventOptions()
    {
        CachingOption = Photon.Realtime.EventCaching.DoNotCache,
        Receivers = Photon.Realtime.ReceiverGroup.All
    };

    // Start is called before the first frame update
    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        button = GetComponent<Button>();
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void buttonIsClicked()
    {
        string message = input.text;
        object[] data = { message, base.photonView.ViewID, PhotonNetwork.LocalPlayer.NickName };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.SendMessage, data, sendToAllOptions, SendOptions.SendReliable);
    }

    public void sendMessageToPlayers(string Message, string playerNickname)
    {
        string messageToBeSent = playerNickname + ": ";
        messageToBeSent += Message;

        GameObject text = GameObject.Find("/Canvas/Chat/Scroll View/Viewport/Text");

        text.GetComponent<Text>().text = messageToBeSent;
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
        if (evCode == (byte)PhotonEventCodes.SendMessage)
        {
            object[] receivedData = eventData.CustomData as object[];
            string message = (string)receivedData[0];
            int viewID = (int)receivedData[1];
            string PlayerNickname = (string)receivedData[2];

            if (viewID == base.photonView.ViewID)
            {
                sendMessageToPlayers(message,PlayerNickname);
            }
        }
    }
}
