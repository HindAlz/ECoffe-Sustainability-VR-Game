using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

[System.Serializable]
public class Message
{
    public string role;
    public string content;
}

[System.Serializable]
public class ChatRequest
{
    public string model;
    public Message[] messages;
    public float temperature;
    public int max_tokens;
}

public class GPTDialogue : MonoBehaviour
{
    public TMP_InputField playerInput;
    public TMP_Text npcResponseText;
    public GameObject correctAnswerPanel;
    public TMP_Text correctAnswerText;
    public System.Action OnConversationFinished;

    public string openAI_API_Key = "key_here";

    private const string API_URL = "https://api.openai.com/v1/chat/completions";

    private string correctExplanation = "Sustainable utensils, cleaner planet!";

    public void OnSendButtonClicked()
    {
        string userMessage = playerInput.text;

        if (userMessage.Length > 200)
        {
            userMessage = userMessage.Substring(0, 200);
        }

        StartCoroutine(SendToGPT(userMessage));
    }

    private bool IsPositiveResponse(string reply)
    {
        // Check if the response contains "OK fine, I agree"
        return reply.ToLower().Contains("ok fine");
    }

    private bool IsNegativeResponse(string reply)
    {
        // If the response is positive, treat it as a non-negative response
        if (IsPositiveResponse(reply))
        {
            return false;
        }return true;

        // Check for specific negative phrases
        //reply = reply.ToLower();
       // return reply.Contains("no") || reply.Contains("i don't") || reply.Contains("not convinced") ||
        //       reply.Contains("still hate") || reply.Contains("won�t") || reply.Contains("don't care") ||
        //       reply.Contains("refuse") || reply.Contains("never");
    }

    IEnumerator SendToGPT(string playerMessage)
    {
        npcResponseText.text = "Thinking...";

        Message[] messages = new Message[]
        {
        new Message { role = "system", content = "You're a grumpy NPC. You hate sustainable utensils. If the player gives a reason for sustainability, agree and say at the start 'OK fine I agree' nd stop being grumpy. Otherwise, stay grumpy. All your responses should be around 7 words or less" },
        new Message { role = "user", content = playerMessage }
        };

        ChatRequest chatRequest = new ChatRequest
        {
            model = "gpt-4o",  // Ensure you use the correct model
            messages = messages,
            temperature = 0.7f,
            max_tokens = 50  // Limit to 50 tokens for cost-effective responses
        };

        string json = JsonUtility.ToJson(chatRequest);
        json = FixJsonArray(json, messages);
        Debug.Log("Sending to AI: " + json);

        UnityWebRequest request = new UnityWebRequest(API_URL, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + openAI_API_Key);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            npcResponseText.text = "Error: " + request.error;
        }
        else
        {
            string responseJson = request.downloadHandler.text;
            string reply = ExtractReplyFromJson(responseJson);
            npcResponseText.text = reply;

            if (IsNegativeResponse(reply))
            {
                correctAnswerPanel.SetActive(true);
                correctAnswerText.text = "Oops! Here's the correct reason:\n" + correctExplanation;
            }
            else
            {
                correctAnswerPanel.SetActive(false);
            }
        }
        if (OnConversationFinished != null)
        {
            OnConversationFinished.Invoke();
        }

    }


    private string FixJsonArray(string originalJson, Message[] messages)
    {
        string messagesJson = "[";
        for (int i = 0; i < messages.Length; i++)
        {
            messagesJson += JsonUtility.ToJson(messages[i]);
            if (i < messages.Length - 1) messagesJson += ",";
        }
        messagesJson += "]";

        int messagesStart = originalJson.IndexOf("\"messages\":");
        int startBrace = originalJson.IndexOf('[', messagesStart);
        int endBrace = originalJson.IndexOf(']', startBrace) + 1;

        return originalJson.Substring(0, startBrace) + messagesJson + originalJson.Substring(endBrace);
    }

    private string ExtractReplyFromJson(string json)
    {
        // Deserialize the JSON response to match the OpenAI API structure
        var response = JsonUtility.FromJson<OpenAIResponse>(json);

        if (response == null || response.choices.Length == 0)
        {
            return "Could not parse response.";
        }

        // Check if the choices have the expected content and extract the text
        if (response.choices.Length > 0 && response.choices[0].message != null && !string.IsNullOrEmpty(response.choices[0].message.content))
        {
            string content = response.choices[0].message.content;
            return content.Replace("\\n", "\n").Replace("\\\"", "\"");
        }

        return "Could not find text content.";
    }


    
}

[System.Serializable]
public class OpenAIResponse
{
    public Choice[] choices;
}

[System.Serializable]
public class Choice
{
    public Message message;
}

