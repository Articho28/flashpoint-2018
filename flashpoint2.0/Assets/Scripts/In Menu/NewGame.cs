using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class NewGame: MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		
	}
    public void switchScenes (){
        SceneManager.LoadScene("SELECT GAME MODE",LoadSceneMode.Single);
    }
    public void switchScenesPlayersOnline()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 4);
    }
}
