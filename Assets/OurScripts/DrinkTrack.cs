using System.Collections.Generic;
using System.Linq; // For list comparison
using UnityEngine;

public class DrinkTrack : MonoBehaviour
{
    public List<string> drinkList = new List<string>();
    private GameObject latteArt; // Reference to latteArt child
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable; // For detecting grab state

    void Start()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>(); // Get the XR grab interactable component

        // Find the latteArt object within this GameObject's children
        latteArt = transform.Find("latteArt")?.gameObject;

        // Ensure it's disabled at the start
        if (latteArt != null)
            latteArt.SetActive(false);
    }

    public void AddDrink(string drink)
    {
        if (drinkList.Count < 4)
        {
            drinkList.Add(drink);
            Debug.Log("Drink added: " + drink);
        }

        // Count the occurrences of "coffee" and "milk"
        int coffeeCount = drinkList.Count(d => d == "coffee");
        int milkCount = drinkList.Count(d => d == "milk");

        // Check if the drink list contains exactly 3 coffees and 1 milk, no matter the order
        if (coffeeCount == 3 && milkCount == 1)
        {
            Debug.Log("Latte Art enabled!");
            if (latteArt != null)
                latteArt.SetActive(true);
        }
    }

    public bool HasDrink(string drinkName)
    {
        return drinkList.Contains(drinkName);
    }
}
