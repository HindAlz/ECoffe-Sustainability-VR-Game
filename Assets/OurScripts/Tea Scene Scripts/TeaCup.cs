using UnityEngine;

public class TeaCup : MonoBehaviour
{
    private bool hasWater = false;

    public GameObject waterObject;  // Assign water mesh
    public Material brownWaterMaterial; // Assign brown material (for red tea)
    public Material greenWaterMaterial; // Assign green material (for green tea)
    public bool isRed;
    public bool isGreen;
    public GameObject teabag;

    private Renderer waterRenderer;
    private DrinkTrack drinkTrack; // Reference to DrinkTrack
    private AudioSource audioSource; // Reference to AudioSource component
    public AudioClip pourSound; // Sound to play when water is poured

    void Start()
    {
        waterObject.SetActive(false);
        waterRenderer = waterObject.GetComponent<Renderer>();

        // Get DrinkTrack attached to the same GameObject
        drinkTrack = GetComponent<DrinkTrack>();

        if (drinkTrack == null)
        {
            Debug.LogError("DrinkTrack component is missing on the TeaCup object!");
        }

        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing on the TeaCup object!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Teapot"))
        {
            AddWater();
        }
        else if (other.CompareTag("RedTeaBox"))
        {
            AddToDrinkList("Red Tea");

            isRed = true;
            teabag.SetActive(true);
            if (hasWater) ChangeWaterColorRed();
        }
        else if (other.CompareTag("GreenTeaBox"))
        {
            isGreen = true;
            teabag.SetActive(true);
            if (hasWater) ChangeWaterColorGreen();

        }
    }

    public void AddWater()
    {
        if (!hasWater)
        {
            hasWater = true;
            waterObject.SetActive(true);
            AddToDrinkList("Water");

            // Change water color based on tea type
            if (isRed)
                ChangeWaterColorRed();
            else if (isGreen)
                ChangeWaterColorGreen();

            // Play the sound when water is added
            PlayPourSound();
        }
    }

    void ChangeWaterColorRed()
    {
        if (waterRenderer != null && brownWaterMaterial != null)
        {
            waterRenderer.material = brownWaterMaterial;
        }
        else
        {
            Debug.LogWarning("Water Renderer or Brown Water Material is not assigned.");
        }
    }

    void ChangeWaterColorGreen()
    {
        if (waterRenderer != null && greenWaterMaterial != null)
        {
            waterRenderer.material = greenWaterMaterial;
        }
        else
        {
            Debug.LogWarning("Water Renderer or Green Water Material is not assigned.");
        }
    }

    void AddToDrinkList(string drinkName)
    {
        if (drinkTrack != null)
        {
            drinkTrack.AddDrink(drinkName);
        }
    }

    // Method to play the pouring sound
    void PlayPourSound()
    {
        if (audioSource != null && pourSound != null)
        {
            audioSource.PlayOneShot(pourSound);
        }
    }
}
