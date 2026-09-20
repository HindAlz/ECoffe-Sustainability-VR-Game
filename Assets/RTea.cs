using UnityEngine;
using System.Collections.Generic;

public class RTea : MonoBehaviour
{
    [Header("Recipe Configuration")]
    public string recipeName = "RTea";
    public List<string> ingredients = new List<string> { "Red Tea", "Water" };
    public bool isDrink = true;  // Explicitly mark this as a drink

    private CustomerSystem customerSystem;

    void Awake()
    {
        customerSystem = FindObjectOfType<CustomerSystem>();
        if (customerSystem == null)
        {
            Debug.LogError("CustomerSystem not found in scene!");
            return;
        }

        RegisterRecipe();
    }

    void RegisterRecipe()
    {
        customerSystem.RegisterRecipe(recipeName, ingredients, true);
        Debug.Log($"Registered {(isDrink ? "drink" : "food")} recipe: {recipeName}");
    }

    [ContextMenu("Test Recipe Registration")]
    void TestRegister()
    {
        RegisterRecipe();
    }
}