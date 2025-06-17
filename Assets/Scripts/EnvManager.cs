using UnityEngine;

public class EnvManager : MonoBehaviour
{
    public Material skyMat;
    public float speedX;
    public float speedY;

    public Material groundMat;
    public float speedZ;
    public float timeMax;
    private float timer;

    private bool shrink;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        skyMat.mainTextureOffset += new Vector2(Time.deltaTime * speedX,Time.deltaTime * speedY);

        if (!shrink)
        {
            groundMat.mainTextureOffset += new Vector2(Time.deltaTime * speedZ, Time.deltaTime * speedZ);
        }else
        {
            groundMat.mainTextureOffset += new Vector2(Time.deltaTime * -speedZ, Time.deltaTime * -speedZ);
        }
        
        timer += Time.deltaTime;
        if (timer >= timeMax)
        {
            timer = 0;
            shrink = !shrink;
        }
    }
}
