using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Trashmanager : MonoBehaviour
{
    [Header("Gameplay")]
    public int score = 0;
    public int trashCount = 0;
    public int trashTarget = 5;
    public TextMeshProUGUI scoreText;
    [Header("Trash Spawning")]
    public GameObject[] trashPrefabs;
    public Transform spawnPoint;
    private bool canSpawn = false;
    public TextMeshProUGUI reviewText;
    public TextMeshProUGUI title;

    [Header("UI")]
    public GameObject tutorialUI;
    public GameObject choicePanel; // This panel should contain the two buttons
    public Button sceneAButton;
    public Button sceneBButton;
    public string sceneAName;
    public string sceneBName;

    void Start()
    {
        StartCoroutine(ShowTutorialAndStart());


        if (choicePanel != null)
            choicePanel.SetActive(false);

        if (sceneAButton != null)
            sceneAButton.onClick.AddListener(() => LoadScene(sceneAName));

        if (sceneBButton != null)
            sceneBButton.onClick.AddListener(() => LoadScene(sceneBName));
    }

    IEnumerator ShowTutorialAndStart()
    {
        yield return new WaitForSeconds(5f);

        if (tutorialUI != null)
            tutorialUI.SetActive(false);

        canSpawn = true;
    }

    void Update()
    {
        scoreText.text = "Bonus: " + score;

        bool trashExists = false;
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.name.ToLower().Contains("trash"))
            {
                trashExists = true;
                break;
            }
        }

        if (canSpawn && !trashExists && trashCount < trashTarget)
        {
            SpawnTrash();
        }
    }
    public void SetEndOfDayReview()
    {
        if (reviewText != null)
        {
            // Load saved data
            PlayerData data = SaveSystem.Load();
            if (data == null)
            {
                data = new PlayerData(); // create default if none
            }

            int previousTotal = data.dayScore;
            int bonus = score;
            int updatedTotal = data.Coins + bonus;

            // Update text and save
            title.text = $"Day {data.Day} Review";
            reviewText.text = $"Day Score = {data.dayScore}\nBonus = {bonus}\nDay Rating = {data.susRating}\nTotal = {updatedTotal}";
            data.Coins = updatedTotal;
            SaveSystem.Save(data);

        }
        else
        {
            Debug.LogWarning("Review text not assigned.");
        }
    }


    void SpawnTrash()
    {
        if (trashPrefabs.Length == 0 || spawnPoint == null)
        {
            Debug.LogWarning("TrashManager: No trash prefabs or spawn point set.");
            return;
        }

        int index = Random.Range(0, trashPrefabs.Length);
        GameObject selectedPrefab = trashPrefabs[index];

        // Base spawn position
        Vector3 spawnPosition = spawnPoint.position;


        GameObject newTrash = Instantiate(selectedPrefab, spawnPosition, spawnPoint.rotation);
        IncrementTrashCount();
    }



    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log("Score: " + score);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Plastic") || other.CompareTag("Glass") || other.CompareTag("Paper") || other.CompareTag("Cans"))
        {
            Destroy(other.gameObject);

        }
    }
    public void IncrementTrashCount()
    {
        trashCount++;
        Debug.Log("Trash Count: " + trashCount);

        if (trashCount >= trashTarget)
        {
            SetEndOfDayReview();
            ShowChoiceButtons();
        }
    }

    void ShowChoiceButtons()
    {
        if (choicePanel != null)
        {
            choicePanel.SetActive(true);
            canSpawn = false;
        }
    }

    void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("TrashManager: Scene name not set.");
        }
    }
}
