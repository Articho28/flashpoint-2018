﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class displayMessage : MonoBehaviour {

    public string username;

    public int maxMessage = 25;

    public GameObject chatPanel,textObject;
    public InputField chatBox;
    public Color playerMessage, info;

    [SerializeField]
    List<Message> messageList = new List<Message>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(chatBox.text != ""){
            if(Input.GetKeyDown(KeyCode.Return)){
                SendMessageToChat(username +": " + chatBox.text,Message.MessageType.playerMessage);
                chatBox.text = "";
            }
        }
        else{
            if(!chatBox.isFocused && Input.GetKeyDown(KeyCode.Return))
            {
                chatBox.ActivateInputField();
            }
        }

        if (!chatBox.isFocused){
            if (Input.GetKeyDown(KeyCode.Space))
                SendMessageToChat("Hello World!",Message.MessageType.info);
        }

	}

    public void SendMessageToChat(string text,Message.MessageType messageType){

        if(messageList.Count >= maxMessage){

            Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);

        }

        Message newMessage = new Message();

        newMessage.text = text;

        GameObject newText = Instantiate(textObject, chatPanel.transform);

        newMessage.textObject = newText.GetComponent<Text>();

        newMessage.textObject.text = newMessage.text;

        newMessage.textObject.color = MessageTypeColor(messageType);

        messageList.Add(newMessage);
    }

    Color MessageTypeColor(Message.MessageType messageType){
        Color color = info;

        switch(messageType){
            case Message.MessageType.playerMessage:
                color = playerMessage;
                break;
        }

        return color;
    }
}

[System.Serializable]
public class Message{

    public string text;
    public Text textObject;
    public MessageType messageType;
    public enum MessageType
    {
        playerMessage,
        info,
    }


}