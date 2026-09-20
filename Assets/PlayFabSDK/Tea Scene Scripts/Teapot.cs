using UnityEngine;
using System.Collections;

public class Teapot : MonoBehaviour
{
    private GameObject drink;
    private bool isDrinkFull = false;
    private float currentFillPercentage = 0f;

    public Material teapotMaterial;  // The material for the teapot
    public float basePositionY;     // Base position for IceCup
    public float maxPositionY;      // Max position for IceCup
    public float fillIncrement = 0.25f;  // Amount of rise per interaction (25%)

    public AudioClip pourSound;  // The pouring sound clip
    private AudioSource audioSource;  // AudioSource component

    private void Start()
    {
        // Ensure the AudioSource is attached to the same GameObject
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("IceCup"))  // Only handle IceCup tag
        {
            Transform drinkTransform = other.transform.Find("Drink");  // Finding the 'drink' child of the IceCup
            if (drinkTransform != null)
            {
                drink = drinkTransform.gameObject;
                isDrinkFull = false;
                currentFillPercentage = 0f;

                StartFillingCup();
            }
        }
    }

    private void StartFillingCup()
    {
        if (drink != null && !isDrinkFull)
        {
            // Set material for the drink (though it's the same for all cases)
            Renderer drinkRenderer = drink.GetComponent<Renderer>();
            if (drinkRenderer != null && teapotMaterial != null)
            {
                drinkRenderer.material = teapotMaterial;
            }

            // Play the pouring sound at the start of the filling process
            if (audioSource != null && pourSound != null)
            {
                audioSource.PlayOneShot(pourSound);  // Play the pouring sound
            }

            // Access the DrinkTrack component on the parent of the drink object
         

            StartCoroutine(FillCupCoroutine());
        }
    }

    private IEnumerator FillCupCoroutine()
    {
        float currentY = drink.transform.localPosition.y;
        float fillStep = (maxPositionY - basePositionY) * fillIncrement;  // 25% of the height range
        float targetY = Mathf.Min(currentY + fillStep, maxPositionY);

        float timeElapsed = 0f;
        float fillDuration = 0.5f;  // Duration for each fill step

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
        if (currentFillPercentage >= 1f) isDrinkFull = true;
    }
}
