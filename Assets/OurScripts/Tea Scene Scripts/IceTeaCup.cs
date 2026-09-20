using UnityEngine;

public class IceTeaCup : MonoBehaviour
{
    private bool hasWater = false;
    private bool hasTeaBag = false;
    private bool hasIce = false;
    public bool hasJuice = false;

    public GameObject teaBagObject;
    public GameObject ice;
    public GameObject orangeSlice;
    public GameObject lemonSlice;

    public Material brownWaterMaterial; // Red tea material
    public Material greenWaterMaterial; // Green tea material
    public Material orange; // Orange juice material
    public Material lemon; // Lemon juice material

    public bool isGreen;
    public bool isRed;
    public bool isOrange;
    public bool isLemon;

    public Transform drinkObject; // Drink child object reference
    private Renderer drinkRenderer; // Renderer for the Drink object
    private DrinkTrack drinkTracker; // Reference to DrinkTrack

    void Start()
    {
        teaBagObject.SetActive(false);
        orangeSlice.SetActive(false);
        lemonSlice.SetActive(false);
        ice.SetActive(false);

        if (drinkObject != null)
        {
            drinkRenderer = drinkObject.GetComponent<Renderer>();
        }
        else
        {
            Debug.LogWarning("Drink object is not assigned on " + gameObject.name);
        }

        drinkTracker = GetComponent<DrinkTrack>();
        if (drinkTracker == null)
        {
            Debug.LogWarning("DrinkTrack script is missing from " + gameObject.name);
        }
    }

    public void ResetCup()
    {
        hasWater = false;
        hasTeaBag = false;
        hasIce = false;
        hasJuice = false;
        isRed = false;
        isOrange = false;
        isLemon = false;

        teaBagObject.SetActive(false);
        orangeSlice.SetActive(false);
        lemonSlice.SetActive(false);
        ice.SetActive(false);

        if (drinkRenderer != null)
        {
            drinkRenderer.material = null;
        }

        Debug.Log("Cup has been reset.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Teapot"))
        {
            AddWater();
        }
        else if (other.CompareTag("RedTeaBox"))
        {
            AddRedTeaBag();
        }
        else if (other.CompareTag("GreenTeaBox"))
        {
            AddGreenTeaBag();
        }
        else if (other.CompareTag("Ice"))
        {
            AddIce();
        }
        else if (other.CompareTag("Juicer"))
        {
            Juicer juicerObj = other.GetComponent<Juicer>();
            if (juicerObj != null)
            {
                if (juicerObj.oj)
                    AddJuice(true, false);
                else if (juicerObj.lem)
                    AddJuice(false, true);
            }
        }
        else if (other.CompareTag("PStraw"))
        {
            EnableStraw("Straw"); // plastic straw
        }
        else if (other.CompareTag("SStraw"))
        {
            EnableStraw("Sustainable Straw"); // sustainable straw
        }
    }

    private void EnableStraw(string strawName)
    {
        Transform straw = transform.Find(strawName);
        if (straw != null)
        {
            straw.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Straw not found: " + strawName);
        }
    }

    public void AddWater()
    {
        if (!hasWater && !hasJuice)
        {
            hasWater = true;
            if (drinkRenderer != null)
            {
                drinkRenderer.material = brownWaterMaterial; // Default to brown
                if (hasTeaBag)
                {
                    if (isRed) ChangeWaterColorRed();
                    else ChangeWaterColorGreen();
                }
            }
            UpdateDrinkList();
        }
    }

    public void AddJuice(bool isOrangeJuice, bool isLemonJuice)
    {
        if (hasJuice || hasTeaBag || hasWater)
        {
            Debug.LogWarning("Cannot add juice to tea or water.");
            return;
        }

        hasJuice = true;
        isOrange = isOrangeJuice;
        isLemon = isLemonJuice;

        if (drinkRenderer != null)
        {
            // Handle material change based on juice type
             if (isOrange)
            {
                drinkRenderer.material = orange;
                orangeSlice.SetActive(true);
                lemonSlice.SetActive(false);
            }
            else if (isLemon)
            {
                drinkRenderer.material = lemon;
                lemonSlice.SetActive(true);
                orangeSlice.SetActive(false);
            }
        }

        UpdateDrinkList();
    }

    public void AddRedTeaBag()
    {
        if (!hasTeaBag && !hasJuice)
        {
            hasTeaBag = true;
            isRed = true;
            teaBagObject.SetActive(true);

            if (hasWater)
            {
                ChangeWaterColorRed();
            }

            UpdateDrinkList();
        }
    }

    public void AddGreenTeaBag()
    {
        if (!hasTeaBag && !hasJuice)
        {
            hasTeaBag = true;
            isGreen = true;
            isRed = false;
            teaBagObject.SetActive(true);

            if (hasWater)
            {
                ChangeWaterColorGreen();
            }

            UpdateDrinkList();
        }
    }

    public void AddIce()
    {
        if (!hasIce)
        {
            hasIce = true;
            ice.SetActive(true);
            UpdateDrinkList();
        }
    }

    private void ChangeWaterColorRed()
    {
        if (drinkRenderer != null && brownWaterMaterial != null)
        {
            drinkRenderer.material = brownWaterMaterial;
        }
        else
        {
            Debug.LogWarning("Drink Renderer or Brown Water Material is not assigned.");
        }
    }

    private void ChangeWaterColorGreen()
    {
        if (drinkRenderer != null && greenWaterMaterial != null)
        {
            drinkRenderer.material = greenWaterMaterial;
        }
        else
        {
            Debug.LogWarning("Drink Renderer or Green Water Material is not assigned.");
        }
    }

    public bool isTea()
    {
        return hasWater || hasTeaBag || hasIce;
    }

    private void UpdateDrinkList()
    {
        if (drinkTracker == null) return;

        if (hasWater)
        {
            drinkTracker.AddDrink("Water");
        }

        if (hasWater && isRed)
        {
            drinkTracker.AddDrink("Red Tea");
        }

        if (hasWater && isGreen)
        {
            drinkTracker.AddDrink("Green Tea");
        }

        if (hasIce)
        {
            drinkTracker.AddDrink("Ice");
        }

        if (hasJuice)
        {
            if (isOrange)
            {
                drinkTracker.AddDrink("Orange Juice");
            }
            else if (isLemon)
            {
                drinkTracker.AddDrink("Lemon Juice");
            }
        }
    }
}
