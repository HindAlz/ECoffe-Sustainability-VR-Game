using UnityEngine;
using System.Collections;

public class Milk : MonoBehaviour
{
    private GameObject drink;
    private bool isDrinkFull = false;
    private float currentFillPercentage = 0f;

    public Material milkMaterialAlone;
    public Material milkInCoffeeMaterial;

    public float[] basePositionY;
    public float[] maxPositionY;
    public float fillIncrement = 0.25f;

    public AudioClip pourSound;  // The pouring sound clip
    private AudioSource audioSource; // AudioSource component

    private void Start()
    {
        // Ensure the AudioSource is attached to the same GameObject
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EspressoCup") || other.CompareTag("Mug") || other.CompareTag("IceCup"))
        {
            Transform drinkTransform = other.transform.Find("Drink");
            if (drinkTransform != null)
            {
                drink = drinkTransform.gameObject;
                isDrinkFull = false;
                currentFillPercentage = 0f;

                FillCup(other.tag);
            }
        }
    }

    private void FillCup(string cupType)
    {
        if (drink != null && !isDrinkFull)
        {
            DrinkTrack tracker = drink.transform.parent.GetComponent<DrinkTrack>();
            if (tracker != null)
            {
                tracker.AddDrink("Milk");

                // Set material depending on whether coffee was already added
                Material selectedMaterial = tracker.HasDrink("Coffee") ? milkInCoffeeMaterial : milkMaterialAlone;

                Renderer drinkRenderer = drink.GetComponent<Renderer>();
                if (drinkRenderer != null && selectedMaterial != null)
                {
                    drinkRenderer.material = selectedMaterial;
                }
            }

            // Play the pouring sound at the start of the fill process
            if (audioSource != null && pourSound != null)
            {
                audioSource.PlayOneShot(pourSound);  // Play the pouring sound
            }

            StartCoroutine(FillCupCoroutine(cupType));
        }
    }

    private IEnumerator FillCupCoroutine(string cupType)
    {
        int cupIndex = GetCupTypeIndex(cupType);
        if (cupIndex == -1) yield break;

        float baseY = basePositionY[cupIndex];
        float maxY = maxPositionY[cupIndex];
        float currentY = drink.transform.localPosition.y;
        float fillStep = (maxY - baseY) * 0.25f;
        float targetY = Mathf.Min(currentY + fillStep, maxY);

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
        currentFillPercentage += 0.25f;
        if (currentFillPercentage >= 1f) isDrinkFull = true;
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
}
