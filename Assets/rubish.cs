using UnityEngine;

public class Rubbish : MonoBehaviour
{
    [Header("Trash Settings")]
    [Tooltip("Tags of objects that can be thrown away")]
    [SerializeField] private string[] trashableTags = { "IceCup", "EspressoCup", "Mug", "TeaCup", "Toast", "Croisant" };
    [SerializeField] private int scorePenalty = 5;

    [Header("Audio")]
    [SerializeField] private AudioClip trashSound;

    private AudioSource audioSource;
    private CustomerSystem customerSystem;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        customerSystem = FindObjectOfType<CustomerSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if collided object should be trashed
        foreach (string tag in trashableTags)
        {
            if (other.CompareTag(tag))
            {
                PlayTrashSound();
                DestroyTrashObject(other.gameObject);
                ApplyScorePenalty();
                return;
            }
        }
    }

    private void PlayTrashSound()
    {
        if (trashSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(trashSound);
        }
    }

    private void DestroyTrashObject(GameObject objectToTrash)
    {
        // Special handling for drinks (destroy the parent cup holder)
        if (objectToTrash.CompareTag("IceCup") ||
            objectToTrash.CompareTag("EspressoCup") ||
            objectToTrash.CompareTag("Mug") ||
            objectToTrash.CompareTag("TeaCup"))
        {
            if (objectToTrash.transform.parent != null)
            {
                Destroy(objectToTrash.transform.parent.gameObject);
            }
            else
            {
                Destroy(objectToTrash);
            }
        }
        else // For toast and croissants
        {
            Destroy(objectToTrash);
        }
    }

    private void ApplyScorePenalty()
    {
        if (customerSystem != null)
        {
            customerSystem.totalScore = Mathf.Max(0, customerSystem.totalScore - scorePenalty);
            Debug.Log($"Penalty! -{scorePenalty} points. New score: {customerSystem.totalScore}");
        }
    }
}