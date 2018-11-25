using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToLoginMenu : MonoBehaviour {

    public void switchScenes(){
        SceneManager.LoadScene("LoginMenu", LoadSceneMode.Single);
    }


}
