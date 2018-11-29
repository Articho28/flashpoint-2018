using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Firefighter : MonoBehaviour {

    [SerializeField] int actionPoints = 4;


    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        //this code snippet is in the update method ONLY for demoing the movement of the one firefighter.
        if (Input.GetMouseButtonDown(0)) {
            Vector3 cameraPos = Camera.main.transform.position;
            Vector3 mousePos = Input.mousePosition + new Vector3(0, 0, -cameraPos.z);
            Vector3 mousePosInWorldPoint = Camera.main.ScreenToWorldPoint(mousePos);

            //gets the middle coordinate of the unity unit where the mouse cursor is.
            Vector3 tilePos = GameManager.instance.WorldPointToTilePos(mousePosInWorldPoint);

            HighlightValidActions();

            //if action is valid, update player's position
            bool validAction = IsValidAction(tilePos);
            if (validAction) {
                transform.position = tilePos;
                //Reduce the action points
                //WAEL ADDED THIS, EVERYTHING BELOW
                actionPoints--;
                Debug.Log(actionPoints);
            }
            else{
                Debug.Log("Not enough AP");
            }
        }
    }

    bool IsValidAction(Vector3 tilePos) {
        //tile is outside the bounds of the board
        if(tilePos.x > 5 || tilePos.x < -5 || tilePos.y < -4 || tilePos.y > 4) {
            return false;
        }

        //tile is reachable with player's AP
        //WAEL MODIFIED THIS: I ADDED  "&& actionPoints > 0"
        if ((tilePos.x - transform.position.x) +
            (tilePos.y - transform.position.y) < actionPoints && actionPoints > 0) {
            return true;
        }

        //action is performable with player's AP

        return false;
    }

    void HighlightValidActions() {
        
    }
}
