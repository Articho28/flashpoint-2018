using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class sendMessage : MonoBehaviour {
    //name of player
    string playerName;

    //this function will be called once a user clicks a button
    public void uponClick(){
        //get the button's text field
        var buttonText = GetComponentInChildren<Text>();

        //get the name of the player the user has clicked on
        playerName = buttonText.text;


        //turn off all other buttons
        Button[] buttons = FindObjectsOfType<Button>();

        //iterating through the array
        foreach (Button b in buttons){

            //name of the button
            var textButton = b.name;

            //disable the player buttons only
            if (textButton.Equals("Player") || textButton.Equals("PLAYERS ONLINE")){
                disable(b);
            }
        }

        //iterate through the hidden objects and enable the scroll rect
        ScrollRect[] scrollRects = Resources.FindObjectsOfTypeAll<ScrollRect>();
        foreach (ScrollRect sc in scrollRects){
            var label = sc.GetComponentInChildren<Text>();
            //change the label name to the intended player to text
            label.text = playerName;
            sc.gameObject.SetActive(true);
        }

    }

    //Function to disable a button
    private void disable(Button button){
        button.gameObject.SetActive(false);
    }
}
