using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GreenTeaRecipe : MonoBehaviour
{
    [Header("Green Tea Recipe")]
    [SerializeField] private string recipeName = "Green_Tea";
    [SerializeField] private List<string> ingredients = new List<string> { "Green Tea", "Water", "Water", "Water" };
    [SerializeField] private bool isDrink = true;

    private CustomerSystem customerSystem;

    void Awake()
    {
        customerSystem = FindObjectOfType<CustomerSystem>();
        if (customerSystem == null)
        {
            Debug.LogError("CustomerSystem reference missing!");
            enabled = false;
            return;
        }

        RegisterRecipe();
    }

    void RegisterRecipe()
    {
        customerSystem.RegisterRecipe(recipeName, ingredients, isDrink);
        Debug.Log($"Registered {(isDrink ? "drink" : "food")}: {recipeName}");
    }
}