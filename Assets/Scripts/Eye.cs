using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Eye : NetworkBehaviour
{
    public GameObject adds;

    public float addTime;

    private float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        //host is 1, next player is 2 etc...
        base.OnNetworkSpawn();
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= addTime)
        {
            GameObject add = Instantiate(adds, transform.position, transform.rotation);
            add.GetComponent<NetworkObject>().Spawn();
            timer = 0;
        }
    }
}
