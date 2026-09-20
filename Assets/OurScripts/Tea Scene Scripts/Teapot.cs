using UnityEngine;
using System.Collections;

public class Teapot : MonoBehaviour
{
    private GameObject drink;
    private bool isCupInHolder = false;
    private bool isDrinkFull = false;
    private float currentFillPercentage = 0f;

    public Material waterMaterial;

    public float[] basePositionY;
    public float[] maxPositionY;
    public float fillIncrement = 0.25f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EspressoCup") || other.CompareTag("Mug") || other.CompareTag("ColdDrinks"))
        {
            Transform drinkTransform = other.transform.Find("Drink");
            if (drinkTransform != null)
            {
                drink = drinkTransform.gameObject;
                isCupInHolder = true;
                isDrinkFull = false;
                currentFillPercentage = 0f;

                FillCup(other.tag);
            }
        }
    }

    private void FillCup(string cupType)
    {
        if (isCupInHolder && drink != null && !isDrinkFull)
        {
            DrinkTrack tracker = drink.transform.parent.GetComponent<DrinkTrack>();
            if (tracker != null)
            {
                tracker.AddDrink("Water");

                Renderer drinkRenderer = drink.GetComponent<Renderer>();
                if (drinkRenderer != null && waterMaterial != null)
                {
                    drinkRenderer.material = waterMaterial;
                }
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
        float fillStep = (maxY - baseY) * fillIncrement;
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
        currentFillPercentage += fillIncrement;
        if (currentFillPercentage >= 1f) isDrinkFull = true;
    }

  

    private int GetCupTypeIndex(string cupType)
    {
        switch (cupType)
        {
            case "EspressoCup": return 0;
            case "Mug": return 1;
            case "ColdDrinks": return 2;
            default: return -1;
        }
    }
}
