using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInputManager : MonoBehaviour
{
    #region
    public static UserInputManager instance;

    private void Awake() {
        instance = this;
    }

    #endregion

    GameObject lastObjectClicked;
    Space lastSpaceClicked;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) { // if left button pressed
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);             RaycastHit hit;             if (Physics.Raycast(ray, out hit)) {
                lastObjectClicked = hit.transform.gameObject;
                 if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Door")) {                     Debug.Log("a door was clicked");                 }                 else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Wall")) {                     Debug.Log("a wall was clicked");                 }                 else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Board")) {                     Debug.Log("a tile was clicked");
                    lastSpaceClicked = StateManager.instance.spaceGrid.WorldPointToSpace(lastObjectClicked.transform.position);                 }

            }         }
    }

    public GameObject getLastObjectClicked() {
        return lastObjectClicked;
    }

    public Space getLastSpaceClicked() {
        return lastSpaceClicked;
    }
}
