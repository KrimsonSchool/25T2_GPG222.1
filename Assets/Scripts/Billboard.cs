using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Vector3 rotationOffset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //rotationOffset = new Vector3(90, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindWithTag("Player"))
        {
            Vector3 directionToPlayer = GameObject.FindWithTag("Player").transform.position - transform.position;
            directionToPlayer.y = 0; // Crucially, zero out the Y component

            // Ensure there's a direction to look at
            if (directionToPlayer != Vector3.zero)
            {
                // Create a rotation that looks in that direction
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

                // Apply only the Y-axis rotation, then combine with the offset
                // We apply the offset *after* the Y-axis calculation.
                transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0) *
                                     Quaternion.Euler(rotationOffset);
            }
        }
    }
}
