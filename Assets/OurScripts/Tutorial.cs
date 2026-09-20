using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    public GameObject tutorialUI2;
    public GameObject tutorialUI;
    public Transform player;
    public float rotationDuration = 2f;

    [Header("Map Settings")]
    public GameObject mapUI;                  // Assign your map panel in the Inspector
    
    public Vector2 homeWorldPos;             // World position of home
    public Vector2 cafeWorldPos;             // World position of cafe
    public Rect mapBounds;                   // Bounds of the area shown in the map (in world units)
    public RectTransform mapArea;            // UI map area that icons move within

    void Start()
    {
        CheckTutorialStatus();
        StartCoroutine(RotatePlayer180());
    }
    public void OnShopClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("shop");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMap();
        }

        
    }

    void CheckTutorialStatus()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataReceived, OnDataError);
    }

    void OnDataReceived(GetUserDataResult result)
    {
        if (result.Data == null || !result.Data.ContainsKey("TutorialDone"))
        {
            ShowTutorial();
        }
    }

    void OnDataError(PlayFabError error)
    {
        ShowTutorial();
    }

    void ShowTutorial()
    {
        Time.timeScale = 0f;
        tutorialUI.SetActive(true);
    }

    public void OnTutorialOkClicked()
    {
        tutorialUI.SetActive(false);
        Time.timeScale = 1f;

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string> { { "TutorialDone", "1" } }
        };

        PlayFabClientAPI.UpdateUserData(request,
            result => Debug.Log("TutorialDone saved."),
            error => Debug.LogError("Failed to save TutorialDone: "  )
        );
    }

    public void OnTutorialOkClicked2()
    {
        tutorialUI2.SetActive(false);
        Time.timeScale = 1f;

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string> { { "TutorialDone", "1" } }
        };

        PlayFabClientAPI.UpdateUserData(request,
            result => Debug.Log("TutorialDone saved."),
            error => Debug.LogError("Failed to save TutorialDone: "  )
        );
    }

    IEnumerator RotatePlayer180()
    {
        if (player == null)
            yield break;

        Quaternion startRot = player.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, 180, 0);
        float elapsed = 0f;

        while (elapsed < rotationDuration)
        {
            player.rotation = Quaternion.Slerp(startRot, endRot, elapsed / rotationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        player.rotation = endRot;
    }

    void ToggleMap()
    {
        bool newState = !mapUI.activeSelf;
        mapUI.SetActive(newState);
        Time.timeScale = 1f;

        if (newState)
        {
            Time.timeScale = 0f;
        }
    }


}
