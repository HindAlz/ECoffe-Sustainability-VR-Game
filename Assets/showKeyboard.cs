using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
public class showKeyboard : MonoBehaviour
{
    private TMP_InputField inputField;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.onSelect.AddListener(x=> OpenKeybard());
    }

    // Update is called once per frame
    void OpenKeybard()
    {
        NonNativeKeyboard.Instance.InputField = inputField;
        NonNativeKeyboard.Instance.PresentKeyboard(inputField.text);
    }
}
