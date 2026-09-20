using UnityEngine;
using System.Collections.Generic; // Add this line


public class cheese : MonoBehaviour
{
    [SerializeField] private CustomerSystem customerSystem;

    void Start()
    {
        if (customerSystem == null)
        {
            customerSystem = FindObjectOfType<CustomerSystem>();
        }

        AddRecipe();
    }

    public void AddRecipe()
    {
        if (customerSystem == null)
        {
            Debug.LogError("CustomerSystem not found!");
            return;
        }

        // Directly add to the recipes dictionary
        customerSystem.RegisterRecipe("CherryT",
            new List<string> { "Microwaved", "Tomato", "Cheese", "Basil" }, false);

        Debug.Log("Added Cherry Tomato Toast recipe to CustomerSystem");
    }
}