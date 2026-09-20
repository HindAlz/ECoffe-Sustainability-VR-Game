using UnityEngine;
using System.Collections;

public class CupFiller : MonoBehaviour
{
    protected GameObject drink;
    private bool isCupInHolder = false;
    protected bool isDrinkFull = false;
    protected float currentFillPercentage = 0f;

    public string correctCupType = "All";
    public string drinkName = "Coffee"; // Set in Inspector

    public float[] basePositionY;
    public float[] maxPositionY;
    public float fillIncrement = 0.25f;

    // Particle system for the pouring effect
    public ParticleSystem pourParticleSystem;
    // AudioSource for the pouring sound
    public AudioSource pourAudioSource;
    public AudioClip pourSound;

    private bool isPouring = false;  // Flag to check if pouring is happening

    private void OnTriggerEnter(Collider other)
    {
        if (correctCupType == "All" || other.CompareTag(correctCupType) ||
            other.CompareTag("EspressoCup") || other.CompareTag("Mug") || other.CompareTag("IceCup")) // Accept IceCup too
        {
            Transform drinkTransform = other.transform.Find("Drink");
            if (drinkTransform != null)
            {
                drink = drinkTransform.gameObject;
                isCupInHolder = true;
                isDrinkFull = false;
                currentFillPercentage = 0f;
                Debug.Log($"Cup entered: {other.tag}"); // Debug for cup detected
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EspressoCup") || other.CompareTag("Mug") || other.CompareTag("IceCup"))
        {
            isCupInHolder = false;
            drink = null;

            Rigidbody cupRb = other.GetComponent<Rigidbody>();
            if (cupRb != null)
            {
                cupRb.useGravity = true;
            }

            Debug.Log($"Cup exited: {other.tag}"); // Debug for cup exit
        }
    }

    public Material coffeeMixMaterial;
    public Material defaultMaterial;

    public void FillCup(Material selectedMaterial)
    {
        if (isCupInHolder && drink != null && !isDrinkFull && !isPouring)
        {
            isPouring = true;

            DrinkTrack tracker = drink.transform.parent.GetComponent<DrinkTrack>();
            if (tracker != null)
            {
                tracker.AddDrink(drinkName);

                if (tracker.HasDrink("Milk"))
                {
                    selectedMaterial = coffeeMixMaterial;
                }
                else
                {
                    selectedMaterial = defaultMaterial;
                }
            }

            SetDrinkMaterial(selectedMaterial);
            string cupType = drink.transform.parent.tag;
            StartCoroutine(FillCupCoroutine(cupType));
        }
    }

    private IEnumerator FillCupCoroutine(string cupType)
    {
        int cupIndex = GetCupTypeIndex(cupType);
        if (cupIndex == -1)
        {
            Debug.LogWarning($"Unknown cup type: {cupType}"); // Debug warning if cup type not found
            yield break;
        }

        // Play particle effect and sound when the pouring starts
        if (pourParticleSystem != null)
        {
            pourParticleSystem.Play();
        }

        if (pourAudioSource != null && pourSound != null)
        {
            pourAudioSource.clip = pourSound;
            pourAudioSource.Play();
        }

        // Wait for 0.4 seconds
        yield return new WaitForSeconds(0.4f);

        // Stop particle and sound
        if (pourParticleSystem != null)
        {
            pourParticleSystem.Stop();
        }

        if (pourAudioSource != null)
        {
            pourAudioSource.Stop();
        }

        // Filling the drink
        float baseY = basePositionY[cupIndex];
        float maxY = maxPositionY[cupIndex];
        float currentY = drink.transform.localPosition.y;
        float fillStep = (maxY - baseY) * fillIncrement;
        float targetY = Mathf.Min(currentY + fillStep, maxY);

        Debug.Log($"Filling {cupType}: CurrentY={currentY:F3} TargetY={targetY:F3}"); // <-- ADD THIS TO SHOW FILLING INFO

        float timeElapsed = 0f;
        float fillDuration = 0.5f;

        while (timeElapsed < fillDuration)
        {
            drink.transform.localPosition = new Vector3(
                drink.transform.localPosition.x,
                Mathf.Lerp(currentY, targetY, timeElapsed / fillDuration),
                drink.transform.localPosition.z
            );
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        drink.transform.localPosition = new Vector3(drink.transform.localPosition.x, targetY, drink.transform.localPosition.z);
        currentFillPercentage += fillIncrement;
        if (currentFillPercentage >= 1f)
        {
            isDrinkFull = true;
            Debug.Log("Drink is now full!");
        }

        // Allow pouring again after a short delay
        yield return new WaitForSeconds(0.5f);
        isPouring = false;
    }

    private int GetCupTypeIndex(string cupType)
    {
        switch (cupType)
        {
            case "EspressoCup": return 0;
            case "Mug": return 1;
            case "IceCup": return 2;
            default: return -1;
        }
    }

    private void SetDrinkMaterial(Material selectedMaterial)
    {
        if (drink != null && selectedMaterial != null)
        {
            Renderer drinkRenderer = drink.GetComponent<Renderer>();
            if (drinkRenderer != null)
            {
                drinkRenderer.material = selectedMaterial;
            }
        }
    }
}
