using System;
using DefaultNamespace;
using Unity.Netcode;
using UnityEngine;

public class EASTriadNeoTriacrhSeventeen : NetworkBehaviour
{
    private ulong ownerIndex = 999;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Health>()!=null)
        {
            other.GetComponent<Health>().Damage(1, ownerIndex);
            if (other.GetComponent<Add>()==null)
            {
                Kill();
            }
        }
    }

    public void Kill()
    {
        if (IsServer)
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }
}
