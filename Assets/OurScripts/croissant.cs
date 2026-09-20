using UnityEngine;
using UnityEngine.UI; // To use the Text component
using System.Collections;

public class croissant : MonoBehaviour
{
    public GameObject PlasticPlate;
    public GameObject GlassPlate;
    bool onPlate;

    Renderer CriosRenderer;
    bool isCooking;
    public bool isCooked;

    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable Grab;
    public Material material;

    public ImgsFillDynamic fillBar; // Progress bar reference
    public Text txtValue; // Reference to the text for showing "Baking..." or "Complete!"
    public float cookTime = 5f; // Time to cook the croissant (adjustable)

    // Add an AudioSource and AudioClip for the sound effect
    private AudioSource audioSource; // Reference to AudioSource component
    public AudioClip dingSound; // The "ding" sound to play when cooking is complete

    private void Start()
    {
        Grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        CriosRenderer = GetComponent<Renderer>();

        // Get the AudioSource component from the GameObject
        audioSource = GetComponent<AudioSource>();

        if (fillBar != null)
            fillBar.gameObject.SetActive(false); // Hide the fill bar initially
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlasticPlate"))
        {
            PlaceOnPlate(PlasticPlate);
        }
        else if (other.CompareTag("GlassPlate"))
        {
            PlaceOnPlate(GlassPlate);
        }
        else if (other.CompareTag("Oven") && !isCooking && !isCooked)
        {
            isCooking = true;
            StartCoroutine(OvenSequence());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Ensure we're detecting the exit correctly
        if (other.CompareTag("Oven") && !isCooking)
        {
            StopCooking();
        }
    }

    private IEnumerator OvenSequence()
    {
        if (fillBar != null)
        {
            fillBar.SetValue(0f, true); // Reset the progress bar

            fillBar.gameObject.SetActive(true); // Show progress bar
        }

        if (txtValue != null)
        {
            txtValue.text = "Baking"; // Start with "Baking" text
            StartCoroutine(UpdateBakingText()); // Start the coroutine for the dots
        }

        float elapsedTime = 0f; // Timer to track the cooking time

        while (elapsedTime < cookTime)
        {
            elapsedTime += Time.deltaTime; // Increment time
            float progress = Mathf.Clamp01(elapsedTime / cookTime); // Calculate progress percentage
            fillBar.SetValue(progress, false); // Update the progress bar

            yield return null; // Wait for the next frame
        }

        OvenCrios(); // Finish cooking once the time is up
    }

    private IEnumerator UpdateBakingText()
    {
        string baseText = "Baking";
        int dotCount = 0;

        while (isCooking)
        {
            dotCount = (dotCount + 1) % 4; // Cycles between 0 to 3 dots
            txtValue.text = baseText + new string('.', dotCount); // Update text with dots
            yield return new WaitForSeconds(0.5f); // Wait for half a second before updating
        }
    }

    private void Pause()
    {
        if (Grab != null)
        {
            Grab.enabled = false; // Disable interaction during cooking
        }
    }

    private void PlaceOnPlate(GameObject plate)
    {
        if (plate != null && !onPlate)
        {
            plate.SetActive(true);
            onPlate = true;
        }
    }

    private void StopCooking()
    {
        if (fillBar != null)
        {
            fillBar.gameObject.SetActive(false); // Hide the progress bar when croissant leaves oven
            fillBar.SetValue(0f, true); // Reset the progress bar

        }

        if (Grab != null)
        {
            Grab.enabled = true; // Re-enable interaction after cooking
        }

        if (txtValue != null)
        {
            txtValue.text = ""; // Clear the text when cooking stops
        }

        isCooking = false; // Stop cooking when exiting the oven
        isCooked = false; // Reset cooked state
    }

    public void OvenCrios()
    {
        if (Grab != null)
        {
            Grab.enabled = true; // Re-enable interaction after cooking
        }

        if (CriosRenderer != null && material != null)
        {
            CriosRenderer.material = material; // Apply cooked material
        }

        if (txtValue != null)
        {
            txtValue.text = "Complete!"; // Show "Complete!" once the croissant is cooked
        }

        isCooked = true;
        isCooking = false; // Reset flag

        // Play the "ding" sound when cooking is complete
        if (audioSource != null && dingSound != null)
        {
            audioSource.PlayOneShot(dingSound);
        }
    }
}
