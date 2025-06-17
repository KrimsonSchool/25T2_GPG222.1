using Unity.Netcode;
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
    
    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    private void Move_Rpc()
    {
        transform.position += transform.forward * Time.deltaTime * speed;
    }
}
