using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    #region Singleton
    public static GameManager instance;

    private void Awake() {
        instance = this;
    }

    #endregion
    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        //this code snippet is ONLY for the demo for spawning one firefighter
        if (FirefighterManager.instance.firefighters.Count == 0) {
            if (Input.GetMouseButtonDown(0)) {
                Vector3 cameraPos = Camera.main.transform.position;
                Vector3 mousePos = Input.mousePosition + new Vector3(0, 0, -cameraPos.z);
                Vector3 mousePosInWorldPoint = Camera.main.ScreenToWorldPoint(mousePos);
                Vector3 tilePos = GameManager.instance.WorldPointToTilePos(mousePosInWorldPoint);

                bool isValidSpawnTile = false;
                //if spawn point is valid, create firefighter
                if((tilePos.x == 4.5 || tilePos.x == -4.5) && tilePos.y <= 3.5 && tilePos.y >= -3.5) {
                    isValidSpawnTile = true;
                }
                else if((tilePos.y == 3.5 || tilePos.y == -3.5) && tilePos.x <= 4.5 && tilePos.x >= -4.5) {
                    isValidSpawnTile = true;
                }

                if (isValidSpawnTile) {
                    FirefighterManager.instance.AddFirefighter(tilePos);
                }
            }
        }
	}

    //gets the middle coordinate of the unity unit where the position is.
    public Vector3 WorldPointToTilePos(Vector3 pos) {
        float numX = Mathf.Round(pos.x);
        if (numX > pos.x) {
            pos.x = numX - 0.5f;
        }
        else if (numX < pos.x) {
            pos.x = numX + 0.5f;
        }

        float numY = Mathf.Round(pos.y);
        if (numY > pos.y) {
            pos.y = numY - 0.5f;
        }
        else if (numY < pos.y) {
            pos.y = numY + 0.5f;
        }

        return pos;
    }

}
