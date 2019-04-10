using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInputManager : MonoBehaviour
{
    #region
    public static UserInputManager instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    GameObject lastObjectClicked;
    Space lastSpaceClicked;

    public KeyCode validInput;
    public bool crIsRunning;

    // Start is called before the first frame update
    void Start()
    {
        crIsRunning = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        { // if left button pressed
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);             RaycastHit hit;             if (Physics.Raycast(ray, out hit))
            {
                lastObjectClicked = hit.transform.gameObject;
                 if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Door"))
                {                     Debug.Log("a door was clicked");                 }                 else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Wall"))
                {                     Debug.Log("a wall was clicked");                 }                 else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Board"))
                {                     Debug.Log("a tile was clicked");
                    lastSpaceClicked = StateManager.instance.spaceGrid.WorldPointToSpace(lastObjectClicked.transform.position);

                    Debug.Log("space clicked x: " + lastSpaceClicked.indexX + "space clicked y: " + lastSpaceClicked.indexY);                 }

            }         }
    }

    public GameObject getLastObjectClicked()
    {
        return lastObjectClicked;
    }

    public Space getLastSpaceClicked()
    {
        return lastSpaceClicked;
    }

    public IEnumerator waitForValidUserInput(KeyCode[] codes, string message, Fireman fireman) {
        bool pressed = false;
        while (!pressed) {
            crIsRunning = true;

            GameConsole.instance.UpdateFeedback(message);
            foreach (KeyCode k in codes) {
                if (Input.GetKey(k)) {
                    pressed = true;
                    validInput = k;

                    crIsRunning = false;
                    break;
                }
            }

            yield return new WaitForSeconds(0.15f);
        }
    }

    public int keyCodeToIntKey(KeyCode keyCode) { 
        switch(keyCode) {
            case (KeyCode.Alpha0): return 0;
            case (KeyCode.Alpha1): return 1;
            case (KeyCode.Alpha2): return 2;
            case (KeyCode.Alpha3): return 3;
            default : return -1;
        }
    }
}

