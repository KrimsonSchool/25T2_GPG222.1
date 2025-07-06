using DefaultNamespace;
using Unity.Netcode;
using UnityEngine;

public class Add : NetworkBehaviour, Health
{
    private Player[] players;

    public float speed;
    public Transform goal;

    private float timer;
    public float attackTime;


    public int health;

    public GameObject attackSurface;
    public GameObject attackPos;
    // On spawn in server
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        //find all players
        players = FindObjectsByType<Player>(FindObjectsSortMode.InstanceID);
        //find the mime core
        Transform mimeCore = GameObject.FindWithTag("Finish").GetComponent<Transform>();
        //set the default goal to be the mime core
        goal = mimeCore;
        //50% chance
        if (Random.Range(0, 2) == 1)
        {
            //set goal to be randomly one of the players
            goal = players[Random.Range(0, players.Length)].transform;
        }
    }

    void Update()
    {
        //if goal isnt null
        if (goal != null)
        {
            //move towards the gaol based on speed
            transform.position = Vector3.MoveTowards(transform.position, goal.position, speed * Time.deltaTime);
        }

        //idk...
        transform.position = new Vector3(transform.position.x, 1.286f, transform.position.z);
        
        //increment timer
        timer += Time.deltaTime;
        //if timer is greater than attack time
        if (timer >= attackTime)
        {
            //trigger attack function
            Attack();
            //set the timer to 0
            timer = 0;
        }
    }
    
    //Damage function
    public void Damage(int damage, ulong owner)
    {
        //if is the Server and the attacker isn't ID 999
        if (IsServer && owner != 999)
        {
            //reduce health by damage taken
            health-=damage;

            //if health is less than 1
            if (health <= 0)
            {
                ulong ownerIndex = owner;
                int index = (int)ownerIndex;
                
                //increment the score of the killing player
                FindFirstObjectByType<Eye>().playerScores[index] ++;
                //despawn the object
                GetComponent<NetworkObject>().Despawn();
                //destroy the object
                Destroy(gameObject);
            }
        }
    }

    //attack function
    public void Attack()
    {
        //spawn an attack surface
        GameObject ase = Instantiate(attackSurface, attackPos.transform.position, Quaternion.identity);
        //spawn the attack surface on the network
        ase.GetComponent<NetworkObject>().Spawn();
    }
}
