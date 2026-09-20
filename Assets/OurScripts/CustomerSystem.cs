
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using TMPro;


public class CustomerSystem : MonoBehaviour
{
    [Header("Setup")]
    public GameObject bubble;
    public GameObject angryBubble;
    public Transform[] walkPoints;
    public Transform spawnPoint;
    public Transform orderCompleteSpot;
    public ImgsFillDynamic patienceBar;
    public Text orderAndPatienceText;
    public Text tipJarText;
    [Header("Sound Effects")]
    public AudioClip orderSuccessSound;
    [Header("Customer Prefabs")]
    public List<GameObject> normalCustomers;
    public List<GameObject> angryCustomers;
    public List<GameObject> thiefCustomers;
    public GameObject nightTimeGameObject;
    [Header("Lighting")]
    public Light mainLight;
    [Range(0f, 1f)] public float nightLightIntensity = .5f;
    [Range(0f, 1f)] public float nightAmbientDim = 0f;
    private float originalAmbientLight;
    [Header("Gameplay")]
    public int customersPerDay = 6;
    public bool nightShift = false;
    public GPTDialogue gptDialogue;
    public GameObject GPTCanv;
    [Header("Customer Probability Settings")]
    [Range(0f, 1f)] public float dayAngryChance = 0f;
    [Range(0f, 1f)] public float nightThiefChance = 0.4f;

    [Header("Recipes")]
    public Dictionary<string, List<string>> recipes = new Dictionary<string, List<string>>();
    public PlayerInventoryManager playerInventoryManager;
    PlayerData data;

    [Header("Audio Settings")]
    public AudioClip doorbellClip;
    private AudioSource audioSource;
    public TutorialManager TutorialManager;
    private enum CustomerType { Normal, Angry, Thief }
    private GameObject currentCustomer;
    private int currentCustomerIndex = 0;
    private string currentOrder;
    private List<string> currentRecipe;
    public float patienceTimer = 100f;
    private bool waitingForOrder = false;
    private float walkSpeed = 2f;
    private CustomerType currentCustomerType;
    public int totalScore = 0;
    bool Done = true;
    [Header("UI References")]
    public TMP_Text drinksText;
    public TMP_Text foodsText;

    [Header("Settings")]
    public float initialDelay = 5f; // Time to wait for recipes to register
    public string loadingMessage = "Loading recipes...";
    public string noRecipesMessage = "No recipes available yet!";
    private bool locke=false;

    public GameObject endDay;
    public GameObject Shop;
    void Awake()
    {
        data = SaveSystem.Load();

        // If data is null, create default
        if (data == null)
        {
            data = new PlayerData();
            SaveSystem.Save(data);
        }
    }


    void Start()
    {
        data = SaveSystem.Load();
        StartCoroutine(InitializeWithDelay());
        StartCoroutine(WaitForReady());

    }

    IEnumerator InitializeWithDelay()
    {
        // Wait for recipes to register
        yield return new WaitForSeconds(initialDelay);

    }



    bool IsDrink(List<string> ingredients)
    {
        string[] drinkKeywords = { "Coffee", "Tea", "Juice", "Milk", "Water", "Ice", "Espresso" };
        return ingredients.Any(i => drinkKeywords.Any(k => i.Contains(k)));
    }

    void ShowErrorState()
    {
        drinksText.text = "Error loading recipes";
        foodsText.text = "Please restart the game";
    }



    IEnumerator WaitForReady()
    {
        yield return new WaitUntil(() => Done);
        originalAmbientLight = RenderSettings.ambientIntensity;
        if (patienceBar != null) patienceBar.SetValue(1f, true);
        audioSource = GetComponent<AudioSource>();

        if (SceneManager.GetActiveScene().name != "Shop")
            SpawnCustomer();
        else Shop.SetActive(true);
    }
    void SpawnCustomer()
    {
        if (currentCustomerIndex >= customersPerDay) return;

        // Switch to night after 5 customers
        if (currentCustomerIndex == 3 && !nightShift)
        {
            nightShift = true;
            mainLight.intensity = nightLightIntensity;
            // Set ambient lighting for night
            RenderSettings.ambientIntensity = originalAmbientLight * nightAmbientDim;
            nightTimeGameObject.SetActive(nightShift);

        }

        GameObject prefab = GetRandomCustomerType();
        currentCustomer = Instantiate(prefab, walkPoints[0].position, Quaternion.identity);

        audioSource.PlayOneShot(doorbellClip);

        currentOrder = GetRandomOrder();
        currentRecipe = new List<string>(recipes[currentOrder]);
        patienceTimer = 100f;
        waitingForOrder = true;
        currentCustomerIndex++;

        StartCoroutine(WalkCustomer(walkPoints[1], () =>
        {
            bubble.SetActive(true);
            StartCoroutine(HandleCustomerAtCounter());
        }, stopWalkingOnArrival: false));
    }

