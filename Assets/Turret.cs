using Unity.Netcode;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [HideInInspector] public Add[] adds;
    [HideInInspector] public Add closestAdd;

    public GameObject turretTurn;
    public GameObject shootPos;

    public GameObject dirl;

    public float shootSpeed;

    private float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        adds = FindObjectsByType<Add>(FindObjectsSortMode.None);
        float closestDist = 9999;
        foreach (Add add in adds)
        {
            if (Vector3.Distance(gameObject.transform.position, add.transform.position) < closestDist)
            {
                closestDist = Vector3.Distance(gameObject.transform.position, add.transform.position);
                closestAdd = add;
            }
        }

        if (closestAdd != null)
        {
            RotateTurret(closestAdd.gameObject);
            
            timer += Time.deltaTime;
            if (timer >= shootSpeed)
            {
                timer = 0;
                GameObject bltt = Instantiate(dirl, shootPos.transform.position, shootPos.transform.rotation);
                bltt.GetComponent<Bullet>().ownerIndex = 0;
                bltt.GetComponent<NetworkObject>().Spawn();
            }
        }
    }

    public void RotateTurret(GameObject target)
    {
        Vector3 direction = target.transform.position - transform.position;
    
        direction.y = 0;
    
        if (direction != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
        
            transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        }
    }
}
