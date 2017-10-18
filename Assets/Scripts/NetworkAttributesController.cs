using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkAttributesController : NetworkBehaviour {
    //super-simple network attribute controller
    bool ClientAtt = false;
    bool ClientCallAtt = false;
    bool ServerAtt = false;
    bool ServerCallAtt = false;
    bool CommandAtt = false;
    bool ClientRpcAtt = false;
    bool TargetAtt = false;


    public delegate void SyncEventDelegate(bool click);

    [SyncEvent]
    public event SyncEventDelegate EventSync;

    [SyncVar]
    int counter = 0;
    
    private void OnGUI()
    {
        /*
        GUI.Box (Rect (0,0,100,50), "Top-left");
        GUI.Box (Rect (Screen.width - 100,0,100,50), "Top-right");
        GUI.Box (Rect (0,Screen.height - 50,100,50), "Bottom-left");
        GUI.Box (Rect (Screen.width - 100,Screen.height - 50,100,50), "Bottom-right");
        */

        if (ClientAtt) GUI.Label(new Rect(Screen.width - 200, 0, 200, 20), "This will show on [Client]!");
        if (ClientCallAtt) GUI.Label(new Rect(Screen.width - 200, 40, 200, 20), "This will show on [ClientCallback]!");
        if (ServerAtt) GUI.Label(new Rect(Screen.width - 200, 80, 200, 20), "This will show on [Server]!");
        if (ServerCallAtt) GUI.Label(new Rect(Screen.width - 250, 120, 250, 20), "This will show on [ServerCallback]!");

        if (CommandAtt) GUI.Label(new Rect(Screen.width - 200, 160, 200, 20), "[Command] from a client!");
        if (isClient && GUI.Button(new Rect(Screen.width - 350, 160, 150, 40), "Command")) CmdCommandAttribute(true);

        if (isServer && GUI.Button(new Rect(Screen.width - 350, 200, 150, 40), "5 second delay")) GetComponent<SlowestNetwork>().poke = !GetComponent<SlowestNetwork>().poke;

        if (isServer && GUI.Button(new Rect(Screen.width - 350, 240, 150, 40), "ClientRPC")) RpcClientRpcAttribute();
        if (ClientRpcAtt) GUI.Label(new Rect(Screen.width - 200, 240, 200, 20), "This is [ClientRPC]!");

        if (isClient && GUI.Button(new Rect(Screen.width - 350, 280, 150, 40), "TargetRpc")) CmdTargetAttribute();
        if (TargetAtt) GUI.Label(new Rect(Screen.width - 200, 280, 200, 20), "You touched that button!");

        if (GUI.Button(new Rect(Screen.width - 350, 320, 150, 40), "Send [SyncEvent]"))
        {
            if (isClient) CmdGenEvent();
            else EventSync(true);
        }

        if (GUI.Button(new Rect(Screen.width - 350, 360, 150, 40), "Add +100 to [SyncVar]"))
        {
            if (isClient) CmdAddSyncVar();
            else counter += 100;
        }
        GUI.Label(new Rect(Screen.width - 200, 360, 200, 20), "SyncVar = " + counter);


    }

    //will execute only on client side and will warn on server
    [Client]
    void ClientAttribute()
    {
        ClientAtt = true;
    }

    //will execute only on client side
    [Client]
    void ClientCallbackAttribute()
    {
        ClientCallAtt = true;
    }

    //will execute only on server side and will warn on client
    [Server]
    void ServerAttribute()
    {
        ServerAtt = true;
    }

    //will execute only on server side silently
    [ServerCallback]
    void ServerCallbackAttribute()
    {
        ServerCallAtt = true;
    }

    //will execute server-side by client
    [Command]
    void CmdCommandAttribute(bool value)
    {
        CommandAtt = true;
    }

    [ClientRpc]
    void RpcClientRpcAttribute()
    {
        ClientRpcAtt = true;
    }

    [Command]
    void CmdTargetAttribute()
    {
        TargetAttribute(connectionToClient);
    }

    [TargetRpc]
    void TargetAttribute(NetworkConnection target)
    {
        TargetAtt = true;
    }

    [Command]
    void CmdGenEvent()
    {
        EventSync(true);
    }

    [Command]
    void CmdAddSyncVar()
    {
        counter += 100;
    }

    private void Start()
    {
        //execute all the procedures!
        ClientAttribute();
        ClientCallbackAttribute();
        ServerAttribute();
        ServerCallbackAttribute();

    }

    // Update is called once per frame
    void Update () {
		
	}
}

