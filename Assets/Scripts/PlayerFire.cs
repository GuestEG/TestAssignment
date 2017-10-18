using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerFire : NetworkBehaviour
{

    //[SerializeField]Bullet bulletPrefab = null;
    float bulletOffset = 3.0f; //meters. how far bullet should be instanced from player's face
    float bulletSpeed = 5.0f; //m per second
    //float bulletLifeTime = 10.0f; //seconds

    //command to execute on server
    [Command]
    public void CmdFireBullet()
    {
        //Debug.Log("Firing!");

        //define instancing point
        //offset to "forward"
        Vector3 instancePos = transform.position + bulletOffset * transform.forward;
        Quaternion instanceRot = transform.rotation;
        Vector3 instanceSpeed = transform.forward * bulletSpeed;

        FindObjectOfType<BulletPool>().BulletCreate(instancePos, instanceRot, instanceSpeed);

        /*        
        if (bulletPrefab != null)
        {
            //create instance locally
            Transform bullet = Instantiate(bulletPrefab, instancePos, instanceRot);

            //set speed
            bullet.GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;

            //paint bullet yellow
            bullet.GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;

            //kill bullet by age (collision event destruction implemented on bullet object)
            Destroy(bullet.gameObject, bulletLifeTime);

            //spawn object representation on clients
            NetworkServer.Spawn(bullet.gameObject);
        }
        else
        {
            Debug.LogError("No prefab associated with the bullet!", this);
        }
        */

    }
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
