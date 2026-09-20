using UnityEngine;
using TMPro;

public class VRKeyboardManager : MonoBehaviour
{
    public static VRKeyboardManager Instance;

    public GameObject vrKeyboard; // Assign your VR Keyboard GameObject in the Inspector
    public TMP_InputField currentInputField;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SelectInputField(TMP_InputField inputField)
    {
        currentInputField = inputField;
        vrKeyboard.SetActive(true);
    }

    public void TypeKey(string key)
    {
        if (currentInputField != null)
        {
            currentInputField.text += key;
        }
    }

    public void Backspace()
    {
        if (currentInputField != null && currentInputField.text.Length > 0)
        {
            currentInputField.text = currentInputField.text.Substring(0, currentInputField.text.Length - 1);
        }
    }

    public void HideKeyboard()
    {
        vrKeyboard.SetActive(false);
    }
}
