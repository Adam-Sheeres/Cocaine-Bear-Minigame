using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("Status")]
    public float healthPoints = 100.0f;
    public float cocainePoints = 100.0f;
    public HealthBarScript healthBar; 

    [Header("Cocaine Settings")]
    public float cocaineDecreaseRate = 5.0f;
    public float sleepThreshold = 0.0f;
    public HealthBarScript cocaineBar;

    private bool isSleeping = false;

    private void Start()
    {
        // Initialize the cocaine meter.
        cocainePoints = Mathf.Clamp(cocainePoints, 0.0f, cocainePoints);
        healthBar.SetMaxHealth(healthPoints);
    }

    private void Update()
    {
        // Decrease the cocaine meter over time.
        cocainePoints = Mathf.Clamp(cocainePoints - cocaineDecreaseRate * Time.deltaTime, 0.0f, cocainePoints);

        healthBar.setHealth(healthPoints);
        cocaineBar.setHealth(cocainePoints);

        // Check if the cocaine meter has reached the sleep threshold.
        if (cocainePoints <= sleepThreshold && !isSleeping)
        {
            // Call the sleep function.
            outOfCocaine();
        }
    }

    void TakeDamage(float damage)
    {
        healthPoints -= damage;
        
    }

    private void outOfCocaine()
    {
        // Set the player to sleep.
        isSleeping = true;

        // TODO: Implement sleep functionality here.
        Debug.Log("Cocaine level at 0");

        // Reset the cocaine meter.
        cocainePoints = 100.0f;

        // Wake the player up.
        isSleeping = false;
    }
}
