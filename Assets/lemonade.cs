using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class lemonade : MonoBehaviour
{
    [Header("Lemon Juice Recipe")]
    [SerializeField] private string recipeName = "Lemonade";
    [SerializeField] private List<string> ingredients = new List<string> { "Lemon Juice", "Ice" };
    [SerializeField] private bool isDrink = true;

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
        Debug.Log($"Registered {(isDrink ? "drink" : "food")}: {recipeName}");
    }

    [ContextMenu("Test Registration")]
    void TestRegister()
    {
        RegisterRecipe();
    }
}