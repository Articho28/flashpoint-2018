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
    public bool isFirstReset;

    [SerializeField]
    GameObject GameUIPanel;

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
            isFirstReset = true;
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

    // Update is called once per frame
    void Update()
    {

    }

    public void OnAllPrefabsSpawned()
    {
        GameStatus = FlashPointGameConstants.GAME_STATUS_INITIALPLACEMENT;
        GameUI.instance.AddGameState(GameStatus);
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

        Photon.Realtime.RaiseEventOptions options = new Photon.Realtime.RaiseEventOptions()
        {
            CachingOption = Photon.Realtime.EventCaching.DoNotCache,
            Receivers = Photon.Realtime.ReceiverGroup.All
        };

        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.PlaceInitialFireFighter, null, options, SendOptions.SendUnreliable);

    }

    
    public static void IncrementTurn()
    {

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
                if (isFirstReset)
                {
                    //change the status to play game
                    Debug.Log("All firefighters have been placed!");
                    GameStatus = FlashPointGameConstants.GAME_STATUS_PLAY_GAME;
                    GameUI.instance.AddGameState(GameStatus);
                    isFirstReset = false;
                }
                Turn = 1;
            }
        }
        else if (evCode == (byte)PhotonEventCodes.PlaceInitialFireFighter)
        {
            Turn = 1;
            GameStatus = FlashPointGameConstants.GAME_STATUS_INITIALPLACEMENT;
        }

    }
}