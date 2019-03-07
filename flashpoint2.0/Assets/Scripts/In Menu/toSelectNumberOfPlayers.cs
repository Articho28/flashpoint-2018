using UnityEngine.SceneManagement;
using UnityEngine;

public class toSelectNumberOfPlayers : MonoBehaviour {

    public void SwitchScene(){
        SceneManager.LoadScene("PlayerNumberSelectionScreenFamilyGame", LoadSceneMode.Single);
    }
}
