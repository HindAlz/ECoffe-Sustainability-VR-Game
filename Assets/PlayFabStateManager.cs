using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class PlayFabStateManager : MonoBehaviour
{
    public GameObject blackScreenPanel; // Assign in Inspector
    public Material nightSkyboxMaterial; // Assign in Inspector
    public Button button; // Assign in Inspector
    public Light directionalLight; // Assign in Inspector

    private bool isChecking = false;

    void Start()
    {
        blackScreenPanel.SetActive(false);
        button.onClick.AddListener(OnButtonClick);
    }

    public void OnButtonClick()
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            Debug.LogWarning("Not logged in. Make sure you are logged in before using this feature.");
            return;
        }

        Debug.Log("Button clicked!");
        UpdatePlayFabState(1); // Set "state" to 1
        blackScreenPanel.SetActive(true); // Show waiting screen

        if (!isChecking)
        {
            StartCoroutine(CheckStateChange());
        }
    }

    private void UpdatePlayFabState(int stateValue)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "state", stateValue.ToString() }
            }
        };

        PlayFabClientAPI.UpdateUserData(request,
            result => Debug.Log("State updated successfully"),
            error => Debug.LogError("Failed to update state: "  ));
    }

    private IEnumerator CheckStateChange()
    {
        isChecking = true;

        while (true)
        {
            yield return new WaitForSeconds(2f); // Check every 2 seconds

            if (!PlayFabClientAPI.IsClientLoggedIn())
            {
                Debug.Log("Still not logged in...");
                continue;
            }

            var request = new GetUserDataRequest();
            PlayFabClientAPI.GetUserData(request, result =>
            {
                if (result.Data != null && result.Data.ContainsKey("state"))
                {
                    int stateValue = int.Parse(result.Data["state"].Value);
                    if (stateValue == 0)
                    {
                        blackScreenPanel.SetActive(false); // Hide waiting screen
                        SceneManager.LoadScene("Sorting");
                        // Set the skybox
                        RenderSettings.skybox = nightSkyboxMaterial;
                        DynamicGI.UpdateEnvironment();

                        // Dim the directional light
                        if (directionalLight != null)
                        {
                            directionalLight.intensity = 0f;
                        }
                        else
                        {
                            Debug.LogWarning("Directional light not assigned!");
                        }

                        isChecking = false;
                    }
                }
            }, error =>
            {
                Debug.LogError("Failed to get state: "  );
            });

            if (!isChecking) yield break;
        }
    }
}
