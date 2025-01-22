using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.Netcode; 

public class XRGrabbableGun : UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable
{
    [Header("Gun Settings")]
    [Tooltip("Prefab of the projectile to spawn. Must have a NetworkObject component.")]
    public GameObject projectilePrefab;

    [Tooltip("Spawn point from where the projectile is fired.")]
    public Transform projectileSpawnPoint;

    [Tooltip("Speed of the projectile.")]
    public float projectileSpeed = 20f;

    [Tooltip("Time between shots (in seconds).")]
    public float fireRate = 0.5f;

    private float lastFireTime;

    private NetworkObject networkObject;

    protected override void Awake()
    {
        base.Awake();

        // Ensure the gun has a NetworkObject
        networkObject = GetComponent<NetworkObject>();
        if (networkObject == null)
        {
            Debug.LogError("XRGrabbableGun requires a NetworkObject component for Netcode functionality.");
        }
    }

    protected override void OnActivated(ActivateEventArgs args)
    {
        base.OnActivated(args);

        // Check if the gun can fire based on fire rate
        if (Time.time >= lastFireTime + fireRate)
        {
            if (networkObject != null && networkObject.IsOwner)
            {
                ShootServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                lastFireTime = Time.time;
            }
        }
    }

    [ServerRpc]
    private void ShootServerRpc(Vector3 position, Quaternion rotation)
    {
        // Ensure a valid projectile prefab is assigned
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab is not assigned.");
            return;
        }

        // Spawn projectile on the server
        GameObject projectile = Instantiate(projectilePrefab, position, rotation);

        // Add NetworkObject and sync with clients
        NetworkObject projectileNetworkObject = projectile.GetComponent<NetworkObject>();
        if (projectileNetworkObject != null)
        {
            projectileNetworkObject.Spawn();
        }
        else
        {
            Debug.LogError("Projectile prefab must have a NetworkObject component.");
            Destroy(projectile);
            return;
        }

        // Apply velocity to the projectile
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = projectileSpawnPoint.forward * projectileSpeed;
        }

        // Destroy the projectile after a set time to avoid memory leaks
        Destroy(projectile, 5f);
    }
}
