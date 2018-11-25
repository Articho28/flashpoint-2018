using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class FamilyMode : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		
	}
    public void switchScenes()
    {
        SceneManager.LoadScene("LOOKING FOR PLAYERS",LoadSceneMode.Single);
    }
}

