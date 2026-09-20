using UnityEngine;
using System.Collections.Generic;

public class OrangeJ : MonoBehaviour
{
    [SerializeField] private CustomerSystem customerSystem;

    void Start()
    {
        if (customerSystem == null)
            customerSystem = FindObjectOfType<CustomerSystem>();

        AddOrangeJamRecipe();
    }

    public void AddOrangeJamRecipe()
    {
        if (customerSystem == null)
        {
            Debug.LogError("CustomerSystem not found!");
            return;
        }

        // Add Orange Jam Toast recipe
        customerSystem.RegisterRecipe("OJam",  new List<string> { "Microwaved", "Orange Jam" }, false);

        Debug.Log("Added Orange Jam Toast recipe");
    }
}