using UnityEngine;
using Unity.Netcode;

public class PaintballGunManager : NetworkBehaviour
{
    public enum GunType
    {
        Standard,
        RapidFire,
        Shotgun
    }

    [System.Serializable]
    public class Gun
    {
        public string name;
        public GunType type;
        public GameObject paintballPrefab;
        public Transform firePoint;
        public float fireRate;
        public float damage;
        public int pellets;
        public float spreadAngle;
    }

    public Gun[] guns; // Array of all available guns
    private Gun currentGun;
    private float nextFireTime;

    private void Start()
    {
        EquipGun(GunType.Standard); // Default gun at the start
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Fire();
        }
    }

    public void EquipGun(GunType type)
    {
        foreach (Gun gun in guns)
        {
            if (gun.type == type)
            {
                currentGun = gun;
                Debug.Log($"Equipped: {currentGun.name}");
                return;
            }
        }

        Debug.LogError($"Gun type {type} not found in the list!");
    }

    public void EquipGunByName(string gunName)
    {
        foreach (Gun gun in guns)
        {
            if (gun.name == gunName)
            {
                currentGun = gun;
                Debug.Log($"Equipped: {currentGun.name}");
                return;
            }
        }

        Debug.LogError($"Gun with name {gunName} not found in the list!");
    }

    private void Fire()
    {
        nextFireTime = Time.time + 1f / currentGun.fireRate;

        switch (currentGun.type)
        {
            case GunType.Standard:
            case GunType.RapidFire:
                SpawnPaintballServerRpc(currentGun.firePoint.position, currentGun.firePoint.forward, currentGun.damage);
                break;

            case GunType.Shotgun:
                for (int i = 0; i < currentGun.pellets; i++)
                {
                    Vector3 spread = currentGun.firePoint.forward +
                                     new Vector3(
                                         Random.Range(-currentGun.spreadAngle, currentGun.spreadAngle),
                                         Random.Range(-currentGun.spreadAngle, currentGun.spreadAngle),
                                         0
                                     );
                    SpawnPaintballServerRpc(currentGun.firePoint.position, spread.normalized, currentGun.damage / currentGun.pellets);
                }
                break;
        }
    }

    [ServerRpc]
    private void SpawnPaintballServerRpc(Vector3 position, Vector3 direction, float damage)
    {
        GameObject paintball = Instantiate(currentGun.paintballPrefab, position, Quaternion.LookRotation(direction));
        paintball.GetComponent<NetworkObject>().Spawn();

        Paintball paintballScript = paintball.GetComponent<Paintball>();
        if (paintballScript != null)
        {
            paintballScript.SetDamage(damage);
        }
    }
}

