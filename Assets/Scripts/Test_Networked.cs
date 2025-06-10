using System;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class Test_Networked : NetworkBehaviour
{
    private bool big;
    private Color color;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        //start
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FindFirstObjectByType<NetworkManager>().StartHost();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            FindFirstObjectByType<NetworkManager>().StartClient();
        }
        
        if (IsServer)
        {
            if (Random.value > 0.95f)
            {
                big = !big;
                TestFunction_Rpc(big);
            }

            if (Random.value > 0.5f)
            {                
                color = new Color(Random.value, Random.value, Random.value);
                ChangeColour_Rpc(color);
            }
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void TestFunction_Rpc(bool _big)
    {
        transform.localScale = _big ? new Vector3(2, 2, 2) : new Vector3(1, 1, 1);
    }
   
    [Rpc(SendTo.ClientsAndHost)]
    public void ChangeColour_Rpc(Color _colour)
    {
        GetComponent<MeshRenderer>().material.color = _colour;
    }
    
}
