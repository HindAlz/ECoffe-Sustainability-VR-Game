using UnityEngine;

public class IceTeaCup : MonoBehaviour
{
    private bool hasWater = false;
    private bool hasTeaBag = false;
    private bool hasIce = false;
    public bool hasJuice = false;

    public GameObject waterObject;  // Assign water mesh
    public GameObject teaBagObject; // Assign teabag object
    public GameObject ice; // Assign ice object
    public GameObject orangeSlice; // Assign orange slice object
    public GameObject lemonSlice; // Assign lemon slice object
    public Material brownWaterMaterial; // Assign brown material for red tea
    public Material greenWaterMaterial; // Assign green material for green tea
    public Material orange; // Assign orange material for orange juice
    public Material lemon; // Assign lemon material for lemon juice

    public bool isRed; // Indicates if the tea is red
    public bool isOrange; // Indicates if the juice is orange
    public bool isLemon; // Indicates if the juice is lemon

    private Renderer waterRenderer;

    void Start()
    {
        // Initialize the cup as empty
        waterObject.SetActive(false);
        teaBagObject.SetActive(false);
        orangeSlice.SetActive(false);
        lemonSlice.SetActive(false);
        ice.SetActive(false);

        // Get the renderer component of the water object
        waterRenderer = waterObject.GetComponent<Renderer>();
    }
    public void ResetCup()
    {
        // Reset all state variables
        hasWater = false;
        hasTeaBag = false;
        hasIce = false;
        hasJuice = false;
        isRed = false;
        isOrange = false;
        isLemon = false;

        // Deactivate all visual components
        waterObject.SetActive(false);
        teaBagObject.SetActive(false);
        orangeSlice.SetActive(false);
        lemonSlice.SetActive(false);
        ice.SetActive(false);

        // Reset the water material to its default (if needed)
        if (waterRenderer != null)
        {
            waterRenderer.material = null; // Or set to a default material if you have one
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
        else if (other.CompareTag("OrangeJuice"))
        {
            AddJuice(true, false); // Add orange juice
        
            AddJuice(false, true); // Add lemon juice
        }
    }

    public void AddWater()
    {
        if (!hasWater && !hasJuice) // Only add water if there's no juice
        {
            hasWater = true;
            waterObject.SetActive(true);

            // Change water color if tea bag is already added
            if (hasTeaBag)
            {
                if (isRed)
                    ChangeWaterColorRed();
                else
                    ChangeWaterColorGreen();
            }
        }
    }

    public void AddJuice(bool isOrangeJuice, bool isLemonJuice)
    {
        if (hasJuice || hasTeaBag || hasWater) // Prevent adding juice if tea or water is already present
        {
            Debug.LogWarning("Cannot add juice to tea or water.");
            return;
        }

        hasJuice = true;
        isOrange = isOrangeJuice;
        isLemon = isLemonJuice;
        waterObject.SetActive(true);

        // Change water color and activate the correct slice based on juice type
        if (waterRenderer != null)
        {
            if (isOrange)
            {
                waterRenderer.material = orange;
                orangeSlice.SetActive(true);
                lemonSlice.SetActive(false); // Ensure lemon slice is deactivated
            }
            else if (isLemon)
            {
                waterRenderer.material = lemon;
                lemonSlice.SetActive(true);
                orangeSlice.SetActive(false); // Ensure orange slice is deactivated
            }
        }
    }

    public void AddRedTeaBag()
    {
        if (!hasTeaBag && !hasJuice) // Only add tea bag if there's no juice
        {
            hasTeaBag = true;
            isRed = true;
            teaBagObject.SetActive(true);

            // Change water color if water is already added
            if (hasWater)
            {
                ChangeWaterColorRed();
            }
        }
    }

    public void AddGreenTeaBag()
    {
        if (!hasTeaBag && !hasJuice) // Only add tea bag if there's no juice
        {
            hasTeaBag = true;
            isRed = false;
            teaBagObject.SetActive(true);

            // Change water color if water is already added
            if (hasWater)
            {
                ChangeWaterColorGreen();
            }
        }
    }

    public void AddIce()
    {
        if (!hasIce)
        {
            hasIce = true;
            ice.SetActive(true);
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
    public bool isTea()
    {
        return hasWater || hasTeaBag || hasIce;
    }
}