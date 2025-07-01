using System;
using DefaultNamespace;
using Unity.Netcode;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public float speed;

    [HideInInspector] public ulong ownerIndex;

    private float timer;

    public float lifeTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //Move_Rpc();
        transform.position += transform.forward * Time.deltaTime * speed;

        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Kill();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Health>()!=null)
        {
            other.GetComponent<Health>().Damage(1, ownerIndex);
            Kill();
        }
    }

    public void Kill()
    {
        if (IsServer)
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }
}