using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Iced_RTea : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private string recipeName = "Iced_Tea";
    [SerializeField] private List<string> ingredients = new List<string> { "Red Tea", "Ice", "Water", "Water" };

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
        if (!customerSystem.recipes.ContainsKey(recipeName))
        {
            customerSystem.RegisterRecipe(recipeName, ingredients, true);
            Debug.Log($"Successfully registered {recipeName} recipe");
        }
        else
        {
            Debug.LogWarning($"{recipeName} recipe already exists!");
        }
    }


}