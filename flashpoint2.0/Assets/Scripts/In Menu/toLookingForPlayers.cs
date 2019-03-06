using UnityEngine;
using UnityEngine.SceneManagement;

public class toLookingForPlayers : MonoBehaviour {

    public void switchScenes(){
        SceneManager.LoadScene("LOOKING FOR PLAYERS", LoadSceneMode.Single);
    }
}
