using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class DayStartButton : MonoBehaviour
{
    [Header("UI References")]
    public GameObject uiPanel;    // Assign your UI Panel here
    public TMP_Text scoreText;    // Assign your TMP Text here
    public Material daySkybox;    // Assign your Day skybox material here

    private int todayScore = 0;
    private int dayCount = 1; // Default if missing
    private int newTotalCoins = 0; // To display after adding todayScore

    public void OnButtonClick()
    {
        uiPanel.SetActive(true);

        // Set skybox
        if (daySkybox != null)
        {
            RenderSettings.skybox = daySkybox;
        }

        // Optionally set "Loading..." while fetching
        if (scoreText != null)
        {
            scoreText.text = "Loading...";
        }

        // Fetch PlayFab user data
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataReceived, OnDataError);
    }

    private void OnDataReceived(GetUserDataResult result)
    {
        bool needsTutorialUpdate = false;
        var data = result.Data;

        // todayScore
        if (data != null && data.ContainsKey("todayScore"))
        {
            int.TryParse(data["todayScore"].Value, out todayScore);
        }
        else
        {
            todayScore = 0;
        }

        // DayCount
        if (data != null && data.ContainsKey("DayCount"))
        {
            int.TryParse(data["DayCount"].Value, out dayCount);
            dayCount++; // Increment
        }
        else
        {
            dayCount = 1; // If missing
            needsTutorialUpdate = true;
        }

        // TutorialDone
        if (data == null || !data.ContainsKey("TutorialDone"))
        {
            needsTutorialUpdate = true;
        }

        // Update todayScore to Currency
        if (todayScore > 0)
        {
            AddCoins(todayScore, needsTutorialUpdate);
        }
        else
        {
            // No coins to add, still update data
            UpdateUserData(needsTutorialUpdate);
        }
    }

    private void AddCoins(int amountToAdd, bool needsTutorialUpdate)
    {
        var addRequest = new AddUserVirtualCurrencyRequest
        {
            VirtualCurrency = "CO",   // Currency code
            Amount = amountToAdd
        };

        PlayFabClientAPI.AddUserVirtualCurrency(addRequest,
            result =>
            {
                newTotalCoins = result.Balance;

                // Update UI after adding coins
                if (scoreText != null)
                {
                    scoreText.text = $"Day: {dayCount}\r\n\r\n                     {todayScore}\r\n\r\n\r\n                       {newTotalCoins}";
                }

                // After adding, reset todayScore and update DayCount
                UpdateUserData(needsTutorialUpdate);
            },
            error =>
            {
                Debug.LogError("Error adding virtual currency: "  );
            });
    }

    private void UpdateUserData(bool needsTutorialUpdate)
    {
        var updateRequest = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "todayScore", "0" },                // Reset todayScore
                { "DayCount", dayCount.ToString() }   // Save incremented DayCount
            }
        };

        if (needsTutorialUpdate)
        {
            updateRequest.Data.Add("TutorialDone", "1");
        }

        PlayFabClientAPI.UpdateUserData(updateRequest,
            result => Debug.Log("User data updated successfully."),
            error => Debug.LogError("Error updating user data: "  ));
    }

    private void OnDataError(PlayFabError error)
    {
        Debug.LogError("Error fetching user data: "  );
    }
    public void OnReturnButtonClick()
    {
        uiPanel.SetActive(false);
    }
}
