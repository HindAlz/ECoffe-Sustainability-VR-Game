using UnityEngine;
using System.Collections.Generic;

public class PinkDon : MonoBehaviour
{
    public string recipeName = "PinkDon";
    public List<string> ingredients = new List<string> { "PinkDon" };
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
