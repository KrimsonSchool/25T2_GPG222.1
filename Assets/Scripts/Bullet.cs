using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public float speed;

    [HideInInspector]
    public ulong ownerIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        
    }

    // Update is called once per frame
    void Update()
    {
        //Move_Rpc();
        transform.position += transform.forward * Time.deltaTime * speed;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
}
