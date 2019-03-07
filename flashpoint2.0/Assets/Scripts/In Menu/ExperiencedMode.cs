using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ExperiencedMode : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		
	}
    public void SwitchScenes()
    {
        SceneManager.LoadScene("DIFFICULTY LEVEL",LoadSceneMode.Single);
    }
}
