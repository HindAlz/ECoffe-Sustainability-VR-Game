using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class VRButton : MonoBehaviour
{
    public float deadTime = 1.0f;
    private bool _deadTimeActive = false;

    public UnityEvent onPressed, onReleased;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable simpleInteractable;

    private void Awake()
    {
        simpleInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        if (simpleInteractable != null)
        {
            simpleInteractable.selectEntered.AddListener(OnButtonPressed);
            simpleInteractable.selectExited.AddListener(OnButtonReleased);
        }
        else
        {
            Debug.LogError("XRSimpleInteractable component missing on " + gameObject.name);
        }
    }

    private void OnButtonPressed(SelectEnterEventArgs args)
    {
        if (!_deadTimeActive)
        {
            onPressed?.Invoke();
            Debug.Log("Simple Button Pressed");
        }
    }

    private void OnButtonReleased(SelectExitEventArgs args)
    {
        if (!_deadTimeActive)
        {
            onReleased?.Invoke();
            Debug.Log("Simple Button Released");
            StartCoroutine(WaitForDeadTime());
        }
    }

    private IEnumerator WaitForDeadTime()
    {
        _deadTimeActive = true;
        yield return new WaitForSeconds(deadTime);
        _deadTimeActive = false;
    }
}
