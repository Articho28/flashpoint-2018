using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class NewGame: MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		
	}
    public void SwitchScenes (){
        SceneManager.LoadScene("SELECT GAME MODE",LoadSceneMode.Single);
    }
    public void SwitchScenesPlayersOnline()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 4);
    }
}
