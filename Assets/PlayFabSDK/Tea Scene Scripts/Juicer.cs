using UnityEngine;
using System.Collections;

public class Juicer : MonoBehaviour
{
    [Header("Visual Settings")]
    public Material orangeJuiceMaterial;
    public Material lemonJuiceMaterial;
    public float basePositionY = -0.1f;
    public float maxPositionY = 0.1f;
    public float fillIncrement = 0.25f; // 25% per interaction
    public float fillDuration = 0.5f; // Duration for each fill step

    [Header("Audio")]
    public AudioClip pourSound;
    private AudioSource audioSource;

    private GameObject currentDrink;
    private bool hasOrange = false;
    private bool hasLemon = false;
    private bool isFilling = false;
    private float currentFillPercentage = 0f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isFilling) return;

        if (other.CompareTag("Orange"))
        {
            Destroy(other.gameObject);
            hasOrange = true;
            TryStartFilling();
        }
        else if (other.CompareTag("Lemon"))
        {
            Destroy(other.gameObject);
            hasLemon = true;
            TryStartFilling();
        }
        else if (other.CompareTag("IceCup"))
        {
            IceTeaCup cup = other.GetComponent<IceTeaCup>();
            if (cup != null && !cup.hasJuice && !cup.isTea())
            {
                Transform drinkTransform = other.transform.Find("Drink");
                if (drinkTransform != null)
                {
                    currentDrink = drinkTransform.gameObject;
                    currentFillPercentage = 0f;
                    TryStartFilling();
                }
            }
        }
    }

    private void TryStartFilling()
    {
        if (currentDrink != null && (hasOrange || hasLemon) && !isFilling)
        {
            StartCoroutine(FillCupCoroutine());
        }
    }

    private IEnumerator FillCupCoroutine()
    {
        isFilling = true;

        // Set juice material if we have a drink object
        if (currentDrink != null)
        {
            Renderer drinkRenderer = currentDrink.GetComponent<Renderer>();
            if (drinkRenderer != null)
            {
                // Determine which material to use
                Material juiceMaterial = null;
                if (hasOrange && hasLemon)
                {
                    // If you want a mixed juice, you could add a mixed material here
                    juiceMaterial = orangeJuiceMaterial; // Default to orange for now
                }
                else if (hasOrange)
                {
                    juiceMaterial = orangeJuiceMaterial;
                }
                else if (hasLemon)
                {
                    juiceMaterial = lemonJuiceMaterial;
                }

                if (juiceMaterial != null)
                {
                    drinkRenderer.material = juiceMaterial;
                    // Also enable the renderer in case it was disabled
                    drinkRenderer.enabled = true;
                }
            }
        }

        float currentY = currentDrink.transform.localPosition.y;
        float fillStep = (maxPositionY - basePositionY) * fillIncrement;
        float targetY = Mathf.Min(currentY + fillStep, maxPositionY);

        // Animate filling
        float timeElapsed = 0f;
        while (timeElapsed < fillDuration)
        {
            currentDrink.transform.localPosition = new Vector3(
                currentDrink.transform.localPosition.x,
                Mathf.Lerp(currentY, targetY, timeElapsed / fillDuration),
                currentDrink.transform.localPosition.z
            );
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Final position
        currentDrink.transform.localPosition = new Vector3(
            currentDrink.transform.localPosition.x,
            targetY,
            currentDrink.transform.localPosition.z
        );

        // Update state
        currentFillPercentage += fillIncrement;
        if (currentFillPercentage >= 1f)
        {
            // Mark cup as filled if we have juice components
            IceTeaCup cup = currentDrink.transform.parent.GetComponent<IceTeaCup>();
            if (cup != null)
            {
                cup.AddJuice(hasOrange, hasLemon);
            }
            ResetJuicer();
        }
        else
        {
            isFilling = false;
        }
    }

    private void ResetJuicer()
    {
        currentDrink = null;
        hasOrange = false;
        hasLemon = false;
        isFilling = false;
        currentFillPercentage = 0f;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("IceCup") && !isFilling)
        {
            currentDrink = null;
            currentFillPercentage = 0f;
        }
    }
}