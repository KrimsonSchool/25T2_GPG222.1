using UnityEngine;
using Unity.Netcode;

public class Shooter : NetworkBehaviour
{
    [Header("Main Wep")]
    public GameObject bulletPrefab;
    public GameObject shootFrom;

    public GameObject hand;
    public GameObject handShoot;

    private bool shot;
    private float timer;

    public float shootSpeed;
    
    [Header("skul Wep")]
    public GameObject bullet2Prefab;
    public GameObject shoot2From;
    
    public float shoot2Speed;

    public NetworkVariable<bool> skulWep;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if is the local player
        if (IsLocalPlayer)
        {
            //if left mouse button is pressed
            if (Input.GetMouseButtonDown(0))
            {
                //if hasn't recently shot
                if (!shot)
                {
                    //trigger shoot request to server
                    Shoot_Request_Rpc();
                }
            }
        }

        //if has shot recently
        if (shot)
        {
            //increment timer
            timer+=Time.deltaTime;
            //if timer is greater than shot speed
            if (skulWep.Value)
            {
                if (timer >= shoot2Speed)
                {
                    timer = 0;
                    shot = false;
                }
            }
            else
            {
                if (timer >= shootSpeed)
                {
                    //set the timer to 0
                    timer = 0;
                    //set has shot recently to false
                    shot = false;
                    //activate normal hand
                    hand.SetActive(true);
                    //deactivate shooting hand
                    handShoot.SetActive(false);
                }
            }
            
        }
    }
    
    //send shoot request to server
    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void Shoot_Request_Rpc()
    {
        //trigger server shoot function
        Shoot_Response_Rpc();
    }


    //server shoot function
    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    private void Shoot_Response_Rpc()
    {
        if (skulWep.Value)
        {
            if (IsServer)
            {
                GameObject blet = Instantiate(bullet2Prefab, shoot2From.transform.position, transform.rotation);

                blet.GetComponent<Bullet>().ownerIndex = this.NetworkObjectId;
                blet.GetComponent<NetworkObject>().Spawn();
            }
            transform.Find("fpsarms").GetComponent<Animator>().SetBool("Shoot", true);
            shot = true;
        }
        else
        {
            //if is the server
            if (IsServer)
            {
                //spawn a bullet
                GameObject blet = Instantiate(bulletPrefab, shootFrom.transform.position, transform.rotation);
                //set the bullets owner to this player
                blet.GetComponent<Bullet>().ownerIndex = this.NetworkObjectId;
                //spawn the bullet on the server
                blet.GetComponent<NetworkObject>().Spawn();
            }

            //set the normal hand inactive
            hand.SetActive(false);
            //show the shooting hand
            handShoot.SetActive(true);
            //set has shot recently to true
            shot = true;
        }
    }
}
