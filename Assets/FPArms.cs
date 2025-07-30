using UnityEngine;

public class FPArms : MonoBehaviour
{
    public GameObject[] killMePlsObjs;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (GameObject obj in killMePlsObjs)
        {
            obj.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShootDone()
    {
        GetComponent<Animator>().SetBool("Shoot", false);
    }
}
