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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        players = FindObjectsByType<Player>(FindObjectsSortMode.InstanceID);
        Transform mimeCore = GameObject.FindWithTag("Finish").GetComponent<Transform>();
        goal = mimeCore;
        if (Random.Range(0, 2) == 1)
        {
            //go after players
            goal = players[Random.Range(0, players.Length)].transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (goal != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, goal.position, speed * Time.deltaTime);
        }

        transform.position = new Vector3(transform.position.x, 1.286f, transform.position.z);
        
        
        timer += Time.deltaTime;
        if (timer >= attackTime)
        {
            //attack
            Attack();
            timer = 0;
        }
    }

    //TODO attack players and Mime Core
    
    public void Damage(int damage, ulong owner)
    {
        if (IsServer && owner != 999)
        {
            health-=damage;

            if (health <= 0)
            {
                ulong ownerIndex = owner;
                int index = (int)ownerIndex;
                    
                FindFirstObjectByType<Eye>().playerScores[index] ++;
                GetComponent<NetworkObject>().Despawn();
                Destroy(gameObject);
            }
        }
    }

    public void Attack()
    {
        GameObject ase = Instantiate(attackSurface, attackPos.transform.position, Quaternion.identity);
        ase.GetComponent<NetworkObject>().Spawn();
    }
}
