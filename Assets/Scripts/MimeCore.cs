using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class MimeCore : NetworkBehaviour
{
    public NetworkVariable<int> pedestalHealth;
    public TextMeshPro healthText;

    public GameObject splo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public override void OnNetworkSpawn()
    {
        HealthChanged(pedestalHealth.Value, pedestalHealth.Value);
    }

    private void OnEnable()
    {
        pedestalHealth.OnValueChanged += HealthChanged;
    }

    private void OnDisable()
    {
        pedestalHealth.OnValueChanged -= HealthChanged;
    }

    void HealthChanged(int oldHealth, int newHealth)
    {
        healthText.text = "HP: "+newHealth;

        if (pedestalHealth.Value <= 0)
        {
            if (IsServer)
            {
                splo.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
            if (other.CompareTag("EnemyAttack"))
            {
                //pedestalHealth.Value--;
            }
            if (other.CompareTag("Enemy"))
            {
                pedestalHealth.Value--;
                other.GetComponent<Add>().Damage(999, 0);
                
                print("Killing clown...");
            }
        
    }
}
