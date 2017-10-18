using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetActions : NetworkBehaviour
{

    public enum Modes { ACTION_COLORABLE, ACTION_RESIZABLE, ACTION_MOVABLE }; //all possible actions

    public Modes mode = Modes.ACTION_COLORABLE; //action of the object


    bool transformed = false; //changed state

    float resetTime = 2.0f; //time before reset
    
    //color collection for change
    Color[] Palette =
    {
        Color.red,
        Color.blue,
        Color.green,
        Color.cyan,
        Color.magenta,
        Color.yellow
    };

    //synced params of object and syncHooks to update client objects
    [SyncVar(hook = "SyncColor")]
    public Color syncColor;

    [SyncVar(hook = "SyncScale")]
    public Vector3 syncScale;

    [SyncVar(hook = "SyncPosition")]
    public Vector3 syncPos;    

    //memory for reset
    Vector3 lastScale;
    Vector3 lastPos;

    //trigger for action
    public void RayHit()
    {
        //invoke server actions depending on mode
        switch (mode)
        {
            case Modes.ACTION_COLORABLE:
                ChangeColor();
                break;
            case Modes.ACTION_MOVABLE:
                ChangePos();
                break;
            case Modes.ACTION_RESIZABLE:
                ChangeSize();
                break;
        }

    }

#region procedures for changing
    public void ChangeColor()
    {
        //make sure color is really new
        Color changeColor;
        do
        {
            changeColor = (Color)Palette.GetValue(Random.Range(0, Palette.Length - 1));
        }
        while (GetComponent<MeshRenderer>().material.color == changeColor);

        syncColor = changeColor;
    }
    
    public void ChangeSize()
    {
        if (!transformed)
        {
            lastScale = transform.localScale;
            //transform.localScale = new Vector3(1, 0.3f, 1);
            syncScale = new Vector3(1, 0.3f, 1);

            transformed = true;

            Invoke("ResetSize", resetTime);
        }
    }

    public void ChangePos()
    {
        if (!transformed)
        {
            lastPos = transform.position;
            //transform.Translate(new Vector3(0, 3, 0));
            syncPos = transform.position + new Vector3(0, 3, 0);

            transformed = true;

            Invoke("ResetPos", resetTime);
        }
    }
#endregion

    //resets
    void ResetSize()
    {
        syncScale = lastScale;
        transformed = false;
    }

    void ResetPos()
    {
        syncPos = lastPos;
        transformed = false;
    }

    //apply new states
    void SyncColor(Color value)
    {
        //Debug.Log("SyncColor!");
        GetComponent<MeshRenderer>().material.color = value;
    }

    void SyncScale(Vector3 value)
    {
        //Debug.Log("SyncScale!");
        transform.localScale = value;
    }

    void SyncPosition(Vector3 value)
    {
        //Debug.Log("SyncPosition!");
        transform.position = value;
    }


    private void Awake()
    {
        //init all server states with values - clients get them after sync        
        if (isClient) return;

        syncPos = transform.position;
        syncScale = transform.localScale;
        syncColor = GetComponent<MeshRenderer>().material.color;        
    }
    
    public override void OnStartClient()
    {
        //if we're late at party - make sure we're synced
        SyncColor(syncColor);
        SyncScale(syncScale);
        SyncPosition(syncPos);
    }    
}
