using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirefighterManager : MonoBehaviour {

    #region Singleton

    public static FirefighterManager instance;

    void Awake() {
        instance = this;
    }

    #endregion

    public List<Firefighter> firefighters;

    [SerializeField] Firefighter firefighterPrefab;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddFirefighter(Vector3 tilePos) {
        Firefighter newFirefighter = Instantiate<Firefighter>(firefighterPrefab);
        newFirefighter.GetComponent<Transform>().position = tilePos;

        firefighters.Add(newFirefighter);
    }
}
