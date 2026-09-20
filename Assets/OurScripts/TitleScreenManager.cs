using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class TitleScreenManager : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject controlScreen;

    public void ShowControlsScreen()
    {
        titleScreen.SetActive(false);
        controlScreen.SetActive(true);
    }

    public void OpenShop()
    {
        SceneManager.LoadScene("Shop");
    }

    public void BackToTitle()
    {
        controlScreen.SetActive(false);
        titleScreen.SetActive(true);
    }

    
    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void eraseSave()
    {
        SaveSystem.Erase();
    }
    
}
