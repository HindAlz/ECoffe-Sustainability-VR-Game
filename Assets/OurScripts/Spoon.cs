using UnityEngine;

public class Spoon : MonoBehaviour
{
    public bool isStrawb;
    public bool isOrange;
    public Material strawbMat;
    public Material orangeMat;
    public Material none;
    private Renderer jamRenderer;

    // Define the AudioSource for collision and removal sound
    private AudioSource audioSource;

    // Define the AudioClips
    public AudioClip jamCollisionClip; // Drag the collision sound clip here in the inspector
    public AudioClip jamRemovalClip;   // Drag the removal sound clip here in the inspector

    void Start()
    {
        jamRenderer = GetComponent<Renderer>();

        // Initialize the AudioSource
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing from the GameObject!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Play the collision sound when colliding with "StrawJam"
        if (other.CompareTag("StrawJam"))
        {
            isStrawb = true;
            isOrange = false;
            jamRenderer.material = strawbMat;

            PlaySoundForDuration(jamCollisionClip, 0.5f);
        }
        // Play the collision sound when colliding with "OrangeJam"
        else if (other.CompareTag("OrangeJam"))
        {
            isOrange = true;
            isStrawb = false;
            jamRenderer.material = orangeMat;

            PlaySoundForDuration(jamCollisionClip, 0.5f);
        }
    }

    public void Remove()
    {
        isStrawb = false;
        isOrange = false;
        jamRenderer.material = none;

        // Play the removal sound when removing the jam
        PlaySoundForDuration(jamRemovalClip, 0.5f);
    }

    private void PlaySoundForDuration(AudioClip audioClip, float duration)
    {
        if (audioSource != null && audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();

            // Stop the sound after the duration
            StartCoroutine(StopSoundAfterDelay(duration));
        }
    }

    private System.Collections.IEnumerator StopSoundAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.Stop();
    }
}
