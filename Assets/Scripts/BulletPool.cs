using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


/// <summary>
/// Pool to maintain bullets
/// </summary>
public class BulletPool : NetworkBehaviour {

    [SerializeField]
    Bullet bulletPrefab = null;

    [SerializeField]
    int maxPoolCapacity = 3; //pool capacity grow cap

    //server-side only
    [SerializeField]//expose for test
    List<Bullet> bulletList;

    //Singleton implementation
    /*
    private static BulletPool instance;
    
    public static BulletPool Instance
    {
        get
        {
            //lazy init
            //multithread unsafe!
            if (instance == null) instance = new BulletPool();
            return instance;
        }
    }
    */

    private void Awake()
    {
        //create pool
        bulletList = new List<Bullet>(maxPoolCapacity);
    }

    
    //public interface to create a bullet somethere
    public void BulletCreate(Vector3 position, Quaternion rotation, Vector3 speed)
    {
        //get object from pool
        Bullet bullet = GetBulletFromPool();
        //init on server
        BulletInit(position, rotation, speed, bullet);
        //init on client too
        if (isServer) RpcBulletInit(position, rotation, speed, bullet.netId);
    }

    private Bullet GetBulletFromPool()
    {
        
        Bullet bullet;

        //Debug.Log("Current pool capacity = " + bulletList.Capacity + ", pool object count = " + bulletList.Count);

        //try to find something unused
        bullet = bulletList.Find(bFind => !bFind.inUse);
        if (bullet)
        //if (BulletList.Exists(bullet => !bullet.inUse))
        {
            //Debug.Log("Reusing unused asset");            
        }
        else if (bulletList.Count < bulletList.Capacity)
        //is there any capacity left
        {
            //need new one
            if (bulletPrefab != null)
            {
                //Debug.Log("Creating new bullet object");
                //create instance locally
                bullet = Instantiate(bulletPrefab);
                //add bullet to the server pool
                bulletList.Add(bullet);
                //spawn it on clients
                NetworkServer.Spawn(bullet.gameObject);                

            }
            else
            {
                //Debug.LogError ("No prefab associated with the bullet!", this);
                throw new System.Exception("No prefab associated with a bullet! I don't know how bullet should look like!");                
            }

        }
        else
        {
            //Debug.Log("Pool capacity exceeded");
            //no unused assets? take the oldest one!
            //Debug.Log("No unused assets found, reusing oldest");
            bulletList.Sort(Bullet.CompareByLifetime);
            bullet = bulletList[bulletList.Count - 1];
            //need to destroy it first
            BulletDestroy(bullet);
        }

        //Debug.Log("New pool capacity = " + bulletList.Capacity + ", pool object count = " + bulletList.Count);
        return bullet;
    }

    private void BulletInit(Vector3 position, Quaternion rotation, Vector3 speed, Bullet bullet)
    {
        bullet.Init(position, rotation, speed);
        bullet.inUse = true;
        bullet.lifeTime = 0.0f;

        //activate
        bullet.gameObject.SetActive(true);

    }

    //public interface to destroy the bullet
    public void BulletDestroy(Bullet objBullet)
    {
        int index = bulletList.IndexOf(objBullet);
        Bullet bullet = bulletList[index];
        bullet.inUse = false;
        bullet.gameObject.SetActive(false);

        //Debug.Log("Deactivationg server object"+bullet.name);

        if(isServer) RpcBulletDestroy(bullet.netId);
        //deactivate on clients
        //RpcBulletUpdate(bullet.netId, false);
        //NetworkServer.UnSpawn(bullet.gameObject);
    }

    //client wraps
    [ClientRpc]
    private void RpcBulletInit(Vector3 position, Quaternion rotation, Vector3 speed, NetworkInstanceId networkId)
    {
        //local GameObject
        GameObject bulletObj = ClientScene.FindLocalObject(networkId);
        //call function on local object
        if (bulletObj) BulletInit(position, rotation, speed, bulletObj.GetComponent<Bullet>());
    }

    [ClientRpc]
    private void RpcBulletDestroy(NetworkInstanceId networkId)
    {
        //local GameObject
        GameObject bulletObj = ClientScene.FindLocalObject(networkId);
        //just deactivate
        if (bulletObj) bulletObj.SetActive(false);
    }

    //update client's bullet from client's pool
    [ClientRpc]
    private void RpcClientBulletUpdate(NetworkInstanceId networkId, bool enable)
    {
        //local GameObject
        GameObject bulletObj = ClientScene.FindLocalObject(networkId);
        
        //Debug.Log("Update client object" + bulletObj.name +".SetActive("+ enable+ ")");
        bulletObj.SetActive(enable);
     
    }

}
