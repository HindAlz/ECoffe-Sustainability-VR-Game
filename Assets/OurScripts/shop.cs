using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class shop : MonoBehaviour
{

    public GameObject shopUI;
    public TMP_Text currencyText;
    private PlayerInventoryManager tutorialManager;

    public GameObject foodSection;
    public GameObject drinkSection;
    public GameObject decorSection;
    public GameObject startButton;
    public List<Button> foodButtons;
    public List<Button> drinkButtons;
    public List<Button> decorButtons;

    public GameObject firstPage;
    public GameObject secondPage;
    private Dictionary<string, Button> recipeButtonMap = new Dictionary<string, Button>();
    private int playerCoins = 0;
    private const int RECIPE_COST = 10;

    public List<string> possibleRecipes = new List<string>
    {
        "OJ", "Lemonade", "Latte", "RTea", "GTea", "Iced_Latte", "Iced_Tea", "Iced_Green_Tea", "Americano",
        "Croissant", "SJam", "OJam", "EggT", "CherryT", "ChocC", "BlueberryC",
        "CherryCup", "ChocCup", "OreoCup", "RedCup", "BwDon", "PinkDon", "WhiteDon"
    };

    public List<string> possibleDecor = new List<string>
    {
        "LTable", "RTable", "LChairs", "RChairs", "ChessBoard", "Candle",
        "Chandeleir", "Board", "Curtain"
    };

    private Dictionary<string, GameObject> itemInfoPanels = new Dictionary<string, GameObject>();
    public List<GameObject> infoPanels = new List<GameObject>();
    PlayerData data;

    void Awake()
    {
        data = SaveSystem.Load();
        playerCoins = data.Coins;
    }

    private void Start()
    {
        currencyText.text = "Coins: " + playerCoins;
        foreach (string recipe in possibleRecipes)
        {
            GameObject panel = infoPanels.Find(p => p.name == recipe );
            if (panel != null)
            {
                itemInfoPanels.Add(recipe, panel);
                panel.SetActive(false);
            }
            else
            {
                Debug.LogWarning($"Info panel for {recipe} not found!");
            }
        }

        foreach (string decor in possibleDecor)
        {
            GameObject panel = infoPanels.Find(p => p.name == decor);
            if (panel != null)
            {
                itemInfoPanels.Add(decor, panel);
                panel.SetActive(false);
            }
            else
            {
                Debug.LogWarning($"Info panel for {decor} not found!");
            }
        }

        tutorialManager = FindObjectOfType<PlayerInventoryManager>();
        InitializeRecipeButtons();
        FetchPurchasedRecipes();
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

    public void BuyRecipeWrapper(string recipeName)
    {
        if (recipeButtonMap.ContainsKey(recipeName))
        {
            Button button = recipeButtonMap[recipeName];
            button.interactable = false; 
            button.GetComponentInChildren<TMP_Text>().text = "Processing";

            StartCoroutine(TryBuyRecipe(recipeName, button));
        }
        else
        {
            Debug.LogError($"? Button not found for recipe: {recipeName}");
        }
    }

    IEnumerator TryBuyRecipe(string recipeName, Button button)
    {
        yield return new WaitForSeconds(0.5f); // Small delay to avoid spam
        
            if (playerCoins < RECIPE_COST)
            {
                Debug.Log("? Not enough coins!");
                button.interactable = true; // Re-enable button if purchase fails
                button.GetComponentInChildren<TMP_Text>().text = "Buy";
                yield break;
            }

        data.Coins = playerCoins - RECIPE_COST;
        playerCoins = playerCoins - RECIPE_COST;
        currencyText.text = "Coins: " + playerCoins;
        data.items.Add(recipeName);
        Debug.Log($"? {recipeName} purchased!");
        button.GetComponentInChildren<TMP_Text>().text = "Bought";
        SaveSystem.Save(data);
    }


    public void OnRightClick()
    {
        secondPage.SetActive(false);
        firstPage.SetActive(true);

    }

    public void OnkeftClick()
    {
        secondPage.SetActive(true);
        firstPage.SetActive(false);

    }

    public void ShowItemInfoPanel(string itemName)
    {
        foreach (var panel in itemInfoPanels.Values)
        {
            panel.SetActive(false);
        }

        if (itemInfoPanels.ContainsKey(itemName))
        {
            itemInfoPanels[itemName].SetActive(true);
        }

        else
        {
            Debug.LogWarning($"No info panel mapped for {itemName}");
        }
    }


    

    public void OpenShop()
    {
        Debug.Log("?? Opening Shop...");
        shopUI.SetActive(true);
        StartCoroutine(DelayedFetch());
        startButton.SetActive(false);
        ShowFoodSection();
    }

    IEnumerator DelayedFetch()
    {
        yield return new WaitForSeconds(1f);
        FetchPurchasedRecipes();
    }



    void FetchPurchasedRecipes()
    {
        foreach (var item in data.items)
            {
                if (recipeButtonMap.ContainsKey(item))
                {
                    Button button = recipeButtonMap[item];

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
    public void CheckAndGiveStartingMoney()
    {      
    }

    public void ShowDecorSection()
    {
        foodSection.SetActive(false);
        drinkSection.SetActive(false);
        decorSection.SetActive(true);
    }

    public void ShowTitleScreen()
    {
        SceneManager.LoadScene("Game");

    }

}
