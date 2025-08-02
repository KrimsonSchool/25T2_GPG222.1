using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Eye : NetworkBehaviour
{
    public GameObject adds;

    public float addTime;
    private int loops;

    private float timer;

    public NetworkList<int> playerScores =  new NetworkList<int>();
    
    public TextMeshProUGUI scoreText;

    public NetworkVariable<bool> started;
    public GameObject gameStartButton;

    public GameObject[] spawnPoints;
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
        
    }

    // Update is called once per frame
    void Update()
    {
        if (started.Value && gameStartButton.activeSelf)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            gameStartButton.SetActive(false);
        }
        //increment timer
        timer += Time.deltaTime;
        //if timer is greater than add time and is online and is the server
        if (timer >= addTime && started.Value && IsServer)
        {
            for (int i = 0; i < loops + 1; i++)
            {
                //spawn an add
                GameObject add = Instantiate(adds, spawnPoints[Random.Range(0, loops+1)].transform.position,
                    transform.rotation);
                //spawn the add on the server
                add.GetComponent<NetworkObject>().Spawn();
            }

            //increment the add time by a random amount from -1 to 0.5
            addTime += Random.Range(-1f, 0.5f);
            //if add timer is 0 or less
            if (addTime <= 0)
            {
                //set add time to 5
                addTime = 5;
                loops++;
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

    public void StartGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        started.Value = true;
        gameStartButton.SetActive(false);
    }
}
