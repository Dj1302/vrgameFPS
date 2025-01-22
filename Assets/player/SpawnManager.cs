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
        // Automatically spawn the player when they connect (only for server)
        if (IsServer)
        {
            SpawnPlayer();
        }
    }

    public void SpawnPlayer()
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

                // Spawn the player on the network
                playerNetworkObject.Spawn();
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

    // Use OnDestroy for network cleanup when the object is destroyed
    private void OnDestroy()
    {
        base.OnDestroy();
    }
}
