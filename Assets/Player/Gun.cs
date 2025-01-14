using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class Gun : NetworkBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public Camera playerCamera;

    private PlayerInputActions.PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions.PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.fire.performed += _ => ShootServerRpc();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    [ServerRpc]
    void ShootServerRpc()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, range))
        {
            Debug.Log($"Hit {hit.collider.name}");
            if (hit.collider.TryGetComponent(out Health health))
            {
                health.TakeDamage(damage);
            }
        }
    }
}
