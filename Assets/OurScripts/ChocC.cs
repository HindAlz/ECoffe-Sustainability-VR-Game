using UnityEngine;
using System.Collections.Generic;

public class ChocC : MonoBehaviour
{
    public string recipeName = "ChocC";
    public List<string> ingredients = new List<string> { "ChocC" };
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