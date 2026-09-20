using System.Collections;
using UnityEngine;

public class ThiefBehavior : MonoBehaviour
{
    public Transform playerHead;
    public Transform upperArm;
    public Transform forearm;
    public Transform hand;
    public Transform tipJar;
    public float reachSpeed = 0.5f;
    public float retractSpeed = 1f;
    public float stealDistance = 0.2f;

    public bool hasStolen = false;

    private Animator animator;
    private bool isStealing = false;
    private Vector3 originalHandPos;
    private Vector3 originalForearmPos;

    public AudioClip shufflingSound;  // Shuffling sound to play when stealing
    private AudioSource audioSource;

    public CustomerSystem customerSystem;  // Reference to CustomerSystem

    void Start()
    {
        if (playerHead == null)
            playerHead = Camera.main.transform;

        if (tipJar == null)
        {
            // Find the GameObject named "Tips" and assign its Transform to tipJar
            GameObject tipJarObject = GameObject.Find("Tips");
            if (tipJarObject != null)
            {
                tipJar = tipJarObject.transform;
                Debug.Log("Tip jar assigned from GameObject 'Tips'.");
            }
            else
            {
                Debug.LogWarning("Tip jar GameObject with name 'Tips' not found.");
            }
        }

        // Retrieve the CustomerSystem from the GameManager
        GameObject gameManager = GameObject.Find("Game Manager");
        if (gameManager != null)
        {
            customerSystem = gameManager.GetComponent<CustomerSystem>();
            if (customerSystem != null)
            {
                Debug.Log("CustomerSystem successfully assigned from GameManager.");
            }
            else
            {
                Debug.LogWarning("CustomerSystem not found on GameManager.");
            }
        }
        else
        {
            Debug.LogWarning("GameManager GameObject not found.");
        }

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();  // Get AudioSource component
    }

    public void StartStealing()
    {
        if (!isStealing)
        {
            originalHandPos = hand.position;
            originalForearmPos = forearm.position;

            if (animator != null)
                animator.enabled = false;

            StartCoroutine(StealRoutine());
        }
    }

    IEnumerator StealRoutine()
    {
        isStealing = true;
        bool isShufflingPlaying = false;  // New: track if sound is playing

        while (true)
        {
            if (hasStolen)
            {
                // Stop shuffling sound if still playing
                if (isShufflingPlaying && audioSource != null)
                {
                    audioSource.Stop();
                    isShufflingPlaying = false;
                }

                // Finish retracting, return to animation
                MoveLimbTowards(hand, originalHandPos, retractSpeed);
                MoveLimbTowards(forearm, originalForearmPos, retractSpeed);

                if (AllLimbsReturnedToIdle())
                {
                    if (animator != null && !animator.enabled)
                        animator.enabled = true;

                    if (customerSystem != null)
                    {
                        Debug.Log("Thief stole from the tip jar! Score reduced by 5.");
                    }

                    yield break; // Done stealing!
                }

                yield return null;
                continue;
            }

            bool playerLooking = Vector3.Dot(playerHead.forward, (transform.position - playerHead.position).normalized) > 0.6f;

            if (!playerLooking)
            {
                if (animator != null && animator.enabled)
                    animator.enabled = false;

                // Start playing shuffling sound if not already
                if (!isShufflingPlaying && audioSource != null && shufflingSound != null)
                {
                    audioSource.clip = shufflingSound;
                    audioSource.loop = true;
                    audioSource.Play();
                    isShufflingPlaying = true;
                }

                // Reach toward the tip jar
                MoveLimbTowards(hand, tipJar.position, reachSpeed);
                Vector3 forearmMid = Vector3.Lerp(originalForearmPos, tipJar.position, 0.5f);
                MoveLimbTowards(forearm, forearmMid, reachSpeed * 0.8f);

                if (Vector3.Distance(hand.position, tipJar.position) < stealDistance)
                {
                    Debug.Log("?? Thief stole from the tip jar!");
                    customerSystem.totalScore -= 5;
                    hasStolen = true;
                }
            }
            else
            {
                // Player looking -> stop shuffling sound if playing
                if (isShufflingPlaying && audioSource != null)
                {
                    audioSource.Stop();
                    isShufflingPlaying = false;
                }

                // Retract hand
                MoveLimbTowards(hand, originalHandPos, retractSpeed);
                MoveLimbTowards(forearm, originalForearmPos, retractSpeed);

                if (AllLimbsReturnedToIdle())
                {
                    if (animator != null && !animator.enabled)
                        animator.enabled = true;
                }
            }

            yield return null;
        }
    }

    void MoveLimbTowards(Transform limb, Vector3 targetPosition, float speed)
    {
        if (limb == null) return;

        Vector3 newPos = Vector3.MoveTowards(limb.position, targetPosition, speed * Time.deltaTime);
        float minY = transform.position.y;
        newPos.y = Mathf.Max(newPos.y, minY);
        limb.position = newPos;
    }

    bool AllLimbsReturnedToIdle()
    {
        return Vector3.Distance(hand.position, originalHandPos) < 0.05f &&
               Vector3.Distance(forearm.position, originalForearmPos) < 0.05f;
    }
}
