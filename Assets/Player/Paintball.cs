using Unity.Netcode;
using UnityEngine;

public class Paintball : NetworkBehaviour
{
    public GameObject paintSplashPrefab; // Assign a splash effect prefab in the Inspector
    public float lifetime = 5f; // Lifetime before the projectile is destroyed
    public float damageAmount = 10f; // Amount of damage for each paintball hit

    private void Start()
    {
        Destroy(gameObject, lifetime); // Destroy the paintball after a certain time
    }

    [ServerRpc(RequireOwnership = false)]
    public void ShootPaintballServerRpc(Vector3 position, Quaternion rotation)
    {
        // Create the paintball at the server side
        GameObject paintball = Instantiate(gameObject, position, rotation);
        paintball.GetComponent<NetworkObject>().Spawn();
    }

    private float damage; // Variable to store the damage value

    public void SetDamage(float value)
    {
        damage = value; // Assign damage when called
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Create the splash effect at the collision point
        if (paintSplashPrefab != null)
        {
            Instantiate(paintSplashPrefab, collision.contacts[0].point, Quaternion.identity);
        }

        // Check if the collision object has a Health component (e.g., a player)
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                // Call the method to apply damage on the server
                playerHealth.TakeDamage(damageAmount);
            }
        }

        // Destroy the paintball after collision
        Destroy(gameObject);
    }
}
