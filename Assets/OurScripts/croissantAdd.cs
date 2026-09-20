using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CroissantRecipe : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private string recipeName = "Croissant";
    [SerializeField] private List<string> ingredients = new List<string> { "Cooked" };

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
            customerSystem.RegisterRecipe(recipeName, ingredients, false);
            Debug.Log($"Successfully registered {recipeName} recipe");
        }
        else
        {
            Debug.LogWarning($"{recipeName} recipe already exists!");
        }
    }

    // Editor button for testing
    [ContextMenu("Test Recipe Registration")]
    void TestRegister()
    {
        RegisterRecipe();
    }
}