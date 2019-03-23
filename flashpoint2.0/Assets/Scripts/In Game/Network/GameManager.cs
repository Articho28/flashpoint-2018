using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPun
{
    //Initialize Singleton.
    public static GameManager GM;

    //Variables for game statsu and turn.
    public static string GameStatus;
    public int Turn;
    //Local store of NumberOfPlayers.
    public static int NumberOfPlayers;

    [SerializeField]
    public static Dictionary<int, PhotonPlayer> playerPrefabs;

    public void Awake()
    {
        if (GM == null)
        {
            GM = this;
            GameStatus = FlashPointGameConstants.GAME_STATUS_SPAWNING_PREFABS;
            playerPrefabs = new Dictionary<int, PhotonPlayer>();
            NumberOfPlayers = PhotonNetwork.CountOfPlayers;
        }
        else
        {
            if (GM != this)
            {
                Destroy(GM);
                GM = this;
            }
        }
        Debug.Log("The GameManager was created.");
        Debug.Log("The GameStatus is at first:  " + GameStatus);
    }




    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameStatus == FlashPointGameConstants.GAME_STATUS_SPAWNING_PREFABS)
        {
            //Nothing to do here, just wait for the spawning to be done.
        }

        else if (GameStatus == FlashPointGameConstants.GAME_STATUS_INITIALPLACEMENT)
        {
            //Implement Place firefighter function.
            if (Turn <= PhotonNetwork.CountOfPlayers)
            {

                /*
                bool FireFighterIsPlaced = PlaceInitialFireFighter(Turn);
                if (FireFighterIsPlaced)
                {
                    IncrementTurn();
                }*/
            }
            //Debug.Log("Everyone should have chosen a location.");

        }
    }

    public void OnAllPrefabsSpawned()
    {

        GameManager.GameStatus = FlashPointGameConstants.GAME_STATUS_INITIALPLACEMENT;
        Turn = 1;


        if (PhotonNetwork.IsMasterClient)
        {
            PhotonPlayer[] photonPlayers = FindObjectsOfType<PhotonPlayer>();
            if (photonPlayers[0] != null)
            {

                for (int i = 0; i < photonPlayers.Length; i++)
                {
                    Debug.Log(photonPlayers[i].PlayerName + " Added to Dictionary ");
                    Debug.Log(photonPlayers[i]);

                    playerPrefabs.Add(photonPlayers[i].Id, photonPlayers[i]);

                }
            }
            else
            {
                Debug.Log("did not find the photonPlayers");
            }
        }
    }

    public bool PlaceInitialFireFighter(int Player)
    {
        Debug.Log("It's " + PhotonNetwork.PlayerList[Player - 1].NickName + " 's turn to place his firefighter!");

        //TODO link to PlayerMovement Script. 
        bool PlayerHasPlacedFirefighter = false;
        return PlayerHasPlacedFirefighter;
    }

    
    public void IncrementTurn()
    {
        //Turn++;

        //if (Turn > NumberOfPlayers)
        //{
        //    Turn = 1;
        //}

        Photon.Realtime.RaiseEventOptions options = new Photon.Realtime.RaiseEventOptions()
        {
            CachingOption = Photon.Realtime.EventCaching.DoNotCache,
            Receivers = Photon.Realtime.ReceiverGroup.All
        };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.IncrementTurn, null, options, SendOptions.SendUnreliable);
    }



//    ================ NETWORK SYNCHRONIZATION SECTION =================
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
        if (evCode == (byte)PhotonEventCodes.IncrementTurn)
        {
            Turn++;

            if (Turn > NumberOfPlayers)
            {
                Debug.Log("RESETTING TURN TO 1");
                Turn = 1;
            }
            Debug.Log("Current turn is now " + Turn);
        }


    }
}