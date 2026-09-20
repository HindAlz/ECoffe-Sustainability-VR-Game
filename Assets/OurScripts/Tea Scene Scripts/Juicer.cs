using UnityEngine;

public class Juicer : MonoBehaviour
{
    public bool hasJuice = false; // Tracks if the juicer currently has juice
    public bool oj; // Tracks if the juice is orange
    public bool lem; // Tracks if the juice is lemon

    public GameObject Juice;  // Assign the juice mesh
    public Material orange; // Assign the orange juice material
    public Material lemon; // Assign the lemon juice material

    private Renderer waterRenderer;

    private GameObject currentFruit; // Tracks the current fruit being juiced

    void Start()
    {
        Juice.SetActive(false); // Ensure the juice is initially hidden
        waterRenderer = Juice.GetComponent<Renderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Orange") && !hasJuice) // Only juice if no juice is currently in the juicer
        {
            Destroy(other.gameObject); // Destroy the fruit after juicing
            AddOJuice();
        }
        else if (other.CompareTag("Lemon") && !hasJuice) // Only juice if no juice is currently in the juicer
        {
            Destroy(other.gameObject); // Destroy the fruit after juicing
            AddLJuice();
        }
        else if (other.CompareTag("IceCup"))
        {
            IceTeaCup temp = other.GetComponent<IceTeaCup>();

            if (!temp.hasJuice && !temp.isTea())
            {
                if (oj) temp.isOrange = true;
                else if (lem) temp.isLemon = true;
                temp.AddJuice(temp.isOrange, temp.isLemon);
                RemoveJuice();
            }
            
        }
    }

    public void AddOJuice()
    {
        hasJuice = true;
            oj = true;
            lem = false;
            Juice.SetActive(true);
            waterRenderer.material = orange;
            
        
    }

    public void AddLJuice()
    {
            hasJuice = true;
            lem = true;
            oj = false;
            Juice.SetActive(true);
            waterRenderer.material = lemon;
            
        
    }

    public void RemoveJuice()
    {
        if (hasJuice)
        {
            hasJuice = false;
            Juice.SetActive(false);
            lem = false;
            oj = false;

        }
    }
}