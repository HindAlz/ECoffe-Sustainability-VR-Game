using TMPro; // <-- You need this for TextMeshPro
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance; // Singleton instance

    public int score = 0;
    public TMP_Text scoreT; // <-- Fixed: TMP_Text, not textMeshPro

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keep GameController across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates
        }
    }

    // Method to add points to the score
    public void AddScore(int points)
    {
        score += points;
        UpdateScoreDisplay();
    }
    public void AddCoins(int amount)
    {
        score += amount;
    }

    // Method to subtract points from the score
    public void DeductScore(int points)
    {
        score -= points;
        UpdateScoreDisplay();
    }

    // Update the score UI
    void UpdateScoreDisplay()
    {
        if (scoreT != null)
        {
            scoreT.text = "Score: " + score;
        }
    }

    // Reset the score
    public void ResetScore()
    {
        score = 0;
        UpdateScoreDisplay();
    }

    public int GetScore()
    {
        return score;
    }
}
