using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    // Variable for the chance of a thief appearing at night
    public float nightThiefChance = 0.4f; // Default to 0.4 (before change)

    void Start()
    {
        // Adjust the chance of a thief appearing at night (change this value)
        nightThiefChance = 0.2f; // Set to 0.2 (new desired value)
        Debug.Log("Night Thief Chance: " + nightThiefChance);
    }

    // Example method to spawn a customer (including the thief)
    void SpawnCustomer()
    {
        // If it's night time, check for the chance of a thief
        if (IsNightTime())
        {
            float randomChance = Random.Range(0f, 1f);
            if (randomChance <= nightThiefChance)
            {
                Debug.Log("A thief has appeared!");
                // Add code to handle the thief appearance
            }
            else
            {
                Debug.Log("A regular customer has appeared.");
                // Add code for regular customer spawn
            }
        }
    }

    // Placeholder method to simulate day/night cycle
    bool IsNightTime()
    {
        // Assuming a simple condition where night is true for this example
        return true; // Or use actual game time to check if it's night
    }
}
