using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using System.Collections;

public class VRShopManager : PlayerInventoryManager
{

    public GameObject shopUI;
    public TMP_Text currencyText;
    private TutorialManager tutorialManager;

    public GameObject foodSection;
    public GameObject drinkSection;
    public GameObject decorSection;
    public GameObject startButton;
    public List<Button> foodButtons;
    public List<Button> drinkButtons;
    public List<Button> decorButtons;

    private Dictionary<string, Button> recipeButtonMap = new Dictionary<string, Button>();
    private int playerCoins = 0;
    private const int RECIPE_COST = 10; // Price per recipe
   
    void Start()
    {
        tutorialManager = FindObjectOfType<TutorialManager>();
        
        InitializeRecipeButtons();
        FetchPlayerCurrency();
        FetchPurchasedRecipes();
        //Debug.Log("Attempting to log in to PlayFab...");

        //PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest
        //{
          //  CustomId = SystemInfo.deviceUniqueIdentifier,
            //CreateAccount = true
        //}, result =>
        //{
          //  Debug.Log("? PlayFab Login Successful!");
            
        //}, error =>
        //{
          //  Debug.LogError("? PlayFab Login Failed: " + error.GenerateErrorReport());
        //});
    }

    void InitializeRecipeButtons()
    {
        InitializeCategoryButtons(foodButtons, "Food");
        InitializeCategoryButtons(drinkButtons, "Drink");
        InitializeCategoryButtons(decorButtons, "Decor");
    }

    void InitializeCategoryButtons(List<Button> buttons, string category)
    {
        if (buttons == null || buttons.Count == 0)
        {
            Debug.LogWarning($"? No {category} buttons assigned! Check the Inspector.");
            return;
        }

        Debug.Log($"?? Initializing {category} buttons...");

        foreach (Button button in buttons)
        {
            if (button == null)
            {
                Debug.LogError($"? A button in {category} is NULL!");
                continue;
            }

            string recipeName = button.name;
            recipeButtonMap[recipeName] = button;
            Debug.Log($"? Button added to {category} map: {recipeName}");

            button.onClick.AddListener(() => BuyRecipeWrapper(recipeName));
        }
    }

    public void OpenShop()
    {
        Debug.Log("?? Opening Shop...");
        shopUI.SetActive(true);
        FetchPlayerCurrency();
        StartCoroutine(DelayedFetch());
        startButton.SetActive(false);
        ShowFoodSection();
    }

    IEnumerator DelayedFetch()
    {
        yield return new WaitForSeconds(1f);
        FetchPurchasedRecipes();
    }

    void FetchPlayerCurrency()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), result =>
        {
            if (result.VirtualCurrency.ContainsKey("CO"))
            {
                playerCoins = result.VirtualCurrency["CO"];
                currencyText.text = "Coins: " + playerCoins;
                Debug.Log($"?? Player Coins Updated: {playerCoins}");
            }
        }, error =>
        {
            Debug.LogError("? Error fetching currency: " + error.GenerateErrorReport());
        });
    }

    void FetchPurchasedRecipes()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            foreach (var item in result.Data)
            {
                if (recipeButtonMap.ContainsKey(item.Key) && item.Value.Value == "1")
                {
                    Button button = recipeButtonMap[item.Key];

                    if (button != null)
                    {
                        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
                        if (buttonText != null)
                        {
                            button.interactable = false;
                            buttonText.text = "Bought";
                        }
                    }
                }
            }
        }, error =>
        {
            Debug.LogError("? Error fetching data: " + error.GenerateErrorReport());
        });
    }

    public void BuyRecipeWrapper(string recipeName)
    {
        if (recipeButtonMap.ContainsKey(recipeName))
        {
            BuyRecipe(recipeName, recipeButtonMap[recipeName]);
        }
        else
        {
            Debug.LogError($"? Button not found for recipe: {recipeName}");
        }
    }

    public void BuyRecipe(string recipeName, Button button)
    {
        if (!button.interactable) return; // Prevents duplicate purchases

        button.interactable = false; // Disable button immediately
        button.GetComponentInChildren<TMP_Text>().text = "Processing...";

        StartCoroutine(TryBuyRecipe(recipeName, button));
    }

    IEnumerator TryBuyRecipe(string recipeName, Button button)
    {
        yield return new WaitForSeconds(0.5f); // Small delay to avoid spam

        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), result =>
        {
            int serverCoins = result.VirtualCurrency.ContainsKey("CO") ? result.VirtualCurrency["CO"] : 0;

            if (serverCoins < RECIPE_COST)
            {
                Debug.Log("? Not enough coins!");
                button.interactable = true; // Re-enable button if purchase fails
                button.GetComponentInChildren<TMP_Text>().text = "Buy";
                return;
            }

            // Deduct coins from PlayFab (ONLY ONCE)
            PlayFabClientAPI.SubtractUserVirtualCurrency(new SubtractUserVirtualCurrencyRequest
            {
                VirtualCurrency = "CO",
                Amount = RECIPE_COST
            }, subResult =>
            {
                Debug.Log("? Coins deducted successfully!");

                // Update local coin count
                playerCoins = serverCoins - RECIPE_COST;
                currencyText.text = "Coins: " + playerCoins;

                // Save purchase in PlayFab
                PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string> { { recipeName, "1" } }
                }, updateResult =>
                {
                    Debug.Log($"? {recipeName} purchased!");
                    button.GetComponentInChildren<TMP_Text>().text = "Bought";
                   


                }, error =>
                {
                    Debug.LogError($"? Error saving data: {error.GenerateErrorReport()}");
                    button.interactable = true; // Re-enable button if error occurs
                    button.GetComponentInChildren<TMP_Text>().text = "Buy";
                });

            }, subError =>
            {
                Debug.LogError("? Error deducting coins: " + subError.GenerateErrorReport());
                button.interactable = true; // Re-enable button if error occurs
                button.GetComponentInChildren<TMP_Text>().text = "Buy";
            });

        }, error =>
        {
            Debug.LogError("? Failed to fetch updated inventory: " + error.GenerateErrorReport());
            button.interactable = true; // Re-enable button if fetch fails
            button.GetComponentInChildren<TMP_Text>().text = "Buy";
        });
    }


    // SECTION SWITCHING METHODS
    public void ShowFoodSection()
    {
        foodSection.SetActive(true);
        drinkSection.SetActive(false);
        decorSection.SetActive(false);
    }

    public void ShowDrinkSection()
    {
        foodSection.SetActive(false);
        drinkSection.SetActive(true);
        decorSection.SetActive(false);
    }

    public void ShowDecorSection()
    {
        foodSection.SetActive(false);
        drinkSection.SetActive(false);
        decorSection.SetActive(true);
    }
  
    }
