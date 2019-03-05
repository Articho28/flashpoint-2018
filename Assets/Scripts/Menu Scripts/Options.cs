using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Options : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		
	}
    public void switchScenes()
    {
        SceneManager.LoadScene("OPTIONS", LoadSceneMode.Single);
    }
}
