using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Firefighter : MonoBehaviour {

<<<<<<< HEAD
    public int actionPoints = 4;

    // Update is called once per frame
    void Update() {
=======
    [SerializeField] int actionPoints = 4;
    //WAEL ADDED THIS
    string updateAP = "Player 1: Arty\nAP: ";


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

                //WAEL ADDED THIS, EVERYTHING BELOW

                //Reduce the action points
                actionPoints--;

                //Find all the scroll rect objects, navigate PlayerID UI and update its text.
                ScrollRect [] scrollRects = FindObjectsOfType<ScrollRect>();
                foreach (ScrollRect sr in scrollRects)
                {
                    if (sr.name.Equals("PlayerID UI")){
                        Text srText = sr.GetComponentInChildren<Text>();//get the text of the component
                        srText.text = updateAP + actionPoints; //update the text
                    }
                }
            }
            else{//if there not enoug AP, write to console.
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
>>>>>>> 50802cab98cdda003986b01b122c709ab815482d
    }

    public void move(Vector3 worldPosition) {
        
    }
}
