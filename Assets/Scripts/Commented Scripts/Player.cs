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

    //on player spawning in the server, triggers once network server is available
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        //is is the local player
        if (IsLocalPlayer)
        {
            //find the hp slider
            hpSlider = FindFirstObjectByType<Slider>(FindObjectsInactive.Include);
            //set it to active
            hpSlider.gameObject.SetActive(true);
            
            //set the tag to Player
            gameObject.tag = "Player";
            //set username
            string nme = "Player " + NetworkObjectId;
            
            //set player camera enabled
            cam.SetActive(true);
            //lock the cursor to the centre
            Cursor.lockState = CursorLockMode.Locked;
            //make the cursor not visible
            Cursor.visible = false;
            
            //set the hp sliders max value to current health
            hpSlider.maxValue = health;
        }
        
        //create array of all players
        Player[] players = FindObjectsOfType<Player>();

        //for each player in the array
        for (int i = 0; i < players.Length; i++)
        {
            //set their title text to their username
            players[i].titleText.text = "Player "+players[i].gameObject.GetComponent<NetworkObject>().NetworkObjectId;
        }
    }

    public void Update()
    {
        //if is the local player
        if (IsLocalPlayer)
        {
            //move the player based on Vertical and Horizontal axis inputs
            transform.position += transform.forward * Time.deltaTime * speed * Input.GetAxis("Vertical") +
                                  transform.right * Time.deltaTime * speed * Input.GetAxis("Horizontal");
            
            //rotate the players X axis by their X mouse movement
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * Time.deltaTime * rotSpeed, 0);
            
            //set the hp slider's value to the players health
            hpSlider.value = health;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    //Take damage function send to server
    [Rpc(SendTo.Server, RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    void RequestTakeDamage_Rpc(int newHealth)
    {
        TakeDamage_Rpc(newHealth);
    }

    //Take damage function on server
    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    void TakeDamage_Rpc(int newHealth)
    {
        //set the players health to the new health
        health = newHealth;
        //if health is less than 1
        if (health <= 0)
        {
            //send Respawn function trigger to server
            RequestRespawn_Rpc(5);
        }
    }

    //send Respawn function trigger to server
    [Rpc(SendTo.Server, RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
    void RequestRespawn_Rpc(int respawnHealth)
    {
        Respawn_Rpc(respawnHealth);
    }

    //Respawn function on server
    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    void Respawn_Rpc(int respawnHealth)
    {
        //set the players position to 0,0,0 (world origin)
        transform.position = Vector3.zero;
        //set the players health to respawn health (default health)
        health = respawnHealth;
        //update the hp slider to reflect health
        hpSlider.value = health;

    }

    //server damage function
    public void Damage(int damage, ulong owner)
    {
        ulong killerIndex = owner;
        //send damage function trigger to server
        RequestTakeDamage_Rpc(health - damage);
    }
}