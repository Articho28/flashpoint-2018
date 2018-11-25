using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		
	}

    public void switchScenes(){
        SceneManager.LoadScene("MAIN MENU",LoadSceneMode.Single);
    }
}
