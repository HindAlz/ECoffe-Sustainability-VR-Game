using UnityEngine;
using System.Collections.Generic;

public class CherryCup : MonoBehaviour
{
    public string recipeName = "CherryCup";
    public List<string> ingredients = new List<string> { "CherryCup" };
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
