using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Toast : MonoBehaviour
{
    public GameObject PlasticPlate;
    public GameObject GlassPlate;
    bool onPlate;

    public GameObject StrawJam;
    public GameObject Strawberry;
    public GameObject OrangeJam;
    public GameObject Orange;
    public GameObject Cheese;
    public GameObject Tomato;
    public GameObject Basil;
    public GameObject rawEgg;
    public GameObject cookedEgg;

    public bool SSpoon = false;
    public bool isStrawberry;
    public bool isOrange;
    public bool isTomato;
    public bool isCheese;
    public bool isBasil;
    public bool isEgg;
    public bool isRawEgg;

    private Renderer ToastRenderer;
    private bool isCooking;
    public bool isMicrowaved;

    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable Grab;
    public Material material;

    public ImgsFillDynamic fillBar; // Progress bar reference
    public Text txtValue; // Text showing status
    public float microwaveTime = 5f; // Total microwave time

    private float elapsedTime = 0f;

    private AudioSource audioSource;
    public AudioClip dingSound;

    private void Start()
    {
        Grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        ToastRenderer = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();

        if (fillBar != null)
        {
            fillBar.gameObject.SetActive(false); // Hide progress bar at start
        }
        else
        {
            Debug.LogWarning("Fill bar not assigned in inspector!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlasticPlate"))
        {
            PlaceOnPlate(PlasticPlate);
        }
        else if (other.CompareTag("GlassPlate"))
        {
            PlaceOnPlate(GlassPlate);
        }
        else if (other.CompareTag("Spoon") || other.CompareTag("SSpoon"))
        {
            Spoon SpoonObj = other.GetComponent<Spoon>();

            if (other.CompareTag("SSpoon"))
            {
                SSpoon = true;
            }

            if (SpoonObj != null)
            {
                if (SpoonObj.isStrawb)
                {
                    AddIngredient(StrawJam, Strawberry, ref isStrawberry);
                    SpoonObj.Remove();
                }
                else if (SpoonObj.isOrange)
                {
                    AddIngredient(OrangeJam, Orange, ref isOrange);
                    SpoonObj.Remove();
                }
            }
        }
        else if (other.CompareTag("microwave") && !isCooking && !isMicrowaved)
        {
            Debug.Log("Toast entered microwave. Starting MicrowaveSequence.");
            isCooking = true;
            elapsedTime = 0f;
            StartCoroutine(MicrowaveSequence());
        }
        else if (other.CompareTag("Cheese"))
        {
            // Enable Cheese child object
            if (Cheese != null)
            {
                Cheese.SetActive(true); // Enable the cheese object
                Cheese.transform.SetParent(this.transform); // Set cheese as child of the toast
                isCheese = true;

            }
        }
        else if (other.CompareTag("Pan"))
        {
            Pan PanObj = other.GetComponent<Pan>();

            // Check if cooked egg is on the pan
            if (PanObj.isCooked)
            {
                cookedEgg.SetActive(true); // Enable the cooked egg if present
                isEgg = true;
            }
        }
        else if (other.CompareTag("Egg"))
        {
            // Enable raw egg
            if (SceneManager.GetActiveScene().name == "short")
            {
                cookedEgg.SetActive(true); // Enable raw egg if present
                Destroy(other.gameObject);
   
            }else{
                rawEgg.SetActive(true); // Enable raw egg if present
                isRawEgg = true;
                Destroy(other.gameObject);

            }
        }
        else if (other.CompareTag("Tomato"))
        {
            
            Tomato.SetActive(true);
            isTomato=true;
            Destroy(other.gameObject);

        }
        else if (other.CompareTag("Basil"))
        {

            Basil.SetActive(true);
            isBasil = true;
        }
    }


    private void PlaceOnPlate(GameObject plate)
    {
        if (plate != null && !onPlate)
        {
            plate.SetActive(true);
            onPlate = true;
        }
    }

    private void AddIngredient(GameObject ingredient, GameObject additionalObject, ref bool flag)
    {
        if (ingredient != null)
        {
            ingredient.SetActive(true);
            flag = true;
        }
        if (additionalObject != null)
        {
            additionalObject.SetActive(true);
        }
    }

    private IEnumerator MicrowaveSequence()
    {
        if (fillBar != null)
        {
            fillBar.SetValue(0f, true);
            fillBar.gameObject.SetActive(true); // Show progress bar
            Debug.Log("Fill bar activated.");
        }

        if (txtValue != null)
        {
            txtValue.text = "Heating Up";
            StartCoroutine(UpdateMicrowavingText());
        }

        while (elapsedTime < microwaveTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / microwaveTime);

            if (fillBar != null)
                fillBar.SetValue(progress, false); // Update fill value

            yield return null;
        }

        MicrowaveToast(); // After full time
    }

    private IEnumerator UpdateMicrowavingText()
    {
        string baseText = "Heating Up";
        int dotCount = 0;

        while (isCooking)
        {
            dotCount = (dotCount + 1) % 4;
            if (txtValue != null)
            {
                txtValue.text = baseText + new string('.', dotCount);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void MicrowaveToast()
    {
        Debug.Log("Microwaving complete!");

        if (Grab != null)
            Grab.enabled = true;

        if (ToastRenderer != null && material != null)
        {
            ToastRenderer.material = material;
        }

        isMicrowaved = true;
        isCooking = false;

        if (txtValue != null)
            txtValue.text = "Complete!";

        if (fillBar != null)
        {
            fillBar.SetValue(1f, false); // Ensure bar fills fully
        }

        if (audioSource != null && dingSound != null)
        {
            audioSource.PlayOneShot(dingSound);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("microwave"))
        {
            if (fillBar != null)
            {
                fillBar.SetValue(0f, true);
                fillBar.gameObject.SetActive(false); // Hide progress bar
            }

            if (txtValue != null)
                txtValue.text = "";
        }
    }
}
