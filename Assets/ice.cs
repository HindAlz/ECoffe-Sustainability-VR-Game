using UnityEngine;
using System.Collections.Generic;

public class Americano : MonoBehaviour
{
    [SerializeField] private bool isDrink = true;

    void Awake()
    {
        var customerSystem = FindObjectOfType<CustomerSystem>();
        if (customerSystem == null)
        {
            Debug.LogError("CustomerSystem not found!");
            return;
        }

        // Latte: 3 milk + 1 coffee
        customerSystem.RegisterRecipe("Americano",
            new List<string> { "Water", "Ice", "Coffee", "Coffee" },
            isDrink);

        Debug.Log($"Registered {(isDrink ? "drink" : "food")}: Latte");
    }
}