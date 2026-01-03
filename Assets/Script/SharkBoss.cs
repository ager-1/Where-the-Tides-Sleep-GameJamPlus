using UnityEngine;

public class SharkHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int damagePerHit = 1;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void OnTriggerEnter(Collider other)
    {
        // DEBUG: Print the name of EVERYTHING that touches the shark
        Debug.Log("Something touched me: " + other.name);

        if (other.CompareTag("Weapon"))
        {
            currentHealth -= damagePerHit;
            Debug.Log("Shark Hit! Health: " + currentHealth);
            Destroy(other.gameObject); // Destroy the bullet

            if (currentHealth <= 0)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.Log("Hit object did NOT have 'Weapon' tag. It was: " + other.tag);
        }
    }
}