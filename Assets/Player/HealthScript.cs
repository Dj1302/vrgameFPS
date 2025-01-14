using UnityEngine;
using Unity.Netcode;

public class Health : NetworkBehaviour
{
    public float maxHealth = 100f;                  // Max health value
    private NetworkVariable<float> currentHealth;  // Network variable to store current health

    // Optional respawn settings
    public float respawnTime = 5f;
    private Vector3 spawnPoint;

    private void Awake()
    {
        // Initialize current health as max health when the script starts
        currentHealth = new NetworkVariable<float>(maxHealth, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    }

    private void Start()
    {
        // Store spawn point if you plan on respawning players at the same location
        if (IsServer)
        {
            spawnPoint = transform.position;
        }
    }

    // Method for taking damage from other players or projectiles
    public void TakeDamage(float damage)
    {
        if (!IsServer) return;  // Only process damage on the server

        currentHealth.Value -= damage;

        if (currentHealth.Value <= 0)
        {
            currentHealth.Value = 0;
            Die();
        }
    }

    // Method to handle player death (e.g., disable player, respawn, etc.)
    private void Die()
    {
        Debug.Log($"{gameObject.name} has died!");

        // Disable player or trigger death-related animations, effects, etc.
        // For simplicity, let's disable the player here:
        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;

        // Respawn after a delay (using coroutine)
        RespawnPlayer();
    }

    // Respawn logic: Wait for 'respawnTime' and reset health, position, etc.
    private void RespawnPlayer()
    {
        // Wait for respawnTime before respawning the player
        Invoke(nameof(Respawn), respawnTime);
    }

    // Resets player health and re-enables them after respawning
    private void Respawn()
    {
        // Reset health
        currentHealth.Value = maxHealth;

        // Re-enable player and place them at their spawn point
        transform.position = spawnPoint;
        GetComponent<Collider>().enabled = true;
        GetComponent<Renderer>().enabled = true;

        Debug.Log($"{gameObject.name} has respawned!");
    }

    // Optional: Show the current health for debugging purposes
    private void OnGUI()
    {
        if (IsLocalPlayer)  // Display health only for the local player
        {
            GUI.Label(new Rect(10, 10, 200, 20), $"Health: {currentHealth.Value}");
        }
    }
}
