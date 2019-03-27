using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.IO;

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

    //Game relevant variables
    public int buildingDamage;
    static int blackDice;
    static int redDice;

    //Network Options

    static Photon.Realtime.RaiseEventOptions  sendToAllOptions = new Photon.Realtime.RaiseEventOptions()
    {
        CachingOption = Photon.Realtime.EventCaching.DoNotCache,
        Receivers = Photon.Realtime.ReceiverGroup.All
    };



    public void Awake()
    {
        if (GM == null)
        {
            GM = this;
            GameStatus = FlashPointGameConstants.GAME_STATUS_SPAWNING_PREFABS;
            NumberOfPlayers = PhotonNetwork.CountOfPlayers;
            isFirstReset = true;
            buildingDamage = 0;
        }
        else
        {
            if (GM != this)
            {
                Destroy(GM);
                GM = this;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnAllPrefabsSpawned()
    {   
           

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

    public void DisplayPlayerTurn()
    {
        string playerName = PhotonNetwork.PlayerList[Turn - 1].NickName;
        GameUI.instance.UpdatePlayerTurnName(playerName);
    }

    public void DisplayToConsole(string message)
    {
        GameConsole.instance.FeedbackText.text = message;
    }

    public void DisplayToConsolePlayGame(int turn)
    {
        string playerName = PhotonNetwork.PlayerList[turn - 1].NickName;
        string message = "It's " + playerName + "'s turn!";
        GameConsole.instance.FeedbackText.text = message;
    }

    public void DisplayToConsolePlaceFirefighter(int turn)
    {
        string playerName = PhotonNetwork.PlayerList[turn - 1].NickName;
        string message = "It's " + playerName + "'s turn to place their FireFighter";
        GameConsole.instance.FeedbackText.text = message;
    }

    public static void advanceFire()
    {
        rollDice();
        Space targetSpace = StateManager.instance.spaceGrid.getGrid()[blackDice, redDice];
        Debug.Log("Found target space is : " + targetSpace.indexX + " and " + targetSpace.indexY);

        SpaceStatus sp = targetSpace.getSpaceStatus();

        object[] data = new object[] { targetSpace.worldPosition, targetSpace.indexX, targetSpace.indexY };

        if (sp == SpaceStatus.Fire)
        {
            Debug.Log("It's an explosion");

        }
        else if(sp == SpaceStatus.Smoke)
        {
            Debug.Log("It's turned to Fire.");
            PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.AdvanceFireMarker, data, sendToAllOptions, SendOptions.SendUnreliable);
        }
        else
        {
            Debug.Log("It's turned to Smoke");
            PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.AdvanceSmokeMarker, data, sendToAllOptions, SendOptions.SendUnreliable);

        }


    }

    static void rollDice()
    {
        //TODO reset proper randomization
        //System.Random r = new System.Random();
        //blackDice = r.Next(1, 9);
        //redDice = r.Next(1, 7);
        blackDice = 1;
        redDice = 6;

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

        //Increment turn.
        if (evCode == (byte)PhotonEventCodes.IncrementTurn)
        {
            Turn++;

            if (Turn > NumberOfPlayers)
            {
                if (isFirstReset)
                {
                    //change the status to play game
                    GameStatus = FlashPointGameConstants.GAME_STATUS_PLAY_GAME;
                    FiremanUI.instance.SetAP(4);
                    GameUI.instance.AddGameState(GameStatus);
                    isFirstReset = false;
                }
                Turn = 1;
                DisplayPlayerTurn();
                DisplayToConsolePlayGame(Turn);
            }
            else
            {
                if (isFirstReset)
                {
                    DisplayToConsolePlaceFirefighter(Turn);
                    DisplayPlayerTurn();
                }
            }
        }

        else if (evCode == (byte)PhotonEventCodes.PlaceInitialFireFighter)
        {
            Turn = 1;
            GameStatus = FlashPointGameConstants.GAME_STATUS_INITIALPLACEMENT;
            DisplayPlayerTurn();
            DisplayToConsolePlaceFirefighter(Turn);
            GameUI.instance.AddGameState(GameStatus);

        }
        else if (evCode == (byte) PhotonEventCodes.AdvanceFireMarker)
        {
            object[] dataReceived = eventData.CustomData as object[];
            Vector3 receivedPosition = (Vector3) dataReceived[0];
            int indexX = (int)dataReceived[1];
            int indexY = (int)dataReceived[2];

            Space targetSpace = StateManager.instance.spaceGrid.getGrid()[indexX, indexY];

            targetSpace.setSpaceStatus(SpaceStatus.Fire);
            GameObject newFireMarker = Instantiate(Resources.Load("PhotonPrefabs/Prefabs/FireMarker")) as GameObject;
            Vector3 newPosition = new Vector3(targetSpace.worldPosition.x, targetSpace.worldPosition.y, 0);
            newFireMarker.GetComponent<Transform>().position = newPosition;
            Debug.Log("It was placed at " + newPosition);
        }

        else if (evCode == (byte)PhotonEventCodes.AdvanceSmokeMarker)
        {


            object[] dataReceived = eventData.CustomData as object[];
            Vector3 receivedPosition = (Vector3)dataReceived[0];
            int indexX = (int)dataReceived[1];
            int indexY = (int)dataReceived[2];

            Space targetSpace = StateManager.instance.spaceGrid.getGrid()[indexX, indexY];


            targetSpace.setSpaceStatus(SpaceStatus.Fire);
            GameObject newFireMarker = Instantiate(Resources.Load("PhotonPrefabs/Prefabs/FireMarker")) as GameObject;
            Vector3 newPosition = new Vector3(targetSpace.worldPosition.x, targetSpace.worldPosition.y, -5);
            newFireMarker.GetComponent<Transform>().position = newPosition;
            Debug.Log("It was placed at " + newPosition);
        }

    }


}