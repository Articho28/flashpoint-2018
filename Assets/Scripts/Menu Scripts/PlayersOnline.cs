using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayersOnline : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void switchScenes()
    {
        SceneManager.LoadScene("MENU WITH PLAYERS ONLINE",LoadSceneMode.Single);
    }
}
