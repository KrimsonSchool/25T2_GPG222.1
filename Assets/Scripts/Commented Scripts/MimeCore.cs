using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class MimeCore : NetworkBehaviour
{
    //networked variable to stay persistant across all players
    public NetworkVariable<int> pedestalHealth;
    public TextMeshPro healthText;

    public GameObject splo;

    //triggers once the object has spawned on the server
    public override void OnNetworkSpawn()
    {        
        base.OnNetworkSpawn();
        //cal Health changed function
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

    //health changed function
    void HealthChanged(int oldHealth, int newHealth)
    {
        //set the text display to be the current health
        healthText.text = "HP: "+newHealth;

        //if the health is less than 1
        if (pedestalHealth.Value <= 0)
        {
            //activate the explosion effect
            splo.SetActive(true);
            //disable self
            gameObject.SetActive(false);
        }
    }
    
    //on enter trigger
    private void OnTriggerEnter(Collider other)
    {
        //if the other object has the Enemy tag
            if (other.CompareTag("Enemy"))
            {
                //reduce health by 1
                pedestalHealth.Value--;
                //deal 999 damage to the other object
                other.GetComponent<Add>().Damage(999, 0);
            }
        
    }
}
