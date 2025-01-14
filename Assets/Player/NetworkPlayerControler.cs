using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerController : NetworkBehaviour
{
    public bool IsLocalPlayer = false;  // Flag to check if this is the local player
    public float moveSpeed = 5f;        // Movement speed of the player

    void Update()
    {
        if (!IsLocalPlayer) return;  // Ignore movement if not the local player

        // Handle player movement (you can modify this based on your input system)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;
        transform.Translate(move);
    }
}