    GameObject GetRandomCustomerType()
    {
        CustomerType type;
        float rand = Random.value;
        if (!nightShift)
        {
            currentCustomerType = CustomerType.Normal;
            // Daytime probabilities
            //if (rand <= dayAngryChance) { currentCustomerType = CustomerType.Angry; Debug.Log("angy"); }
            //else currentCustomerType = CustomerType.Normal;
        }
        else
        {
            // Nighttime probabilities
            if (rand <= nightThiefChance) currentCustomerType = CustomerType.Thief;
            else currentCustomerType = CustomerType.Normal;
        }
    
        switch (currentCustomerType)
        {
            case CustomerType.Angry:
                return angryCustomers[Random.Range(0, angryCustomers.Count)];
            case CustomerType.Thief:
                return thiefCustomers[Random.Range(0, thiefCustomers.Count)];
            default:
                return normalCustomers[Random.Range(0, normalCustomers.Count)];
        }
    }

    string GetRandomOrder()
    {

        return playerInventoryManager.unlockedRecipeNames[Random.Range(0, playerInventoryManager.unlockedRecipeNames.Count)];

    }


    IEnumerator WalkCustomer(Transform target, System.Action onReach, bool stopWalkingOnArrival = true)
    {
        var cityPeople = currentCustomer.GetComponent<CityPeople.CityPeople>();
        if (cityPeople != null) cityPeople.SetWalking(true);

        Vector3 dir = (target.position - currentCustomer.transform.position).normalized;
        if (dir != Vector3.zero)
            currentCustomer.transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));

        while (currentCustomer != null && Vector3.Distance(currentCustomer.transform.position, target.position) > 0.1f)
        {
            currentCustomer.transform.position = Vector3.MoveTowards(
                currentCustomer.transform.position, target.position, walkSpeed * Time.deltaTime);
            yield return null;
        }


        if (cityPeople != null) cityPeople.SetWalking(false);
        if (currentCustomer != null) currentCustomer.transform.position = target.position;
        onReach?.Invoke();
    }

    IEnumerator HandleCustomerAtCounter()
    {
        waitingForOrder = true;

        if (currentCustomerType == CustomerType.Thief)
        {
            ThiefBehavior thief = currentCustomer.GetComponent<ThiefBehavior>();
            if (thief != null) thief.StartStealing();
        }

        yield break;
    }

    void Update()
    {
        tipJarText.text = "Tips: " + totalScore.ToString();

        if (waitingForOrder)
        {
            patienceTimer -= Time.deltaTime;

            patienceBar.SetValue(patienceTimer / 100f, true);
            string currentOrderName;
            if (currentOrder == "OJ") currentOrderName = "Orange Juice";
            else if (currentOrder == "Lemonade") currentOrderName = "Lemonade";
            else if (currentOrder == "Latte") currentOrderName = "Latte";
            else if (currentOrder == "RTea") currentOrderName = "Red Tea";
            else if (currentOrder == "GTea") currentOrderName = "Green Tea";
            else if (currentOrder == "Iced_Latte") currentOrderName = "Iced Latte";
            else if (currentOrder == "Iced_Tea") currentOrderName = "Iced Tea";
            else if (currentOrder == "Iced_Green_Tea") currentOrderName = "Iced Green Tea";
            else if (currentOrder == "Americano") currentOrderName = "Americano";

            else if (currentOrder == "Croissant") currentOrderName = "Croissant";
            else if (currentOrder == "SJam") currentOrderName = "Strawberry Jam Toast";
            else if (currentOrder == "OJam") currentOrderName = "Orange Jam Toast";
            else if (currentOrder == "EggT") currentOrderName = "Egg Toast";
            else if (currentOrder == "CherryT") currentOrderName = "Cherry Toast";

            else if (currentOrder == "ChocC") currentOrderName = "Chocolate Cheesecake";
            else if (currentOrder == "BlueberryC") currentOrderName = "Blueberry Cheesecake";

            else if (currentOrder == "CherryCup") currentOrderName = "Cherry Cupcake";
            else if (currentOrder == "ChocCup") currentOrderName = "Chocolate Cupcake";
            else if (currentOrder == "OreoCup") currentOrderName = "Oreo Cupcake";
            else if (currentOrder == "RedCup") currentOrderName = "Red Velvet Cupcake";

            else if (currentOrder == "BwDon") currentOrderName = "Black & White Donut";
            else if (currentOrder == "PinkDon") currentOrderName = "Pink Donut";
            else if (currentOrder == "WhiteDon") currentOrderName = "White Donut";
            else if (currentOrder == "Cookie") currentOrderName = "Cookie";
            else currentOrderName = currentOrder;
            orderAndPatienceText.text = $"Order:\n{currentOrderName}";

            if (patienceTimer <= 0)
            {
                StartCoroutine(WalkOutAndRemoveCustomer());
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Object entered trigger: {other.name} with tag: {other.tag}");

        if (other.CompareTag("IceCup") || other.CompareTag("EspressoCup") ||
            other.CompareTag("Mug") || other.CompareTag("Teacup"))
        {
            Debug.Log("Drink cup detected");
            DrinkTrack drink = other.GetComponentInParent<DrinkTrack>();
            if (drink != null)
            {
                Debug.Log("Drink component found, checking order...");
                CheckOrder(drink, other.tag);
                Destroy(drink.gameObject);

            }
            else
            {
                Debug.LogWarning("No DrinkTrack component found on cup");
            }
        }
        else if (other.CompareTag("Toast"))
        {
            Debug.Log("Toast detected");
            Toast toast = other.GetComponent<Toast>();
            if (toast != null)
            {
                bool onPlate = toast.PlasticPlate.activeSelf || toast.GlassPlate.activeSelf;
                Debug.Log($"Toast on plate: {onPlate} (Plastic: {toast.PlasticPlate.activeSelf}, Glass: {toast.GlassPlate.activeSelf})");

                if (onPlate)
                {
                    Debug.Log("Toast is on plate, checking order...");
                    CheckToastOrder(toast);
                    Debug.Log("Destroying toast after order check");
                    Destroy(other.gameObject);
                }
                else
                {
                    Debug.Log("Toast not on plate - rejecting");
                }
            }
            else
            {
                Debug.LogWarning("No Toast component found on toast object");
            }
        }
        else if (other.CompareTag("Croissant"))
        {
            Debug.Log("Croissant detected");
            croissant croissant = other.GetComponent<croissant>();
            if (croissant != null)
            {
                bool onPlate = croissant.PlasticPlate.activeSelf || croissant.GlassPlate.activeSelf;
                Debug.Log($"Croissant on plate: {onPlate} (Plastic: {croissant.PlasticPlate.activeSelf}, Glass: {croissant.GlassPlate.activeSelf})");

                if (onPlate)
                {
                    Debug.Log("Croissant is on plate, checking order...");
                    CheckCroissantOrder(croissant);
                    Debug.Log("Destroying croissant after order check");
                    Destroy(other.gameObject);
                }
                else
                {
                    Debug.Log("Croissant not on plate - rejecting");
                    Destroy(other.gameObject);

                }
            }
            else
            {
                Debug.LogWarning("No Croisant component found on croissant object");
            }
        }
        else
        {
            if (other.CompareTag(currentOrder))
            {
                Destroy(other.gameObject);

                CompleteCustomerOrder(10);

            }
            else if (!other.name.Contains("plate"))
            {
                Destroy(other.gameObject);

                CompleteCustomerOrder(0);
            }

        }
    }



    public void CheckOrder(DrinkTrack drink, string cupType)
    {
        if (drink == null || drink.drinkList.Count == 0)
        {
            Debug.Log("No drink components found");
            CompleteCustomerOrder(0);
            return;
        }

        Debug.Log($"Checking drink order. Current order: {currentOrder}, Recipe: {string.Join(", ", currentRecipe)}");
        Debug.Log($"Submitted drink: {string.Join(", ", drink.drinkList)}");

        bool correctDrink = CompareRecipes(drink.drinkList, currentRecipe);
        bool sustainableCup = cupType == "Sustainable";
        bool hasSustainableStraw = drink.gameObject.CompareTag("IceCup") && drink.transform.Find("Sustainable Straw") != null;

        int score = 0;
        if (correctDrink)
        {
            score += 10;
            Debug.Log("Correct drink - base score 10");
        }
        else
        {
            Debug.Log("Incorrect drink");
        }

        if (!sustainableCup)
        {
            data.susRating -= 1;
            score -= 5;
            Debug.Log("not Sustainable cup -5");
        }

        //if (!hasSustainableStraw)
        //{
          //  data.susRating -= 1;
            //score -= 5;
            //Debug.Log("non Sustainable straw -5");
        //}
        if (correctDrink && sustainableCup)
        {
            data.susRating += 1;
            score += 5;
            Debug.Log("fully correct bonus");
        }

        Debug.Log($"Final drink score: {score}");
        CompleteCustomerOrder(score);
    }

    public void CheckToastOrder(Toast toast)
    {
        if (toast == null)
        {
            Debug.Log("No toast component found");
            CompleteCustomerOrder(0);
            return;
        }

        bool onPlate = toast.PlasticPlate.activeSelf || toast.GlassPlate.activeSelf;
        bool sustainablePlate = onPlate && (toast.PlasticPlate.name.Contains("Sustainable") || toast.GlassPlate.name.Contains("Sustainable"));
        bool sustainableSpoon = toast.SSpoon == true;

        if (!onPlate && !sustainablePlate)
        {
            Debug.Log("Toast not on plate - rejecting");
            CompleteCustomerOrder(0);
            return;
        }

        List<string> toastState = new List<string>();
        if (toast.isMicrowaved) toastState.Add("Microwaved");
        if (toast.isStrawberry) toastState.Add("Strawberry Jam");
        if (toast.isOrange) toastState.Add("Orange Jam");
        if (toast.isCheese) toastState.Add("Cheese");
        if (toast.isTomato) toastState.Add("Tomato");
        if (toast.isBasil) toastState.Add("Basil");
        if (toast.isEgg) toastState.Add("Cooked Egg");
        if (toast.isRawEgg) toastState.Add("Raw Egg");

        Debug.Log($"Checking toast order. Current order: {currentOrder}, Recipe: {string.Join(", ", currentRecipe)}");
        Debug.Log($"Submitted toast: {string.Join(", ", toastState)}");

        bool correctToast = CompareRecipes(toastState, currentRecipe);
        int score = correctToast ? 10 : 0;
        if (!sustainablePlate)
        {
            data.susRating -= 1;
            score -= 5;
            Debug.Log("non Sustainable plate bonus -5");
        }
        else
        {
            data.susRating += 1;
            score += 5;
        }


        Debug.Log($"Final toast score: {score}");
        CompleteCustomerOrder(score);
    }

    public void CheckCroissantOrder(croissant croissant)
    {
        if (croissant == null)
        {
            Debug.Log("No croissant component found");
            CompleteCustomerOrder(0);
            return;
        }

        bool onPlate = croissant.PlasticPlate.activeSelf || croissant.GlassPlate.activeSelf;
        bool sustainablePlate = onPlate && (croissant.PlasticPlate.name.Contains("Sustainable") || croissant.GlassPlate.name.Contains("Sustainable"));

        if (!onPlate && !sustainablePlate)
        {
            Debug.Log("Croissant not on plate - rejecting");
            CompleteCustomerOrder(0);
            return;
        }

        List<string> croissantState = new List<string>();
        if (croissant.isCooked) croissantState.Add("Cooked");

        Debug.Log($"Checking croissant order. Current order: {currentOrder}, Recipe: {string.Join(", ", currentRecipe)}");
        Debug.Log($"Submitted croissant: {string.Join(", ", croissantState)}");

        bool correctCroissant = CompareRecipes(croissantState, currentRecipe);
        int score = correctCroissant ? 10 : 0;
        if (!sustainablePlate)
        {
            data.susRating -= 1;
            score -= 5;
            Debug.Log("non Sustainable plate bonus -5");
        }

        Debug.Log($"Final croissant score: {score}");
        CompleteCustomerOrder(score);
    }
    bool CompareRecipes(List<string> submittedItems, List<string> requiredRecipe)
    {
        // First check if all required items are present
        bool hasAllRequired = !requiredRecipe.Except(submittedItems).Any();

        // Then check if there are no extra items (optional - depends on your game design)
        bool noExtraItems = !submittedItems.Except(requiredRecipe).Any();

        // Return true only if all required items are present and no extras (if you want strict matching)
        return hasAllRequired && noExtraItems;
    }
    void CompleteCustomerOrder(int score)
    {
        Debug.Log($"Completing order with score: {score}");
        if (score>0){
            totalScore += score;
        }

        if (currentCustomerType == CustomerType.Angry && score < 15)
        {
            Debug.Log("Angry customer - starting conversation");
            StartCoroutine(StartAngryConversation());
        }
        else
        {
            audioSource.PlayOneShot(orderSuccessSound);

            StartCoroutine(WalkOutAndRemoveCustomer());
        }
    }

    IEnumerator StartAngryConversation()
    {

        Debug.Log("Angry customer is arguing about plastic use...");

        yield return new WaitForSeconds(1f);

        if (gptDialogue != null)
        {
            // Show conversation UI (make sure it's active)
            GPTCanv.SetActive(true);
            angryBubble.SetActive(true);
            bubble.SetActive(false);
            // Reset fields
            gptDialogue.npcResponseText.text = "What's your reason for using sustainable utensils?!";
            gptDialogue.playerInput.text = "";
            gptDialogue.playerInput.interactable = true;
            gptDialogue.playerInput.ActivateInputField();

            Debug.Log("Conversation UI enabled. Waiting for player input...");

            // Hook into GPT's response event
            gptDialogue.OnConversationFinished = () =>
            {
                StartCoroutine(WaitAndThenWalkOut());

            };

            // New coroutine
            IEnumerator WaitAndThenWalkOut()
            {
                yield return new WaitForSeconds(10f);
                StartCoroutine(WalkOutAndRemoveCustomer());

            }

        }
        else
        {
            Debug.LogError("GPTDialogue reference not assigned!");
        }
    }



    IEnumerator WalkOutAndRemoveCustomer()
    {
        GPTCanv.SetActive(false);
        angryBubble.SetActive(false);
        waitingForOrder = false;

        yield return new WaitForSeconds(2f);
        bubble.SetActive(false);
        FindObjectOfType<SustainableManager>()?.SpawnItemsForCustomer();
        yield return WalkCustomer(walkPoints[2], () =>
        {
            Destroy(currentCustomer);
        });

        yield return new WaitForSeconds(5f);
        locke=false;
        if ((currentCustomerIndex < customersPerDay) && !locke)
        {
            locke = true;
            SpawnCustomer();

        }
        else
        {
            data.Coins += totalScore;
            data.dayScore = totalScore;
            SaveSystem.Save(data);
            SceneManager.LoadScene("TrashSortVR");

        }
    }
    public void ResetGame()
    {
        // Reloads the current active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    // Add to your CustomerSystem class
    private Dictionary<string, bool> recipeCategories = new Dictionary<string, bool>(); // Tracks if recipe is drink

    public void RegisterRecipe(string name, List<string> ingredients, bool isDrink)
    {
        if (!recipes.ContainsKey(name))
        {
            recipes.Add(name, ingredients);
            recipeCategories.Add(name, isDrink);
            Debug.Log($"Registered {(isDrink ? "drink" : "food")} recipe: {name}");
        }
        else
        {
            Debug.LogWarning($"Recipe {name} already registered!");
        }
    }

    public bool IsDrinkRecipe(string recipeName)
    {
        if (recipeCategories.TryGetValue(recipeName, out bool isDrink))
        {
            return isDrink;
        }
        return false; // Default to food if not found
    }
    
    public void backToTitle()
    {
        SceneManager.LoadScene("Title");

    }
}

