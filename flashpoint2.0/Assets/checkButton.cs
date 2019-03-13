using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class checkButton : MonoBehaviour
{
    // Start is called before the first frame update
    public void Button () {
    	Text t = transform.Find ("Text").GetComponent <Text>();
    	t.text = "I was pressed";


    }

    public void onClick(){
        Debug.Log("Clicked");
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
