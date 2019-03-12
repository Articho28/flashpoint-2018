using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;


public class PlayerEntry : MonoBehaviour
{

    public Text PlayerNameText;
    public Button PlayerReadyButton;
    public Image PlayerReadyImage;

    private int ownerId;
    private bool isPlayerReady;


    // Start is called before the first frame update
    void Start()
    {

       
            PlayerReadyButton.onClick.AddListener(() =>
            {
                isPlayerReady = !isPlayerReady;
                SetPlayerReady(isPlayerReady);
                Hashtable props = new Hashtable() { { "IsPlayerReady", isPlayerReady } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                //TODO call callback on player properties changed. 
            });

    }

    public void SetPlayerReady(bool playerReady)
    {
        PlayerReadyButton.GetComponentInChildren<Text>().text = playerReady ? "Ready!" : "Ready?";

        PlayerReadyImage.enabled = playerReady;
    }

    public void Initialize(string name)
    {
        PlayerNameText.text = name;
        PlayerReadyButton.gameObject.SetActive(true);
        SetPlayerReady(false);
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
