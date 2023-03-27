using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("Status")]
    public float healthPoints = 100.0f;
    public float cocainePoints = 100.0f;
    public HealthBarScript healthBar;
    public PointsScript pointsScript;

    [Header("Cocaine Settings")]
    public float cocaineDecreaseRate = 2.5f;
    public float sleepThreshold = 0.0f;
    public HealthBarScript cocaineBar;

    [Header("Points")]
    public int points = 0;

    private void Start()
    {
        // Initialize the cocaine meter.
        cocainePoints = Mathf.Clamp(cocainePoints, 0.0f, cocainePoints);
        healthBar.SetMaxHealth(healthPoints);
    }

    public void setCocaineDrainRate(float rate)
    {
        
        if (rate == 0)
        {
            cocaineDecreaseRate = 2.5f;
        }
        cocaineDecreaseRate = rate;
    }

    public void sendMessage(string message)
    {
        if (message == "powerup")
        {
            cocainePoints += 50.0f;
            if (cocainePoints > 100.0f) cocainePoints = 100.0f;
            points++;

        } else if (message == "take damage")
        {
            TakeDamage(2.0f);
        } else if (message == "add point")
        {
            points++;
        } else if (message == "add health")
        {
            healthPoints += 25;
            if (healthPoints > 100)
            {
                healthPoints = 100;
            }
        }
        
        else
        {
            Debug.Log("Unknown message: " + message);
        }
    }

    private void Update()
    {
        // Decrease the cocaine meter over time.
        cocainePoints = Mathf.Clamp(cocainePoints - cocaineDecreaseRate * Time.deltaTime, 0.0f, cocainePoints);

        healthBar.setHealth(healthPoints);
        cocaineBar.setHealth(cocainePoints);
        pointsScript.setPoints(points);
    }

    void TakeDamage(float damage)
    {
        healthPoints -= damage;
        
    }
}
