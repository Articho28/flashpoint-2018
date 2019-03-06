using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Register : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		
	}

    //switching to the next scene
    public void switchScenes(){
        SceneManager.LoadScene("CreateAnAccount",LoadSceneMode.Single);
    }
}
