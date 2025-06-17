using System;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public GameObject cam;

    public float speed;
    public float rotSpeed;

    public int health;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsLocalPlayer)
        {
            print("MY ID: " + this.NetworkObjectId);

            cam.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (IsHost)
        {
            //gameObject.AddComponent<Host>();
        }
        else
        {
            Destroy(gameObject.GetComponent<Host>());
        }
    }

    public void Update()
    {
        // Local only. Not networked
        if (IsLocalPlayer)
        {
            transform.position += transform.forward * Time.deltaTime * speed * Input.GetAxis("Vertical") +
                                  transform.right * Time.deltaTime * speed * Input.GetAxis("Horizontal");

            Rotate_Request_Rpc(Input.GetAxis("Mouse X"));
        }
    }

    //ROTATE
    [Rpc(SendTo.Server, RequireOwnership = false)]
    void Rotate_Request_Rpc(float rot = 0)
    {
        Rotate_ServerResponse_Rpc(rot);
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    void Rotate_ServerResponse_Rpc(float rot = 0)
    {
        //transform.Rotate(0, rot * Time.deltaTime * rotSpeed, 0);
        transform.rotation *= Quaternion.Euler(0, rot * Time.deltaTime * rotSpeed, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsLocalPlayer)
        {
            if (other.CompareTag("Bullet") && other.GetComponent<Bullet>().ownerIndex != this.NetworkObjectId)
            {
                RequestTakeDamage_Rpc(health - 1);
                
                if (health >= 0)
                {
                    print("I HAVE DIED!!!");
                    //has died, needs to up killers score... not own
                    ulong killerIndex = other.gameObject.GetComponent<Bullet>().ownerIndex;
                    //HOST doesnt spawn for client -> error
                    RequestRespawnAndScore_Rpc(5, FindFirstObjectByType<Host>().score[killerIndex] + 1, killerIndex);
                }


                //other.GetComponent<NetworkObject>().Despawn();
                Destroy(other.gameObject);
            }
        }
    }

    [Rpc(SendTo.Server, RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    void RequestTakeDamage_Rpc(int newHealth)
    {
        TakeDamage_Rpc(newHealth);
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    void TakeDamage_Rpc(int newHealth)
    {
        health = newHealth;
    }


    [Rpc(SendTo.Server, RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    void RequestRespawnAndScore_Rpc(int newHealth, int newScore, ulong killerIndex)
    {
        RespawnAndScore_Rpc(newHealth, newScore, killerIndex);
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    void RespawnAndScore_Rpc(int newHealth, int newScore, ulong killerIndex)
    {
        health = newHealth;
        transform.position = Vector3.zero;

        FindFirstObjectByType<Host>().score[killerIndex] = newScore;
    }
}