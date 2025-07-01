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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        //host is 1, next player is 2 etc...
        base.OnNetworkSpawn();

        if (IsServer)
        {
            for (int i = 0; i < 100; i++)
            {
                playerScores.Add(0);
            }
        }

        online = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= addTime && online && IsServer)
        {
            GameObject add = Instantiate(adds, transform.position, transform.rotation);
            add.GetComponent<NetworkObject>().Spawn();
            
            addTime += Random.Range(-1f, 0.5f);
            if (addTime <= 0)
            {
                addTime = 5;
            }
            
            timer = 0;
        }

        scoreText.text = "Player Scores:\n";

        for (int i = 0; i < playerScores.Count; i++)
        {
            if (playerScores[i] > 0)
            {
                scoreText.text += "Player " + i +": " + playerScores[i] + "\n";
            }
        }
    }
}
