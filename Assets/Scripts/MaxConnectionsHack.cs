using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MaxConnectionsHack : MonoBehaviour {

    //extremely dirty hack - avoid at all costs
    //should be executed both on client and server to prevent CRC config error
    private void Awake()
    {
        GetComponent<NetworkManager>().maxConnections = int.MaxValue;
    }

    
}
