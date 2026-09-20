using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class MapHintTrigger : MonoBehaviour
{
    public GameObject mapHintUI;   // Assign the UI panel in Inspector
    public Button okButton;        // Assign the OK button in Inspector
    private Collider myCollider;
    private bool tutorialDone = false;

    void Start()
    {
        myCollider = GetComponent<Collider>();
        if (mapHintUI != null)
            mapHintUI.SetActive(false);

        if (okButton != null)
            okButton.onClick.AddListener(OnOkClicked);

        // Check PlayFab data
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnUserDataReceived, OnUserDataError);
    }

    void OnUserDataReceived(GetUserDataResult result)
    {
        tutorialDone = result.Data != null && result.Data.ContainsKey("TutorialDone");

        // Disable this collider if tutorial is already done
        if (tutorialDone && myCollider != null)
        {
            myCollider.enabled = false;
        }
    }

    void OnUserDataError(PlayFabError error)
    {
        Debug.LogWarning("Could not fetch PlayFab data: "  );
        // Assume tutorial not done, keep collider active
    }

    void OnTriggerEnter(Collider other)
    {
        if (!tutorialDone && other.CompareTag("Player") && mapHintUI != null)
        {
            ShowHint();
        }
    }

    void ShowHint()
    {
        Time.timeScale = 0f;
        mapHintUI.SetActive(true);

        // Disable collider after showing
        if (myCollider != null)
            myCollider.enabled = false;
    }

    void OnOkClicked()
    {
        if (mapHintUI != null)
            mapHintUI.SetActive(false);

        Time.timeScale = 1f;
    }
}
