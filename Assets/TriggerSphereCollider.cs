using UnityEngine;

public class TriggerSphereCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider that entered is the player's collider
        if (other.gameObject.CompareTag("Player"))
        {
            // Call the sendMessage() function on the PlayerStatus script of the player object
            other.gameObject.GetComponent<PlayerStatus>().sendMessage("powerup");

            // Destroy the prefab game object
            Destroy(gameObject);
        }
    }
}
