using UnityEngine;
using System.Collections.Generic;

public class EspressoRecipe : MonoBehaviour
{
    void Awake()
    {
        var customerSystem = FindObjectOfType<CustomerSystem>();
        if (customerSystem == null) return;

        // Espresso: 4 coffee shots

        customerSystem.RegisterRecipe("Espresso",
            new List<string> { "Coffee", "Coffee", "Coffee", "Coffee" }, true);

        Debug.Log("Added Espresso (4 shots)");
    }
}