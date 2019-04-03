using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class FirefighterMovement : MonoBehaviourPun
{

    private CharacterController myCC;
    private Transform initialPosition;
    private PhotonView PV;

    static Photon.Realtime.RaiseEventOptions sendToAllOptions = new Photon.Realtime.RaiseEventOptions()
    {
        CachingOption = Photon.Realtime.EventCaching.DoNotCache,
        Receivers = Photon.Realtime.ReceiverGroup.All
    };

    // Start is called before the first frame update
    void Start()
    {
        myCC = GetComponent<CharacterController>();
        initialPosition = GetComponent<Transform>();
        PV = GetComponent<PhotonView>();
    }


    private void Update()
    {

        if (PV.IsMine && GameManager.GM.Turn == PhotonNetwork.LocalPlayer.ActorNumber && GameManager.GameStatus ==
        FlashPointGameConstants.GAME_STATUS_INITIALPLACEMENT)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (fireFighterHasBeenPlaced())
                {
                    GameManager.IncrementTurn();
                }
            }
        }
        
    }



    public bool fireFighterHasBeenPlaced()
    {
        Space UserTargetInitialSpace = UserInputManager.instance.getLastSpaceClicked();
        Vector3 position = new Vector3(UserTargetInitialSpace.worldPosition.x, UserTargetInitialSpace.worldPosition.y, -10);
        SpaceKind kind = UserTargetInitialSpace.getSpaceKind();

        if (PV.IsMine && UserTargetInitialSpace != null)
        {
            if (kind == SpaceKind.Outdoor)
            {
                initialPosition.position = position;
                return true;
            }
            else
            {
                GameConsole.instance.UpdateFeedback("Invalid placement. It has to be outside of the house!!");
                return false;
            }
        }
        return false;


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
        if (evCode == (byte) PhotonEventCodes.fireFighterPlaced)
        {
            //object[] data = eventData.CustomData as object[];

            //if (data.Length == 3)
            //{
            //    if ((int)data[0] == PV.ViewID)
            //    {
            //        SpaceKind kind = (SpaceKind)data[1];
            //        Vector3 position = (Vector3)data[2];
            //        if (kind == SpaceKind.Outdoor)
            //        {
            //            initialPosition.position = position;
            //        }
            //    }
            //}
            fireFighterHasBeenPlaced();
        }
    }

}