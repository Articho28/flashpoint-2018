using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLostUI : MonoBehaviour
{
    public Text GameLostText;
    public Button GameLostButton;
    // Start is called before the first frame update
    void Start()
    {
        GameLostText = GetComponent<Text>();
        GameLostButton = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void returnToLobby()
    {
        Debug.Log("RETURNING TO LOBBY");
    }
}
