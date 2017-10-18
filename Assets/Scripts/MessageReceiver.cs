using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MessageReceiver : MonoBehaviour {

    bool handled = false;

    public NetworkAttributesController controller;


    private void OnGUI()
    {
        if (handled) GUI.Label(new Rect(Screen.width - 200, 320, 200, 40), "Event received!");
    }

    // Use this for initialization
    void Start()
    {
        if (NetworkClient.active)
            controller.EventSync += EventHandler;
    }

    void EventHandler(bool click)
    {
        handled = true;
    }
}
