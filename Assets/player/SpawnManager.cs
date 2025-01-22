using Unity.Netcode;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
    [Header("Spawn Points")]
    public Transform[] redTeamSpawns;  // Spawn points for red team
    public Transform[] blueTeamSpawns; // Spawn points for blue team

    [Header("Player Prefab")]
    public GameObject playerPrefab;

    public static SpawnManager Instance;

    private int redTeamCount = 0; // Track number of players in red team
    private int blueTeamCount = 0; // Track number of players in blue team

    private void Awake()
    {
        // Ensure there's only one SpawnManager
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Ensure we spawn players right away when the scene is loaded
        if (IsServer)
        {
            // This will spawn the first player for the server at the start of the scene
            SpawnPlayerForClient(NetworkManager.Singleton.LocalClientId);
        }

        // Register to the client connected callback event for future connections
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
    }

    private void OnDestroy()
    {
        // Unregister the callback to prevent memory leaks
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        // Only the server handles player spawning
        if (IsServer)
        {
            SpawnPlayerForClient(clientId);
        }
    }

    public void SpawnPlayerForClient(ulong clientId)
    {
        if (IsServer)
        {
            // Dynamically assign team
            string team = AssignTeam();

            // Get the spawn point based on the assigned team
            Transform spawnPoint = GetSpawnPoint(team);
            if (spawnPoint != null)
            {
                GameObject playerInstance = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
                NetworkObject playerNetworkObject = playerInstance.GetComponent<NetworkObject>();

                // Assign the team and spawn the player
                PlayerManager playerManager = playerInstance.GetComponent<PlayerManager>();
                playerManager.SetTeam(team);

                // Spawn the player object on the network and assign it to the client
                playerNetworkObject.SpawnAsPlayerObject(clientId);
            }
        }
    }

    // Dynamically assign team (this can be extended for more complex logic)
    private string AssignTeam()
    {
        // Simple logic to balance teams: Assign to the team with fewer players
        if (redTeamCount <= blueTeamCount)
        {
            redTeamCount++;
            return "red";
        }
        else
        {
            blueTeamCount++;
            return "blue";
        }
    }

    public Transform GetSpawnPoint(string team)
    {
        // Return a random spawn point from the appropriate team
        if (team == "red")
        {
            return redTeamSpawns[Random.Range(0, redTeamSpawns.Length)];
        }
        else if (team == "blue")
        {
            return blueTeamSpawns[Random.Range(0, blueTeamSpawns.Length)];
        }
        return null;
    }
}
