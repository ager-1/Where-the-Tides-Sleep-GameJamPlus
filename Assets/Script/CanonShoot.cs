using UnityEngine;

public class CanonShoot : MonoBehaviour
{
    [Header("Assignments")]
    public GameObject canonBall;
    public Transform barell;

    [Header("Settings")]
    public float force;
    public float fireDelay = 0.5f; // Time in seconds between shots

    private float nextFireTime = 0f; // Stores the time when we are allowed to shoot again

    void Update()
    {
        // Check if Mouse is pressed AND current time is greater than the next allowed fire time
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            Shoot();

            // Reset the timer: Current Time + Delay Amount
            nextFireTime = Time.time + fireDelay;
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(canonBall, barell.position, barell.rotation);

        // Applying the velocity
        // Note: "linearVelocity" is for Unity 6. If using older Unity (2022 or older), change this to ".velocity"
        bullet.GetComponent<Rigidbody>().linearVelocity = barell.forward * force * Time.deltaTime;
    }
}