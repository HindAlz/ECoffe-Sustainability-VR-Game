using UnityEngine;
using UnityEngine.UI;

public class replyButton : MonoBehaviour
{
    [Tooltip("Index of the reply option this button represents (0, 1, or 2).")]
    public int replyIndex;

    [Tooltip("Reference to the AngryBehavior script.")]
    private AngryBehavior angryBehavior;

    private Button uiButton;

    void Start()
    {
        uiButton = GetComponent<Button>();
        if (uiButton != null)
        {
            // Try to find the AngryBehavior component on a GameObject with "Angry" in its name
            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.name.Contains("Angry"))
                {
                    angryBehavior = obj.GetComponent<AngryBehavior>();
                    if (angryBehavior != null)
                    {
                        break; // Stop once we find the first AngryBehavior
                    }
                }
            }

            if (angryBehavior == null)
            {
                Debug.LogError("No AngryBehavior component found on any object with 'Angry' in its name.");
            }

            // Add the listener for the button click
            uiButton.onClick.AddListener(HandleReplyButtonClick);
        }
        else
        {
            Debug.LogError("ReplyButtonVR: Button component missing on " + gameObject.name);
        }
    }

    // This method is assigned to the button's onClick event in the Inspector
    public void HandleReplyButtonClick()
    {
        if (angryBehavior != null)
        {
            angryBehavior.OnPlayerChoose(replyIndex);
        }
        else
        {
            Debug.LogWarning("AngryBehavior not found for button on " + gameObject.name);
        }
    }
}
