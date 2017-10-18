using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {

    public bool inUse = false;

    public float lifeTime = 0.0f; //how old the object is

    public static int CompareByLifetime(Bullet bul1, Bullet bul2)
    {
        return bul1.lifeTime.CompareTo(bul2.lifeTime);
    }


    public void Init(Vector3 position, Quaternion rotation, Vector3 speed)
    {
        //re-initialise object with the new params
        transform.position = position;
        transform.rotation = rotation;
        GetComponent<Rigidbody>().velocity = speed;
        //dont forget to kill rotation impulse
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update () {

        lifeTime += Time.deltaTime; //ageing

	}

    private void OnCollisionEnter(Collision collision)
    {
        //collided at something - ask to self-destroy
        //local bullet pool?
        if (!isServer) return;
        FindObjectOfType<BulletPool>().BulletDestroy(this);
    }



    public override void OnStartClient()
    {
        //we're spawned on client and should be added to pool
    }
}
