using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Host : NetworkBehaviour
{
    public int[] score;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        //host is 1, next player is 2 etc...
        base.OnNetworkSpawn();
        
        score = new int[32];
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
