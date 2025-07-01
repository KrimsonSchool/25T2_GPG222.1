using UnityEngine;

public class Spasm : MonoBehaviour
{
    public GameObject[] spitzs;

    private float timer;
    public float timeToSpasm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeToSpasm)
        {
            spitzs[Random.Range(0, spitzs.Length)].SetActive(!spitzs[Random.Range(0, spitzs.Length)].activeSelf);
            timer = 0;
        }
    }
}
