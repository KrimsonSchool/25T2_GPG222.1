using System;
using System.ComponentModel;
using Unity.Netcode;
using UnityEngine;

public class scrab : MonoBehaviour
{
    public GameObject otherObReg;
    
    //what do:

    [Header("What it do: [select one]")] 
    public bool teleport;
    public bool changeInt;
    public bool toggleBool;
    public bool triggerAnim;
    public bool changeMaterial;
    public bool toggleGameobject;
    public bool triggerAnimator;
    public bool toggleObj;
    
    [Header("Settings")] 
    
    
    public Vector3 teleportLocation;
    public int intChangeBy;
    public bool setBoolSpecific;[Tooltip("Set the bool to a specific state (Set below).")]
    public bool newBoolState;
    public Animation animToPlay;
    public MeshRenderer meshRendererOfMatChange;
    public int indexOfMat;
    public Material newMat;
    public float delay;
    public string nameOfObjectToEnable;
    public string animBoolToTrigger;
    public GameObject objToToggle;
    //teleport
    //incriment int
    //decriment int
    //set bool t/f
    private bool dely;
    private float timer;
    //need network variable int and bool
    void Start()
    {
        //not going to be modular, sue me
    }

    // Update is called once per frame
    void Update()
    {
            if (otherObReg != null)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    print("SECRET!!!");
                    TriggerSes();
                }
            }

            if (dely)
            {
                timer+=Time.deltaTime;
                if (timer > delay)
                {
                    ToggleGameobject();
                    timer = 0;
                    dely = false;
                }
            }
    }

    public void TriggerSes()
    {
        
        
        if (teleport)
        {
            otherObReg.transform.position = teleportLocation;
        }

        if (triggerAnim)
        {
            animToPlay.Play();
        }

        if (changeMaterial)
        {
            Material[] meshRenMats = meshRendererOfMatChange.materials;
            meshRenMats[indexOfMat] = newMat;
            meshRendererOfMatChange.materials = meshRenMats;
        }

        if (toggleGameobject)
        {
            ToggleGameobject();
        }

        if (triggerAnimator)
        {
            otherObReg.transform.Find("fpsarms").GetComponent<Animator>().SetBool(animBoolToTrigger, true);
        }

        if (toggleObj)
        {
            objToToggle.SetActive(!objToToggle.activeSelf);
        }
    }

    public void ToggleGameobject()
    {
        if (delay > 0 && !dely)
        {
            dely=true;
            return;
        }

        ToggleObject(nameOfObjectToEnable);
    }

    public void ToggleObject(string objectName)
    {
        otherObReg.transform.Find(objectName).gameObject.SetActive(!otherObReg.transform.Find(objectName).gameObject.activeSelf);
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            otherObReg =  other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            otherObReg =  null;
        }
    }
}
