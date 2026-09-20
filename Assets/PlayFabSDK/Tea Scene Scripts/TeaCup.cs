using UnityEngine;

public class TeaCup : MonoBehaviour
{
    private bool hasWater = false;
    private bool hasTeaBag = false;

    public GameObject waterObject;  // Assign water mesh
    public GameObject teaBagObject; // Assign teabag object
    public Material brownWaterMaterial; // Assign brown material
    public Material greenWaterMaterial; // Assign brown material
    public bool isRed;

    private Renderer waterRenderer;

    void Start()
    {
        waterObject.SetActive(false);
        teaBagObject.SetActive(false);
        waterRenderer = waterObject.GetComponent<Renderer>();
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

    }

    public void AddWater()
    {
        if (!hasWater)
        {
            hasWater = true;
            waterObject.SetActive(true);

            if (hasTeaBag && isRed)
            {
                ChangeWaterColorRed();
            }else if (hasTeaBag && !isRed)
            {
                ChangeWaterColorGreen();
            }
        }
    }

    public void AddRedTeaBag()
    {
        if (!hasTeaBag)
        {
            hasTeaBag = true;
            isRed = true;
            teaBagObject.SetActive(true);

            if (hasWater)
            {
                ChangeWaterColorRed();
            }
        }
    }
    public void AddGreenTeaBag()
    {
        if (!hasTeaBag)
        {
            hasTeaBag = true;
            isRed = false;
            teaBagObject.SetActive(true);

            if (hasWater)
            {
                ChangeWaterColorGreen();
            }
        }
    }

    void ChangeWaterColorRed()
    {
        if (waterRenderer != null && brownWaterMaterial != null)
        {
            waterRenderer.material = brownWaterMaterial; // Assign brown material directly
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
            waterRenderer.material = greenWaterMaterial; // Assign brown material directly
        }
        else
        {
            Debug.LogWarning("Water Renderer or Brown Water Material is not assigned.");
        }
    }
}
