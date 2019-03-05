using UnityEngine.SceneManagement;
using UnityEngine;

public class ToPlayGame : MonoBehaviour {

    public void switchScenes(){
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
}
