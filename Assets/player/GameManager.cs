using Unity.Netcode;
using UnityEngine;
using TMPro;  // Import the TextMesh Pro namespace

public class GameManager : NetworkBehaviour
{
    [Header("UI Elements")]
    public TMP_Text redTeamScoreText; // UI Text for red team score using TextMesh Pro
    public TMP_Text blueTeamScoreText; // UI Text for blue team score using TextMesh Pro
    public TMP_Text endGameText; // UI Text for end game message using TextMesh Pro
    public TMP_InputField killLimitInputField; // Input field for host to adjust kill limit using TextMesh Pro

    private int redTeamKills = 0;
    private int blueTeamKills = 0;

    // This is the kill limit, which can be adjusted by the host
    private int maxKills = 25;

    private const float restartDelay = 5f; // Delay before restarting the game

    // Update UI to show scores
    private void UpdateScoreUI()
    {
        redTeamScoreText.text = "Red Team: " + redTeamKills;
        blueTeamScoreText.text = "Blue Team: " + blueTeamKills;
    }

    // This method should be called when a player from a team scores a kill
    public void OnKill(string team)
    {
        if (team == "red")
        {
            redTeamKills++;
        }
        else if (team == "blue")
        {
            blueTeamKills++;
        }

        // Update the UI
        if (IsServer)
        {
            UpdateScoreUI();
            CheckForGameEnd();
        }
    }

    // Check if the game should end (when a team reaches the target score)
    private void CheckForGameEnd()
    {
        if (redTeamKills >= maxKills)
        {
            EndGame("Red Team Wins!");
        }
        else if (blueTeamKills >= maxKills)
        {
            EndGame("Blue Team Wins!");
        }
    }

    // End the game and display the result
    private void EndGame(string winnerMessage)
    {
        // Display the end game message and the score
        endGameText.text = winnerMessage + "\nFinal Score:\nRed Team: " + redTeamKills + "\nBlue Team: " + blueTeamKills;

        DisableGameplay();

        // Wait for the restart and team rebalance
        Invoke(nameof(RestartGame), restartDelay);
    }

    // Disable gameplay (e.g., disable controls, shooting, etc.)
    private void DisableGameplay()
    {
        // Implement the logic to disable gameplay here (e.g., stop player movement)
    }

    // Restart the game (reset score and start a new match)
    public void RestartGame()
    {
        if (IsServer)
        {
            redTeamKills = 0;
            blueTeamKills = 0;
            UpdateScoreUI();
            endGameText.text = "";

            // Rebalance teams and restart the game
            RebalanceTeams();
        }
    }

    // Rebalance the teams after the game ends
    private void RebalanceTeams()
    {
        // Example: Here we simply balance based on player counts (red vs blue).
        // You can implement more sophisticated balancing logic based on player performance, preferences, etc.

        int redTeamCount = GetPlayerCount("red");
        int blueTeamCount = GetPlayerCount("blue");

        // Reassign players to teams based on counts
        if (redTeamCount > blueTeamCount)
        {
            // Move some red team players to blue
            MovePlayersToTeam("red", "blue", redTeamCount - blueTeamCount);
        }
        else if (blueTeamCount > redTeamCount)
        {
            // Move some blue team players to red
            MovePlayersToTeam("blue", "red", blueTeamCount - redTeamCount);
        }

        // Optionally: After rebalancing, you can reset players' positions or spawn them again
        ResetPlayerPositions();
    }

    // Get the count of players in a given team
    private int GetPlayerCount(string team)
    {
        int count = 0;
        // Example: You can use the NetworkManager or a list of players to count members of a team
        foreach (var player in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (player.PlayerObject.GetComponent<PlayerManager>().team == team)
            {
                count++;
            }
        }
        return count;
    }

    // Move a specific number of players from one team to another
    private void MovePlayersToTeam(string fromTeam, string toTeam, int numberToMove)
    {
        // Find players on the "fromTeam" and move them to the "toTeam"
        int moved = 0;
        foreach (var player in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (moved >= numberToMove) break;

            PlayerManager playerManager = player.PlayerObject.GetComponent<PlayerManager>();
            if (playerManager.team == fromTeam)
            {
                playerManager.SetTeam(toTeam);
                moved++;
            }
        }
    }

    // Reset players' positions after rebalancing
    private void ResetPlayerPositions()
    {
        foreach (var player in NetworkManager.Singleton.ConnectedClientsList)
        {
            PlayerManager playerManager = player.PlayerObject.GetComponent<PlayerManager>();
            Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint(playerManager.team);
            player.PlayerObject.transform.position = spawnPoint.position;
        }
    }

    // Allow the host to set a custom kill limit via the input field
    public void SetKillLimit()
    {
        if (int.TryParse(killLimitInputField.text, out int newKillLimit))
        {
            maxKills = newKillLimit;
        }
    }
}
