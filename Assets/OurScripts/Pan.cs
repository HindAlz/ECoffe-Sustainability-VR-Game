using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Pan : MonoBehaviour
{
    public float cookTime = 5f;
    private bool isCooking;
    public bool isCooked;
    public bool hasEgg;
    public GameObject crackedEgg;
    public GameObject cookedEgg;
    public bool picked;

    public ImgsFillDynamic fillBar;   // Assign in inspector
    public Text txtValue;             // Assign in inspector (optional)

    private AudioSource audioSource;   // AudioSource to play sound
    public AudioClip eggDropClip;     // Egg crack sound
    public AudioClip cookingClip;     // Sizzling loop sound
    public AudioClip dingClip;        // Ding when done

    private void Start()
    {
        if (fillBar != null) fillBar.gameObject.SetActive(false);
        if (txtValue != null) txtValue.text = "";

        // Initialize AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing from the GameObject!");
        }
    }

    void Update()
    {
        if (fillBar != null)
        {
            fillBar.transform.position = transform.position + new Vector3(0, 0.16f, 0);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Egg"))
        {
            picked = false;
            hasEgg = true;
            crackedEgg.SetActive(true);
            Destroy(other.gameObject);

            PlayEggDropSound();
            StartCoroutine(StartCooking());
        }
        else if (other.CompareTag("Toast") && isCooked)
        {
            picked = true;
            StopCookingUI();
            cookedEgg.SetActive(false);
            crackedEgg.SetActive(false);
            Invoke("AppearEggOnToast", 2f);
        }
    }

    void AppearEggOnToast()
    {
        isCooked = false;
        hasEgg = false;
    }

    private IEnumerator StartCooking()
    {
        if (isCooking || isCooked || !hasEgg) yield break;

        isCooking = true;
        if (fillBar != null)
        {
            fillBar.SetValue(0f, true);
            fillBar.gameObject.SetActive(true);
        }

        if (txtValue != null)
        {
            txtValue.text = "Cooking";
            StartCoroutine(UpdateCookingText());
        }

        PlayCookingSound();

        float elapsedTime = 0f;
        while (elapsedTime < cookTime)
        {
            if (!hasEgg) yield break;
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / cookTime);
            fillBar?.SetValue(progress, false);
            yield return null;
        }

        FinishCooking();
    }

    private IEnumerator UpdateCookingText()
    {
        string baseText = "Cooking";
        int dotCount = 0;

        while (isCooking)
        {
            dotCount = (dotCount + 1) % 4;
            txtValue.text = baseText + new string('.', dotCount);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void FinishCooking()
    {
        isCooking = false;
        isCooked = true;

        crackedEgg.SetActive(false);
        cookedEgg.SetActive(true);

        StopCookingSound();
        PlayDingSound();

        if (txtValue != null)
            txtValue.text = "Complete!";
    }

    private void StopCookingUI()
    {
        isCooking = false;
        hasEgg = false;

        if (fillBar != null)
        {
            fillBar.gameObject.SetActive(false);
            fillBar.SetValue(0f, true);
        }

        if (txtValue != null)
            txtValue.text = "";
    }

    private void PlayEggDropSound()
    {
        if (audioSource != null && eggDropClip != null)
        {
            audioSource.PlayOneShot(eggDropClip);  // Play once without interfering with other sounds
        }
    }

    private void PlayCookingSound()
    {
        if (audioSource != null && cookingClip != null)
        {
            audioSource.clip = cookingClip;
            audioSource.loop = true;
            audioSource.volume = 0.5f; // Set lower volume for sizzling
            audioSource.Play();
        }
    }


    private void StopCookingSound()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.loop = false;
            audioSource.clip = null;
            audioSource.volume = 1f; // Reset volume after sizzling is done
        }
    }


    private void PlayDingSound()
    {
        if (audioSource != null && dingClip != null)
        {
            audioSource.PlayOneShot(dingClip); // Play ding without replacing clip
        }
    }
}
