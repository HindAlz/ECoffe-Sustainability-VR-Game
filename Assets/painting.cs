using UnityEngine;

public class Painting : MonoBehaviour
{
    void Start()
    {
        // Get the customer system
        CustomerSystem customerSystem = FindObjectOfType<CustomerSystem>();

        if (customerSystem != null)
        {
            // Directly set angry customer chance to 20%
            customerSystem.dayAngryChance = 0f;
            Debug.Log("Calming painting installed! Angry chance now: 0.2");
        }
        else
        {
            Debug.LogWarning("No CustomerSystem found - painting has no effect");
        }
    }
}