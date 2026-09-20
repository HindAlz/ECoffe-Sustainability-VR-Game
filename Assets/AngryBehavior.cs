using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text;

public class AngryBehavior : MonoBehaviour
{
    [Header("API Key")]
    public string openAIApiKey = "key_here";

    [Header("UI References")]
    public Text dialogueText;
    public Button[] replyButtons; // Assign 2 buttons in inspector
    public Text[] replyButtonTexts; // Assign Text components inside each button

    private int correctOptionIndex = 0;
    private int correctAnswers = 0;
    private int round = 0;
    private const int totalRounds = 3;

    private bool isWaitingForReply = false;
    private bool hasSelectedResponse = false;

    [System.Serializable]
    private class Message
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    private class RequestData
    {
        public string model = "gpt-4";
        public Message[] messages;
    }

    [System.Serializable]
    private class Choice
    {
        public Message message;
    }

    [System.Serializable]
    private class ResponseData
    {
        public Choice[] choices;
    }

    private CustomerSystem customerSystem;

    void Start()
    {
        customerSystem = FindObjectOfType<CustomerSystem>();
    }

    public void OnCustomerServed()
    {
        StartCoroutine(StartAngryDialogue());
    }

    private IEnumerator StartAngryDialogue()
    {
        if (customerSystem == null) yield break;

        while (round < totalRounds)
        {
            yield return StartCoroutine(GetChatGPTResponse(GetAngryPrompt(), ShowCustomerDialogue));
            yield return StartCoroutine(GetResponseOptions((options, correctIndex) =>
            {
                correctOptionIndex = correctIndex;
                for (int i = 0; i < 2; i++) // Adjusted for 2 options
                {
                    replyButtonTexts[i].text = options[i];
                    int capturedIndex = i;
                    replyButtons[i].onClick.RemoveAllListeners();
                    replyButtons[i].onClick.AddListener(() => OnPlayerChoose(capturedIndex));
                }
                EnableButtons(true);
                isWaitingForReply = true;
            }));

            while (isWaitingForReply)
                yield return null;

            round++;
        }

        // Final reaction
        if (correctAnswers >= 3)
        {
            ShowCustomerDialogue("Okay... maybe it's not so bad. I'll give it a chance.");
            GivePlayerBonus();
        }
        else
        {
            ShowCustomerDialogue("Ugh, whatever! I�m leaving.");
        }
    }

    public void OnPlayerChoose(int index)
    {
        EnableButtons(false);
        isWaitingForReply = false;

        if (index == correctOptionIndex)
        {
            correctAnswers++;
            ShowCustomerDialogue("Hmm... that's actually a good point.");
        }
        else
        {
            ShowCustomerDialogue("No way! That�s nonsense.");
        }
    }

    private string GetAngryPrompt()
    {
        return "You are an angry customer who dislikes sustainability. " +
               "Complain loudly in under 15 words about how bad sustainable products are.";
    }

    private IEnumerator GetResponseOptions(System.Action<string[], int> callback)
    {
        string optionsPrompt =
            "The player is trying to convince an angry customer to care about sustainability. " +
            "Suggest 2 short persuasive replies (under 10 words). " +
            "Only ONE should be truly convincing. Mark it with **. Format: reply1, reply2.";

        string[] parsedOptions = new string[2];
        int correctIndex = -1;

        yield return StartCoroutine(GetChatGPTResponse(optionsPrompt, response =>
        {
            response = response.Replace("\n", " ").Replace("\r", " ");
            string[] split = response.Split(',');

            for (int i = 0; i < 2; i++)
            {
                if (i < split.Length)
                {
                    string option = split[i].Trim();

                    // Strip leading/trailing ** and mark the correct option
                    bool isCorrect = option.Contains("**");
                    option = option.Replace("**", "").Trim();

                    if (isCorrect && correctIndex == -1)
                    {
                        correctIndex = i; // Set the correct index
                    }

                    parsedOptions[i] = option;
                }
                else
                {
                    parsedOptions[i] = "...";
                }
            }

            callback(parsedOptions, correctIndex);
        }));
    }


    private IEnumerator GetChatGPTResponse(string prompt, System.Action<string> callback)
    {
        var requestData = new RequestData
        {
            messages = new[] { new Message { role = "user", content = prompt } }
        };

        string json = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest("https://api.openai.com/v1/chat/completions", "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {openAIApiKey}");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"ChatGPT Error: {request.error}");
            callback("Error getting response.");
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;
            try
            {
                var data = JsonUtility.FromJson<ResponseData>(jsonResponse);
                callback(data.choices[0].message.content.Trim());
            }
            catch
            {
                callback("Failed to parse ChatGPT response.");
            }
        }
    }

    private void ShowCustomerDialogue(string message)
    {
        dialogueText.text = message;
    }

    private void EnableButtons(bool enable)
    {
        foreach (var btn in replyButtons)
        {
            btn.gameObject.SetActive(enable);
        }
    }

    private void GivePlayerBonus()
    {
        if (customerSystem != null)
        {
            customerSystem.totalScore += 5;
            Debug.Log("Player convinced the customer! Bonus awarded.");
        }
    }
}
