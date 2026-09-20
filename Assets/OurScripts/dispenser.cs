using UnityEngine;

public class dispenser : MonoBehaviour
{
    public Vector3 spawnPoint; // Location where cups should spawn

    // Public method to dispense a specific cup type
    public void DispenseCup(GameObject cupPrefab)
    {
        if (cupPrefab == null)
        {
            Debug.LogWarning("Cup prefab is not assigned!");
            return;
        }

        // Check if a cup of this type already exists
        string cupName = cupPrefab.name;
        if (GameObject.Find(cupName) == null) // Look for a GameObject with this cup's name
        {
            GameObject newCup = Instantiate(cupPrefab, spawnPoint, Quaternion.identity);
            newCup.name = cupName; // Ensure the spawned cup retains its type name
            Debug.Log("Dispensed cup: " + cupName);
        }
        else
        {
            Debug.Log("A cup of type '" + cupName + "' already exists.");
        }
    }

  
}
