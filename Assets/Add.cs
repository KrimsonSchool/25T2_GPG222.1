using Unity.Netcode;
using UnityEngine;

public class Add : NetworkBehaviour
{
    private Player[] players;

    public float speed;
    public Transform mimeCore;

    private float timer;
    public float attackTime;

    private float timer2;
    public float selectorTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        players = FindObjectsByType<Player>(FindObjectsSortMode.InstanceID);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, mimeCore.position, speed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, 1.286f, transform.position.z);
        
        
        
        timer += Time.deltaTime;
        if (timer >= attackTime)
        {
            //attack
            timer = 0;
        }
        
        timer2 += Time.deltaTime;
        if (timer2 >= selectorTimer)
        {
            //select one of the players or the Mime Core to go towards;
            timer2 = 0;
        }
    }
}
