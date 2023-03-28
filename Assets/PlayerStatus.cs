using UnityEngine;
using UnityEngine.Audio;

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

    [Header("Sound Effects")]
    public AudioSource deathSound;
    public AudioSource hit_sound;
    public AudioSource power_up_sound;
    public AudioSource earned_points;

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
            power_up_sound.Play();
            cocainePoints += 50.0f;
            if (cocainePoints > 100.0f) cocainePoints = 100.0f;
            points++;

        } else if (message == "take damage")
        {
            hit_sound.Play();
            TakeDamage(8.0f);
        } else if (message == "add point")
        {
            earned_points.Play();
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

        if (cocainePoints <= 5 && healthPoints >= 25)
        {
            cocainePoints = 5;
            healthPoints = Mathf.Clamp(healthPoints - cocaineDecreaseRate * Time.deltaTime, 0.0f, healthPoints);
        }

        healthBar.setHealth(healthPoints);
        cocaineBar.setHealth(cocainePoints);
        pointsScript.setPoints(points);
    }

    void TakeDamage(float damage)
    {
        healthPoints -= damage;
        
    }
}
