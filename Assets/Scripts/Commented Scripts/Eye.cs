using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Eye : NetworkBehaviour
{
    public GameObject adds;

    public float addTime;

    private float timer;

    public NetworkList<int> playerScores =  new NetworkList<int>();
    
    public TextMeshProUGUI scoreText;

    private bool online;
    
    //on network spawn
    public override void OnNetworkSpawn()
    {
        //host is 1, next player is 2 etc...
        base.OnNetworkSpawn();

        //if is the server
        if (IsServer)
        {
            //100 times
            for (int i = 0; i < 100; i++)
            {
                //add a player score slot
                playerScores.Add(0);
            }
        }

        //set online to true
        online = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        //increment timer
        timer += Time.deltaTime;
        //if timer is greater than add time and is online and is the server
        if (timer >= addTime && online && IsServer)
        {
            //spawn an add
            GameObject add = Instantiate(adds, transform.position, transform.rotation);
            //spawn the add on the server
            add.GetComponent<NetworkObject>().Spawn();
            
            //increment the add time by a random amount from -1 to 0.5
            addTime += Random.Range(-1f, 0.5f);
            //if add timer is 0 or less
            if (addTime <= 0)
            {
                //set add time to 5
                addTime = 5;
            }
            
            //set the timer to 0
            timer = 0;
        }

        //set the score text to Player Scores
        scoreText.text = "Player Scores:\n";

        //for every player score
        for (int i = 1; i < playerScores.Count; i++)
        {
            //if that player score is greater than 0
            if (playerScores[i] > 0)
            {
                //add that players name and their score to the scoreboard
                scoreText.text += "Player " + i +": " + playerScores[i] + "\n";
            }
        }
    }
}
