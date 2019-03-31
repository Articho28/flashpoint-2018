using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameWonUI : MonoBehaviour
{
    public Text GameWonText;

    [Header("Return to Lobby")]
    public Button returnToLobbyButton;

    [Header("Continue Playing")]
    public Button continuePlayingButton;
    // Start is called before the first frame update
    void Start()
    {
        GameWonText = GetComponent<Text>();
        returnToLobbyButton = GetComponent<Button>();
        continuePlayingButton = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void contiuePlaying()
    {
        GameManager.GM.setActivePrefabs("won", false);
    }

    public void returnToLobby()
    {
        Debug.Log("RETURNING TO LOBBY");
    }
}
