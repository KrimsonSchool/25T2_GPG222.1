using System;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication.PlayerAccounts;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public GameObject cam;

    public float speed;
    public float rotSpeed;

    public int health;


    public TextMeshPro titleText;
    
    //list of in network object id's of bullets, when it wants kill, cross from list for all
    //public NetworkVariable<string> names;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsLocalPlayer)
        {
            gameObject.tag = "Player";
            string nme = "Player " + NetworkObjectId;
            print(nme);
/*
            if (!names.Value.Contains(nme))
            {
                
            }
*/
            //names.Value[NetworkObjectId]=nme;
            
            cam.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            //RequestSetName_Rpc("Player" + NetworkObjectId);
        }
        
        Player[] players = FindObjectsOfType<Player>();

        for (int i = 0; i < players.Length; i++)
        {
            players[i].titleText.text = "Player "+players[i].gameObject.GetComponent<NetworkObject>().NetworkObjectId;
        }
        
        //titleText.text = name.Value;
        //gameObject.name = name.Value;

    }

    private void OnEnable()
    {
        //names.OnValueChanged += NameListChanged;
    }

    private void OnDisable()
    {
        //names.OnValueChanged -= NameListChanged;
    }


    public void Update()
    {
        // Local only. Not networked
        if (IsLocalPlayer)
        {
            transform.position += transform.forward * Time.deltaTime * speed * Input.GetAxis("Vertical") +
                                  transform.right * Time.deltaTime * speed * Input.GetAxis("Horizontal");
            
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * Time.deltaTime * rotSpeed, 0);

            //Rotate_Request_Rpc(Input.GetAxis("Mouse X"));
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
        if (IsServer)
        {
            if (other.CompareTag("Bullet") && other.GetComponent<Bullet>().ownerIndex != this.NetworkObjectId)
            {
                RequestTakeDamage_Rpc(health - 1);
                
                if (health <= 0)
                {
                    print("I HAVE DIED!!!");
                    //has died, needs to up killers score... not own
                    ulong killerIndex = other.gameObject.GetComponent<Bullet>().ownerIndex;
                    //HOST doesnt spawn for client -> error
                    
                    //RequestRespawnAndScore_Rpc(5, FindFirstObjectByType<Eye>().score[killerIndex] + 1, killerIndex);
                }
                //other.GetComponent<NetworkObject>().Despawn();
                //Destroy(other.gameObject);
            }
            
            if (other.CompareTag("EnemyAttack"))
            {
                RequestTakeDamage_Rpc(health - 1);
                
                if (health <= 0)
                {
                    print("I HAVE DIED!!!");
                    RequestRespawn_Rpc(5);
                }
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
    void RequestRespawn_Rpc(int newHealth)
    {
        Respawn_Rpc(newHealth);
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    void Respawn_Rpc(int newHealth)
    {
        health = newHealth;
        transform.position = Vector3.zero;

        //FindFirstObjectByType<Eye>().score[killerIndex] = newScore; //add global variable
    }
    
    
    
    [Rpc(SendTo.Server, RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    void RequestSetName_Rpc(string objName)
    {
        SetName_Rpc(objName);
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    void SetName_Rpc(string objName)
    {
        titleText.text = objName;
        gameObject.name = objName;
    }


    private void NameListChanged(string prev, string next)
    {
        //
    }
}