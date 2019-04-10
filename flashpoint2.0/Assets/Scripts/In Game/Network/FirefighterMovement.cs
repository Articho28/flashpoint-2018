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

        if(UserTargetInitialSpace != null) {
            Vector3 position = new Vector3(UserTargetInitialSpace.worldPosition.x, UserTargetInitialSpace.worldPosition.y, -10);
            SpaceKind kind = UserTargetInitialSpace.getSpaceKind();

            if (PV.IsMine) {
                if (kind == SpaceKind.Outdoor) {
                    initialPosition.position = position;


                    object[] data = { UserTargetInitialSpace.indexX, UserTargetInitialSpace.indexY, PV.ViewID };

                    PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.FireFighterPlacedProperly, data, sendToAllOptions, SendOptions.SendReliable);

                    return true;
                }
                else {
                    GameConsole.instance.UpdateFeedback("Invalid placement. It has to be outside of the house!!");
                    return false;
                }
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
        if (evCode == (byte) PhotonEventCodes.FireFighterPlacedProperly)
        {
            object[] receivedData = eventData.CustomData as object[];
            int col = (int)receivedData[0];
            int row = (int)receivedData[1];
            int viewID = (int)receivedData[2];

            if (viewID == PV.ViewID)
            {
                Space space = StateManager.instance.spaceGrid.getGrid()[col, row];
                Fireman curr = this.GetComponentInParent<Fireman>();
                curr.setCurrentSpace(space);
                space.addOccupant(curr);

                object[] dataToSend = { col, row, viewID };
                PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.UpdateSpaceReferenceToFireman, dataToSend, sendToAllOptions, SendOptions.SendReliable);
            }
        }
    }

}