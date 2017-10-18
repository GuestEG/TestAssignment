using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{

    float movementSpeed = 5.0f; //distance per second
    float rotationSpeed = 270.0f; //degrees per second

    float raycastMaxDistance = 100.0f; //meters

    PlayerFire gunScript = null;

    private void Awake()
    {
        gunScript = GetComponent<PlayerFire>();
        if (!gunScript) Debug.LogError("Can't find PlayerFire component!", this);
    }

    // Update is called once per frame
    void Update()
    {

        //if this is local player - we can handle local input
        if (isLocalPlayer)
        {   
            //z axis = forward
            Vector3 movDir = new Vector3(0, 0, Input.GetAxis("Vertical"));
            //y axis = vertical
            Vector3 rotDirEuler = new Vector3(0, Input.GetAxis("Horizontal"), 0);

            transform.Translate(movDir * movementSpeed * Time.deltaTime);
            transform.Rotate(rotDirEuler * rotationSpeed * Time.deltaTime);
            
            //"Jump" equals "Space bar" by default            
            if (Input.GetButtonDown("Jump") && gunScript != null)
            {
                gunScript.CmdFireBullet();
            }
            
            //Left mouse button make click on objects
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                //cast it from main camera
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, raycastMaxDistance))
                {
                    //if we hit something
                    //make sure we hit proper target and call its server command
                    NetActions actions = hit.transform.GetComponent<NetActions>();
                    if (actions)
                    {
                        CmdHitObject(actions.netId);
                    }
                }
            }
            
        }
    }

    [Command]
    void CmdHitObject(NetworkInstanceId objId)
    {
        //invoke action on server object
        GameObject localObject = NetworkServer.FindLocalObject(objId);
        if (localObject)
        {
            //Debug.Log("localObject Found!");
            localObject.GetComponent<NetActions>().RayHit();
        }
        else Debug.LogError("Cant Find localObject by ray hit on server!", this);
    }

    public override void OnStartLocalPlayer()
    {
        //make player opaque-ish
        Color playerColor = GetComponent<MeshRenderer>().material.color;
        playerColor.a = 1.0f;
        GetComponent<MeshRenderer>().material.color = playerColor;
    }

}