using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SustainableManager : MonoBehaviour
{
    public List<Vector3> spawnLocations; // Assign spawn points in Inspector
    public List<GameObject> itemPrefabs; // Assign item prefabs in Inspector

    private void Start()
    {
        SpawnItemsForCustomer();
    }

    public void SpawnItemsForCustomer()
    {
        if (spawnLocations.Count == 0 || itemPrefabs.Count == 0)
        {
            Debug.LogWarning("No spawn locations or items assigned!");
            return;
        }

        // Clean up existing items first
        CleanupExistingItems();

        // Your original shuffle system
        List<Vector3> shuffledLocations = spawnLocations.OrderBy(_ => Random.value).ToList();

        // Spawn items using the shuffled locations
        for (int i = 0; i < itemPrefabs.Count; i++)
        {
            if (i >= shuffledLocations.Count) break;

            Instantiate(itemPrefabs[i], shuffledLocations[i], Quaternion.identity);
        }
    }

    private void CleanupExistingItems()
    {
        // Find all objects in scene that match our prefab names
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            // Check if this object matches any of our prefabs (ignoring Unity's (Clone) suffix)
            foreach (var prefab in itemPrefabs)
            {
                if (obj.name.StartsWith(prefab.name))
                {
                    Destroy(obj);
                    break; // No need to check other prefabs once we found a match
                }
            }
        }
    }
}