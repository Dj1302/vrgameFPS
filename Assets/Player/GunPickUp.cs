using UnityEngine;

public class GunPickup : MonoBehaviour
{
    public string gunName; // The name of the gun (e.g., "Standard", "RapidFire", "Shotgun")

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var gunManager = other.GetComponent<PaintballGunManager>();
            if (gunManager != null)
            {
                gunManager.EquipGunByName(gunName);
                Debug.Log($"{other.name} picked up {gunName}");
                Destroy(gameObject); // Remove the gun from the scene after pickup
            }
        }
    }
}
