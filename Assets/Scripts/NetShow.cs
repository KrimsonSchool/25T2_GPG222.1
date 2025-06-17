using Unity.Netcode;
using UnityEngine;

public class NetShow : NetworkBehaviour
{
    public GameObject[] objectsKillNotLocal;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsLocalPlayer)
        {
            foreach (GameObject go in objectsKillNotLocal)
            {
                go.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
