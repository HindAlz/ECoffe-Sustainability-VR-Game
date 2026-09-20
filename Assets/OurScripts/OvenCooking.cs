using UnityEngine;

public class OvenCooking : MonoBehaviour
{
    public Material uncookedMaterial; // Pale Croissant Material
    public Material cookedMaterial; // Cooked Croissant Material
    public float cookTime = 5f; // Time in seconds to cook the croissant

    private bool isCooking = false;
    private GameObject croissant;
    private bool isCooked = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the oven is the croissant
        if (other.CompareTag("Croissant"))
        {
            // If there's already a croissant in the oven, ignore the new one
            if (croissant != null)
            {
                Debug.Log("Oven is already cooking a croissant.");
                return;
            }

            // Start cooking
            croissant = other.gameObject;
            StartCooking();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the croissant leaves the oven, stop cooking and reset state
        if (other.CompareTag("Croissant") && other.gameObject == croissant)
        {
            StopCooking();
            croissant = null; // Reset the croissant reference
            isCooked = false; // Reset the cooked state
        }
    }

    private void StartCooking()
    {
        if (!isCooking && !isCooked)
        {
            isCooking = true;
            isCooked = false;
            // Change croissant material to uncooked (pale)
            SetCroissantMaterial(uncookedMaterial);
            // Start cooking process after cookTime
            Invoke("CookCroissant", cookTime);
        }
    }

    private void CookCroissant()
    {
        if (croissant != null)
        {
            // Change croissant material to cooked
            SetCroissantMaterial(cookedMaterial);
            isCooked = true;
            Debug.Log("Croissant is cooked!");
        }
    }

    private void StopCooking()
    {
        if (isCooking)
        {
            isCooking = false;

            if (!isCooked)
            {
                SetCroissantMaterial(uncookedMaterial);
            }
        }
    }

    private void SetCroissantMaterial(Material material)
    {
        // Ensure the croissant has a Renderer and update the material
        if (croissant != null)
        {
            Renderer croissantRenderer = croissant.GetComponent<Renderer>();
            if (croissantRenderer != null)
            {
                croissantRenderer.material = material;
            }
        }
    }

    public bool IsCooked()
    {
        return isCooked;
    }
}