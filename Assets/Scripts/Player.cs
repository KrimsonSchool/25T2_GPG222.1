using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public GameObject cam;

    public float rotSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsLocalPlayer)
        {
            cam.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
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

            if (Input.GetKey(KeyCode.A))
            {
                MoveX_Request_Rpc(-0.1f);
            }
            if (Input.GetKey(KeyCode.D))
            {
                MoveX_Request_Rpc(0.1f);
            }
            if (Input.GetKey(KeyCode.W))
            {
                MoveX_Request_Rpc(0,0.1f);
            }
            if (Input.GetKey(KeyCode.S))
            {
                MoveX_Request_Rpc(0,-0.1f);
            }
            
            Rotate_Request_Rpc(Input.GetAxis("Mouse X"));
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
    void MoveX_Request_Rpc(float dirX=0, float dirZ=0)
    {
        MoveLeft_ServerResponse_Rpc(dirX, dirZ);
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    void MoveLeft_ServerResponse_Rpc(float dirX=0, float dirZ=0)
    {
        transform.position += transform.right * dirX + transform.forward * dirZ;
    }
    
    //ROTATE
    [Rpc(SendTo.Server, RequireOwnership = false)]
    void Rotate_Request_Rpc(float rot=0)
    {
        Rotate_ServerResponse_Rpc(rot);
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    void Rotate_ServerResponse_Rpc(float rot=0)
    {
        transform.Rotate(0, rot * Time.deltaTime * rotSpeed, 0);
    }

}
