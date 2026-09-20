using UnityEngine;
using System.Collections.Generic;

public class OreoCup : MonoBehaviour
{
    public string recipeName = "OreoCup";
    public List<string> ingredients = new List<string> { "OreoCup" };
    public bool isDrink = false;

    private CustomerSystem customerSystem;

    void Awake()
    {
        customerSystem = FindObjectOfType<CustomerSystem>();
        RegisterRecipe();
    }

    void RegisterRecipe()
    {
        customerSystem.RegisterRecipe(recipeName, ingredients, isDrink);
        Debug.Log($"Registered {(isDrink ? "drink" : "food")} recipe: {recipeName}");
    }
}
