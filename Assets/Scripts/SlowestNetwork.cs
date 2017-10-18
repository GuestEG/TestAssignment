using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//slowpoke class
[NetworkSettings(channel = 1, sendInterval = 5.0f)] 
public class SlowestNetwork : NetworkBehaviour
{
    [SyncVar(hook = "Poked")]
    public bool poke = false;

    bool poked = false;

    private void OnGUI()
    {
        if (poked) GUI.Label(new Rect(Screen.width - 200, 200, 200, 20), "Client will change it after 5 sec!");
    }

    void Poked(bool value)
    {
        poked = value;
    }
}