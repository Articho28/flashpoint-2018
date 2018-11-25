using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class StartScene : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		
	}

    //function to move to the next scene
    public void switchScenes(){
        SceneManager.LoadScene("LoginMenu",LoadSceneMode.Single);
    }
}
