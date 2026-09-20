using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

using UnityEngine;

public class SortingGame : MonoBehaviour
{
    public Transform[] binPositions; // All possible positions for bins
    public GameObject[] nextWaveBins; // Bins behind the camera for the next wave
    public GameObject[] trashItems; // Declare the array for trash items
    public float waveDuration = 3f;
    public TMP_Text timerText; // Use TMP_Text for TextMeshPro text
    public TMP_Text waveText; // Add TMP_Text for displaying wave number UI
    public GameObject noneBin; // "None" bin
    public Vector3 spawnPosition;
    public GameObject ui;
    private float timeLeft;
    private int activeBinCount = 2; // Start with 2 bins active
    private int maxBins = 5;
    private int waveNumber = 1;
    private List<GameObject> spawnedTrash = new List<GameObject>();
    private Vector3[] originalPositions; // To store the original positions of the bins

    void Start()
    {
        ui.SetActive(false);
        timeLeft = waveDuration;
        originalPositions = new Vector3[nextWaveBins.Length];

        // Store the original positions of the bins
        for (int i = 0; i < nextWaveBins.Length; i++)
        {
            originalPositions[i] = nextWaveBins[i].transform.position;
        }

        StartCoroutine(WaveManager());
        GameController.Instance.ResetScore(); // Reset the score when the game starts

        UpdateWaveText();
    }

    void Update()
    {
        timeLeft -= Time.deltaTime;
        timerText.text = timeLeft.ToString("F1");

        if (timeLeft <= 0)
        {
            if (waveNumber >= 7)
            {
                timeLeft = 0;
                waveText.text = "Game Over!"; // Update UI

                // Save the final score to GameController for use in the City scene
                GameController.Instance.AddCoins(GameController.Instance.score);

                // Load the "City" scene after a small delay
                StartCoroutine(LoadCitySceneAfterDelay(4f));
                enabled = false; // Stop further Update() processing
            }


            timeLeft = waveDuration;
            waveNumber++;
            SetRandomActiveBins(); // Randomize bin count based on wave number
            RemoveUnsortedTrash();
            UpdateWaveText(); // Update wave number UI when wave changes

            // Move next wave bins to active locations in front of the camera
            MoveNextWaveBinsToMainBins();
        }
    }
    IEnumerator LoadCitySceneAfterDelay(float delay)
    {
        ui.SetActive(true);
        yield return new WaitForSeconds(delay);
        UnityEngine.SceneManagement.SceneManager.LoadScene("City");
    }

    IEnumerator WaveManager()
    {
        while (waveNumber <= 7) // Only run waves until wave 7
        {
            SpawnTrashItem();
            yield return new WaitForSeconds(waveDuration);
        }
    }

    void SetRandomActiveBins()
    {
        // Randomize the number of bins between 2 and 3 initially, then increase max as waves progress
        if (waveNumber > 1 && waveNumber <= 5)
        {
            activeBinCount = Random.Range(2, 4); // 2 to 3 bins for the first few waves
        }
        else if (waveNumber > 5)
        {
            activeBinCount = Random.Range(2, 6); // 2 to 5 bins later on
        }

        // Shuffle the next wave bins
        ShuffleNextWaveBins();
    }

    void ShuffleNextWaveBins()
    {
        // Shuffle nextWaveBins while they're hidden behind the camera
        List<GameObject> shuffledBins = new List<GameObject>(nextWaveBins);
        shuffledBins.Shuffle();

        // After shuffling, we move bins to random locations
        MoveNextWaveBinsToMainBins(shuffledBins);
    }

    void MoveNextWaveBinsToMainBins(List<GameObject> shuffledBins = null)
    {
        if (shuffledBins == null)
        {
            shuffledBins = new List<GameObject>(nextWaveBins);
        }

        // Select random positions for the active bins
        List<Transform> availablePositions = new List<Transform>(binPositions);
        availablePositions.Shuffle();

        // Track used positions to avoid overlap
        HashSet<Transform> usedPositions = new HashSet<Transform>();

        for (int i = 0; i < activeBinCount; i++)
        {
            // Find a free position from available positions
            Transform targetPosition = null;
            foreach (var pos in availablePositions)
            {
                if (!usedPositions.Contains(pos)) // Check if this position is already used
                {
                    targetPosition = pos;
                    usedPositions.Add(pos);
                    break;
                }
            }

            if (targetPosition != null)
            {
                // Place the bin from shuffledBins into the selected position
                GameObject bin = shuffledBins[i];
                bin.transform.position = targetPosition.position; // Move the bin into place
            }
        }

        // Move remaining bins back behind the camera if they're not enabled this wave
        for (int i = activeBinCount; i < shuffledBins.Count; i++)
        {
            shuffledBins[i].transform.position = originalPositions[i]; // Move back to original position behind the camera
        }
    }

    void RemoveUnsortedTrash()
    {
        foreach (GameObject trash in new List<GameObject>(spawnedTrash))
        {
            if (trash != null)
            {
                Destroy(trash);
                GameController.Instance.DeductScore(5); // Subtract score for unsorted trash
            }
        }
        spawnedTrash.Clear();
    }

    void SpawnTrashItem()
    {
        if (trashItems.Length > 0)
        {
            // Randomly choose a trash item from the array and spawn it at the spawn position
            GameObject newTrash = Instantiate(trashItems[Random.Range(0, trashItems.Length)], spawnPosition, Quaternion.identity);
            spawnedTrash.Add(newTrash);
        }
    }

    void UpdateWaveText()
    {
        waveText.text = "Wave: " + waveNumber.ToString();
    }
}

public static class ListExtensions
{
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
