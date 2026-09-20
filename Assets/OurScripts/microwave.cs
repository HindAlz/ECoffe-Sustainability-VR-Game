using UnityEngine;
using System.Collections;

public class Microwave : MonoBehaviour
{
    public Material uncookedMaterial; // Pale Croissant Material
    public Material cookedMaterial; // Cooked Croissant Material
    public float cookTime = 5f; // Time in seconds to cook the croissant
    public ImgsFillDynamic progressBarUI; // Reference to the fill bar
    public AudioClip dingSound; // The "ding" sound clip

    private bool isCooking = false;
    private GameObject croissant;
    private bool isCooked = false;

    private AudioSource audioSource; // AudioSource for playing sound

    private void Start()
    {
        // Add an AudioSource component if not already present
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the oven is the croissant
        if (other.CompareTag("Toast"))
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
        // If the croissant leaves the oven, stop cooking
        if (other.CompareTag("Toast") && other.gameObject == croissant)
        {
            StopCooking();
            croissant = null; // Reset the croissant reference
        }
    }

    private void StartCooking()
    {
        if (!isCooking && !isCooked)
        {
            isCooking = true;
            isCooked = false;

            SetCroissantMaterial(uncookedMaterial);

            // Start progress bar from 0
            if (progressBarUI != null)
                progressBarUI.SetValue(0f, true);

            StartCoroutine(CookingCoroutine());
        }
    }

    IEnumerator CookingCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < cookTime)
        {
            if (!isCooking) yield break; // stop if cooking was interrupted

            elapsed += Time.deltaTime;

            // Update fill amount (0 to 1)
            if (progressBarUI != null)
                progressBarUI.SetValue(elapsed / cookTime);

            yield return null;
        }

        CookCroissant();
    }

    private void CookCroissant()
    {
        if (croissant != null)
        {
            // Change croissant material to cooked
            SetCroissantMaterial(cookedMaterial);
            isCooked = true;
            Debug.Log("Croissant is cooked!");

            // Play the "ding" sound when cooking is done
            if (dingSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(dingSound);
            }
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
