using UnityEngine.SceneManagement;
using UnityEngine;

public class toSelectNumberOfPlayers : MonoBehaviour {

    public void switchScene(){
        SceneManager.LoadScene("PlayerNumberSelectionScreen", LoadSceneMode.Single);
    }
}
