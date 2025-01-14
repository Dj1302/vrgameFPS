using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab; // The player prefab to spawn
    public Transform spawnPoint;    // Where to spawn the player

    void Start()
    {
        // Only spawn a player for the local player (when the game starts)
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
        {
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        // Spawn the player at the spawn point
        GameObject player = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);

        // Assign ownership of the player object to the local player
        player.GetComponent<NetworkObject>().Spawn();

        // Optionally set player as local player for input handling
        if (player.TryGetComponent(out NetworkPlayerController controller))
        {
            controller.IsLocalPlayer = true;
        }
    }
}

