using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseM: MonoBehaviour
{
    public GameObject pauseMenuUI;    // Reference to the pause menu UI
    public Button mainMenuButton;     // Main Menu button
    public Button quitButton;         // Quit button

    private bool isPaused = false;

    void Start()
    {
        // Ensure the pause menu is hidden at the start
        pauseMenuUI.SetActive(false);

        // Add listeners for buttons
        mainMenuButton.onClick.AddListener(GoToMainMenu);
        quitButton.onClick.AddListener(QuitGame);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;  // Stop the game time
        pauseMenuUI.SetActive(true);  // Show the pause menu
    }

    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;  // Resume the game time
        pauseMenuUI.SetActive(false);  // Hide the pause menu
    }

    void GoToMainMenu()
    {
        // Load the TitleScreen scene (Main Menu)
        SceneManager.LoadScene("TitleScreen");
    }

    void QuitGame()
    {
        // Quit the game (Works in the Editor and Build)
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
