using UnityEngine;
using System.Collections.Generic;

public class StrawberryJ : MonoBehaviour
{
    [SerializeField] private CustomerSystem customerSystem;

    void Start()
    {
        if (customerSystem == null)
            customerSystem = FindObjectOfType<CustomerSystem>();

        AddStrawberryJamRecipe();
    }

    public void AddStrawberryJamRecipe()
    {
        if (customerSystem == null)
        {
            Debug.LogError("CustomerSystem not found!");
            return;
        }

        // Add Strawberry Jam Toast recipe
        customerSystem.recipes.Add("SJam",
            new List<string> { "Microwaved", "Strawberry Jam" });

        Debug.Log("Added Strawberry Jam Toast recipe");
    }
}