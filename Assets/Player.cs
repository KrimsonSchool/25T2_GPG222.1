using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    
    public void Update()
    {
        // Local only. Not networked
        if (IsLocalPlayer)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                GetBigOrDieTrying_RequestToServer_Rpc();
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                MoveX_Request_Rpc(-1);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                MoveX_Request_Rpc(1);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                MoveX_Request_Rpc(0,1);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                MoveX_Request_Rpc(0,-1);
            }
        }
    }
    
    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void GetBigOrDieTrying_RequestToServer_Rpc()
    {
        // Check if it's legal/not cheating
        GetBigOrDieTrying_ServerToClients_Rpc();
    }


// Function that runs from the Server TO ALL clients
    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    private void GetBigOrDieTrying_ServerToClients_Rpc()
    {
        // This is bugged
        transform.localScale = transform.localScale + new Vector3(1.25f, 1.25f, 1.25f);
    }

    //MOVE

    [Rpc(SendTo.Server, RequireOwnership = false)]
    void MoveX_Request_Rpc(int dirX=0, int dirY=0)
    {
        MoveLeft_ServerResponse_Rpc(dirX, dirY);
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    void MoveLeft_ServerResponse_Rpc(int dirX=0, int dirY=0)
    {
        transform.position += transform.right * dirX + transform.up * dirY;
    }

}
