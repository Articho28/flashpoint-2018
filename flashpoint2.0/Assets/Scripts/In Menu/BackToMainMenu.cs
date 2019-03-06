using UnityEngine.SceneManagement;
using UnityEngine;

public class BackToMainMenu : MonoBehaviour {

    public void switchScenes()
    {
        SceneManager.LoadScene("MAIN MENU",LoadSceneMode.Single);
    }
}
