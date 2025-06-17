using UnityEngine;
using Unity.Netcode;

public class Shooter : NetworkBehaviour
{
    public GameObject bullet;
    public GameObject shootFrom;

    public GameObject hand;
    public GameObject handShoot;

    private bool shot;
    private float timer;

    public float shootSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsLocalPlayer)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!shot)
                {
                    Shoot_Request_Rpc();
                }
            }
        }

        if (shot)
        {
            timer+=Time.deltaTime;
            if (timer >= shootSpeed)
            {
                timer = 0;
                shot = false;
                hand.SetActive(true);
                handShoot.SetActive(false);
            }
        }
    }
    
    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void Shoot_Request_Rpc()
    {
        // Check if it's legal/not cheating
        Shoot_Response_Rpc();
    }


// Function that runs from the Server TO ALL clients
    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    private void Shoot_Response_Rpc()
    {
        //bullet.GetComponent<NetworkObject>().Spawn();
        Instantiate(bullet, shootFrom.transform.position, transform.rotation).GetComponent<Bullet>().ownerIndex = this.NetworkObjectId;
        
        hand.SetActive(false);
        handShoot.SetActive(true);
        shot = true;
    }
}
