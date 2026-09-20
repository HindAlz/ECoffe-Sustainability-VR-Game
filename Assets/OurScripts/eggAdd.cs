using UnityEngine;
using System.Collections.Generic;

public class eggAdd : MonoBehaviour
{
    [Header("Recipe Configuration")]
    public string recipeName = "EggT";
    public List<string> ingredients = new List<string> { "Toast", "Cooked Egg" };
    public bool isDrink = false;  // Explicitly mark this as food

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
        customerSystem.RegisterRecipe(recipeName, ingredients, isDrink);
        Debug.Log($"Registered {(isDrink ? "drink" : "food")} recipe: {recipeName}");
    }
}