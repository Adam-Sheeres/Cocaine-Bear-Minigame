using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStatus : MonoBehaviour
{
    [Header("Stats")]
    public float healthPoints = 10.0f;
    public bool hasCocaine = false;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {

    }

    public void sendMessage(string message, int value)
    {
        switch (message) {
            case "take damage":
                healthPoints -= value;
            break;
            case "being eaten":
                Destroy(gameObject, 10.0f);
                break;
        }
        
    }
}