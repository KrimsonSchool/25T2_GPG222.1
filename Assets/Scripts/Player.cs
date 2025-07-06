using System;
using System.Collections.Generic;
using DefaultNamespace;
using NUnit.Framework;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication.PlayerAccounts;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour, Health
{
    public GameObject cam;

    public float speed;
    public float rotSpeed;

    public int health;


    public TextMeshPro titleText;

    public Slider hpSlider;
    
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
            hpSlider = FindFirstObjectByType<Slider>(FindObjectsInactive.Include);
            hpSlider.gameObject.SetActive(true);
            
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
            
            hpSlider.maxValue = health;
        }
        
        Player[] players = FindObjectsOfType<Player>();

        for (int i = 0; i < players.Length; i++)
        {
            players[i].titleText.text = "Player "+players[i].gameObject.GetComponent<NetworkObject>().NetworkObjectId;
        }
    }

    public void Update()
    {
        if (IsLocalPlayer)
        {
            transform.position += transform.forward * Time.deltaTime * speed * Input.GetAxis("Vertical") +
                                  transform.right * Time.deltaTime * speed * Input.GetAxis("Horizontal");
            
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * Time.deltaTime * rotSpeed, 0);
            
            hpSlider.value = health;
        }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    void Rotate_Request_Rpc(float rot = 0)
    {
        Rotate_ServerResponse_Rpc(rot);
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    void Rotate_ServerResponse_Rpc(float rot = 0)
    {
        transform.rotation *= Quaternion.Euler(0, rot * Time.deltaTime * rotSpeed, 0);
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
        if (health <= 0)
        {
            RequestRespawn_Rpc(5);
        }
    }


    [Rpc(SendTo.Server, RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    void RequestRespawn_Rpc(int respawnHealth)
    {
        Respawn_Rpc(respawnHealth);
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    void Respawn_Rpc(int respawnHealth)
    {
        transform.position = Vector3.zero;
        health = respawnHealth;
        hpSlider.value = health;

    }

    public void Damage(int damage, ulong owner)
    {
        ulong killerIndex = owner;
        RequestTakeDamage_Rpc(health - damage);
    }
}