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

    GameObject objectClicked;
    Space spaceClicked;
    Wall wallClicked;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) { // if left button pressed
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);             RaycastHit hit;             if (Physics.Raycast(ray, out hit)) {
                objectClicked = hit.transform.gameObject;
                 if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Door")) {                     Debug.Log("a door was clicked");                 }                 else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Wall")) {                     Debug.Log("a wall was clicked");
                    wallClicked = objectClicked.transform.gameObject.GetComponent<Wall>();                 }                 else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Board")) {                     Debug.Log("a tile was clicked");
                    spaceClicked = StateManager.instance.spaceGrid.WorldPointToSpace(objectClicked.transform.position);                 }

            }         }
    }

    public GameObject getObjectClicked() {
        return objectClicked;
    }

    public Space getSpaceClicked() {
        return spaceClicked;
    }
}
