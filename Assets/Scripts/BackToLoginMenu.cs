using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToLoginMenu : MonoBehaviour {

    public void switchScenes(){
        SceneManager.LoadScene("LOGIN MENU", LoadSceneMode.Single);
    }


}
