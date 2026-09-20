using UnityEngine;

public class Egg : MonoBehaviour
{
    public bool isRaw;
    public bool isCracked;
    public bool isCooked;
    public GameObject rawEgg; // Assign the cracked egg GameObject (visual)
    public GameObject crackedEgg; // Assign the cracked egg GameObject (visual)
    public GameObject cookedEgg;  // Assign the cooked egg GameObject (visual)

    private void Start()
    {
        // Ensure the egg starts as raw
        isRaw = true;
        crackedEgg.SetActive(false);
        cookedEgg.SetActive(false);
    }

    // Call this method to crack the egg
    public void CrackEgg()
    {
        if (isRaw)
        {
            isCracked = true;
            isRaw = false;
            crackedEgg.SetActive(true); // Show the cracked egg visual
            rawEgg.SetActive(false); // Show the cracked egg visual

        }
    }

    // Call this method to cook the egg
    public void CookEgg()
    {
        if (isCracked)
        {
            isCooked = true;
            isCracked = false;
            crackedEgg.SetActive(false); // Hide the cracked egg visual
            cookedEgg.SetActive(true);   // Show the cooked egg visual
        }
    }

}