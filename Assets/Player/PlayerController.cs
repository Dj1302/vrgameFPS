using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f; // Speed of player movement
    public float lookSensitivity = 2f; // Sensitivity for looking around
    public Transform cameraTransform; // Assign the player's camera in the Inspector
    public GameObject projectilePrefab; // Assign a projectile prefab in the Inspector
    public Transform firePoint; // Position to spawn projectiles
    public float projectileSpeed = 20f; // Speed of the projectile

    private CharacterController characterController;
    private PlayerInputActions.PlayerInputActions inputActions;
    private Vector2 moveInput;
    private Vector2 lookInput;

    private void Awake()
    {
        inputActions = new PlayerInputActions.PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        inputActions.Player.fire.performed += ctx => Fire(); // Call Fire method when the Fire action is triggered
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Start()
    {
        if (IsOwner)  // Only allow the local player to control their own movement
        {
            characterController = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void Update()
    {
        if (IsOwner) // Only allow the local player to control their movement
        {
            HandleMovement();
            HandleLook();
        }
    }

    private void HandleMovement()
    {
        // Convert 2D movement input into 3D movement
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        characterController.Move(move * moveSpeed * Time.deltaTime);
    }

    private void HandleLook()
    {
        transform.Rotate(Vector3.up, lookInput.x * lookSensitivity * Time.deltaTime);
        cameraTransform.Rotate(Vector3.left, lookInput.y * lookSensitivity * Time.deltaTime);
    }

    // This method will be called on the server to spawn the projectile
    [ServerRpc(RequireOwnership = false)]
    private void FireServerRpc()
    {
        // Ensure the Fire method only executes on the server
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("Projectile Prefab or Fire Point is not assigned!");
            return;
        }

        // Spawn the projectile on the server and spawn it for all clients
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        NetworkObject networkObject = projectile.GetComponent<NetworkObject>();
        networkObject.Spawn();  // This spawns the projectile for all clients

        // Apply velocity to the projectile
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = firePoint.forward * projectileSpeed;
        }
    }

    // Fire method to be called locally
    private void Fire()
    {
        if (IsOwner)  // Make sure only the owning player can fire
        {
            FireServerRpc();  // Call the ServerRpc to spawn the projectile
        }
    }
}
