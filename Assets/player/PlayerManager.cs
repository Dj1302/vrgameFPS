using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public bool isSpawnProtected = true;
    public float spawnProtectionTime = 5f;
    public string team;
    private bool isInvincible = false;

    // For synchronization with the network
    private NetworkVariable<int> networkHealth = new NetworkVariable<int>(100);
    private NetworkVariable<bool> networkIsInvincible = new NetworkVariable<bool>(true);

    private void Start()
    {
        if (IsOwner)
        {
            currentHealth = maxHealth;
            StartCoroutine(SpawnProtection());
        }
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        if (IsServer && !isInvincible && networkHealth.Value > 0)
        {
            networkHealth.Value -= damage;
            Debug.Log("Player Health: " + networkHealth.Value);
        }
    }

    private void Die()
    {
        if (IsServer)
        {
            Debug.Log("Player has died.");
            // Handle respawn logic (e.g., delay before respawn)
            StartCoroutine(Respawn());
        }
    }

    private IEnumerator Respawn()
    {
        // Disable controls during respawn
        yield return new WaitForSeconds(3f);  // Wait for 3 seconds before respawn

        // Respawn the player at a spawn point (you can update this based on team or other logic)
        Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint(team);
        transform.position = spawnPoint.position;

        // Reset health and spawn protection
        networkHealth.Value = maxHealth;
        networkIsInvincible.Value = true;

        // Wait for spawn protection time to end
        yield return new WaitForSeconds(spawnProtectionTime);
        networkIsInvincible.Value = false;
    }

    private IEnumerator SpawnProtection()
    {
        networkIsInvincible.Value = true;
        yield return new WaitForSeconds(spawnProtectionTime);
        networkIsInvincible.Value = false;
    }

    public void SetTeam(string playerTeam)
    {
        team = playerTeam;
    }

    // Called to synchronize health across the network
    public void OnHealthChanged(int newHealth)
    {
        networkHealth.Value = newHealth;
    }
}
