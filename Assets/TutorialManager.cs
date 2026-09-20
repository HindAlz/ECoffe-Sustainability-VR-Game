using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class TutorialManager : MonoBehaviour
{

    private PlayerInventoryManager tutorialManager;

    public GameObject foodSection;
    public GameObject drinkSection;
    public List<Button> foodButtons;
    public List<Button> drinkButtons;
    public GameObject firstPage;
    public GameObject secondPage;

    private Dictionary<string, Button> recipeButtonMap = new Dictionary<string, Button>();
    private int playerCoins = 0;
    private const int RECIPE_COST = 10;

    public List<string> possibleRecipes = new List<string>
    {
        "OJ", "Lemonade", "Latte", "RTea", "GTea", "Iced_Latte", "Iced_Tea", "Iced_Green_Tea", "Americano",
        "Croissant", "SJam", "OJam", "EggT", "CherryT", "ChocC", "BlueberryC",
        "CherryCup", "ChocCup", "OreoCup", "RedCup", "BwDon", "PinkDon", "WhiteDon", "Espresso"
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
        foreach (string recipe in possibleRecipes)
        {
            GameObject panel = infoPanels.Find(p => p.name == recipe);
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

        

        tutorialManager = FindObjectOfType<PlayerInventoryManager>();
        InitializeRecipeButtons();
        FetchPurchasedRecipes();
    }
    void InitializeRecipeButtons()
    {
        InitializeCategoryButtons(foodButtons, "Food");
        InitializeCategoryButtons(drinkButtons, "Drink");
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

        }
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

    IEnumerator DelayedFetch()
    {
        yield return new WaitForSeconds(1f);
        FetchPurchasedRecipes();
    }

    void FetchPurchasedRecipes()
    {
        HashSet<string> purchasedItems = new HashSet<string>(data.items);

        foreach (var entry in recipeButtonMap)
        {
            string itemName = entry.Key;
            Button button = entry.Value;

            if (button == null) continue;

            if (purchasedItems.Contains(itemName))
            {
                button.gameObject.SetActive(true);
                TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                {
                    button.interactable = false;
                    buttonText.text = "Bought";
                }
            }
            else
            {
                button.gameObject.SetActive(false); // Hide button if not purchased
            }
        }
    }


    // SECTION SWITCHING METHODS
    public void ShowFoodSection()
    {
        foodSection.SetActive(true);
        drinkSection.SetActive(false);
    }

    public void ShowDrinkSection()
    {
        foodSection.SetActive(false);
        drinkSection.SetActive(true);
    }



}
