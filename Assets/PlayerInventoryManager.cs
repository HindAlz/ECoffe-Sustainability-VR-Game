using System;
using System.Collections.Generic;
using System.Collections; // Add this for coroutine
using UnityEngine;


public class PlayerInventoryManager : MonoBehaviour
{
    public bool Done = false;
    public List<string> possibleRecipes = new List<string>
    {
        "OJ", "Lemonade", "Latte", "RTea", "GTea", "Iced_Latte", "Iced_Tea", "Iced_Green_Tea", "Americano",
        "Croissant", "SJam", "OJam", "EggT", "CherryT", "ChocC", "BlueberryC",
        "CherryCup", "ChocCup", "OreoCup", "RedCup", "BwDon", "PinkDon", "WhiteDon", "Cookie"
    };

    public List<string> possibleDecor = new List<string>
    {
        "LTable", "RTable", "LChairs", "RChairs", "ChessBoard", "Candle",
        "Chandeleir", "Board", "Curtain"
    };
    public List<string> unlockedRecipeNames = new List<string>();

    // GameObject references for tools
    public GameObject Juicer, Oranges, IcedCups, Lemons, Mug, Milk, TeaCup, TeaPot,
                      RTea, GTea, Ice, Toast, SJam, Spoon, Microwave, OJam, Eggs, Pan,
                      Tomato, Basil, Cheese, Croissant, ChocC, BlueberryC, CherryCup,
                      ChocCup, OreoCup, RedCup, BwDon, PinkDon, WhiteDon, RSpawn, GSpawn, LatteSpawn, Cookie;

    // GameObject references for decor
    public GameObject LTable, RTable, LChairs, RChairs, ChessBoard, Candle,
                      Chandelier, Board, Curtain;

    private Dictionary<string, int> playerItemUsage = new Dictionary<string, int>();
    public Dictionary<string, bool> playerRecipes = new Dictionary<string, bool>();
    private List<string> activeTools = new List<string>();
    private List<string> activeDecor = new List<string>();
    // Add this with your other class variables
    private bool _hasLoadedData = false;
    public bool HasLoadedData => _hasLoadedData;
    PlayerData data;
    void Awake()
    {
        data = SaveSystem.Load();
        OnDataReceived();
    }
    private void Start()
    {

    }

    void OnDataReceived()
    {

        foreach (string recipe in data.items)
        {

            if (possibleRecipes.Contains(recipe))
            {
                playerRecipes[recipe] = true;
                SetActiveToolsForRecipe(recipe);
            }
            else if (possibleDecor.Contains(recipe))
            {
                if (!activeDecor.Contains(recipe))
                    activeDecor.Add(recipe);
            }
        }

        ActivateTools();
        ActivateDecor();
    }


    void SetActiveToolsForRecipe(string recipe)
    {
        Dictionary<string, string[]> recipeTools = new Dictionary<string, string[]>
        {
            { "OJ", new string[] { "Juicer", "Oranges", "IcedCups" } },
            { "Lemonade", new string[] { "Juicer", "Lemons", "IcedCups" } },
            { "Latte", new string[] { "Mug", "Milk" } },
            { "RTea", new string[] { "TeaCup", "TeaPot", "RTea" } },
            { "Espresso", new string[] {  } },
            { "GTea", new string[] { "TeaCup", "TeaPot", "GTea" } },
            { "Iced_Latte", new string[] { "IcedCups", "Milk", "Ice", "LatteSpawn" } },
            { "Iced_Tea", new string[] { "IcedCups", "TeaPot", "RTea", "Ice", "RSpawn" } },
            { "Iced_Green_Tea", new string[] { "IcedCups", "TeaPot", "GTea", "Ice", "GSpawn" } },
            { "Croissant", new string[] { "Croissant" } },
            { "SJam", new string[] { "Toast", "SJam", "Spoon", "Microwave" } },
            { "OJam", new string[] { "Toast", "OJam", "Spoon", "Microwave" } },
            { "EggT", new string[] { "Toast", "Eggs", "Pan", "Microwave" } },
            { "CherryT", new string[] { "Toast", "Tomato", "Basil", "Microwave", "Cheese" } },
            { "ChocC", new string[] { "ChocC" } },
            { "Cookie", new string[] { "Cookie" } },
            { "BlueberryC", new string[] { "BlueberryC" } },
            { "CherryCup", new string[] { "CherryCup" } },
            { "ChocCup", new string[] { "ChocCup" } },
            { "OreoCup", new string[] { "OreoCup" } },
            { "RedCup", new string[] { "RedCup" } },
            { "BwDon", new string[] { "BwDon" } },
            { "PinkDon", new string[] { "PinkDon" } },
            { "WhiteDon", new string[] { "WhiteDon" } }
        };

        if (recipeTools.ContainsKey(recipe))
        {
            unlockedRecipeNames.Add(recipe);
            foreach (var tool in recipeTools[recipe])
            {
                if (!activeTools.Contains(tool))
                    activeTools.Add(tool);
            }
            Debug.Log("inventory manager done");
            Done = true;
        }
    }

    void ActivateTools() 
    {
        foreach (var tool in activeTools)
        {
            Debug.Log($"Activating tool: {tool}");

            if (tool == "Juicer") Juicer.SetActive(true);
            if (tool == "Oranges") Oranges.SetActive(true);
            if (tool == "IcedCups") IcedCups.SetActive(true);
            if (tool == "Lemons") Lemons.SetActive(true);
            if (tool == "Mug") Mug.SetActive(true);
            if (tool == "Milk") Milk.SetActive(true);
            if (tool == "TeaCup") TeaCup.SetActive(true);
            if (tool == "TeaPot") TeaPot.SetActive(true);
            if (tool == "RTea") RTea.SetActive(true);
            if (tool == "GTea") GTea.SetActive(true);
            if (tool == "RSpawn") RSpawn.SetActive(true);
            if (tool == "GSpawn") GSpawn.SetActive(true);
            if (tool == "LatteSpawn") LatteSpawn.SetActive(true);
            if (tool == "Ice") Ice.SetActive(true);
            if (tool == "Toast") Toast.SetActive(true);
            if (tool == "SJam") SJam.SetActive(true);
            if (tool == "Spoon") Spoon.SetActive(true);
            if (tool == "Microwave") Microwave.SetActive(true);
            if (tool == "OJam") OJam.SetActive(true);
            if (tool == "Eggs") Eggs.SetActive(true);
            if (tool == "Pan") Pan.SetActive(true);
            if (tool == "Tomato") Tomato.SetActive(true);
            if (tool == "Basil") Basil.SetActive(true);
            if (tool == "Cheese") Cheese.SetActive(true);
            if (tool == "Croissant") Croissant.SetActive(true);
            if (tool == "ChocC") ChocC.SetActive(true);
            if (tool == "BlueberryC") BlueberryC.SetActive(true);
            if (tool == "CherryCup") CherryCup.SetActive(true);
            if (tool == "ChocCup") ChocCup.SetActive(true);
            if (tool == "OreoCup") OreoCup.SetActive(true);
            if (tool == "RedCup") RedCup.SetActive(true);
            if (tool == "BwDon") BwDon.SetActive(true);
            if (tool == "PinkDon") PinkDon.SetActive(true);
            if (tool == "WhiteDon") WhiteDon.SetActive(true);
            if (tool == "Cookie") Cookie.SetActive(true);

        }
    }

    void ActivateDecor()
    {
        foreach (var decor in activeDecor)
        {
            Debug.Log($"Activating decor: {decor}");

            if (decor == "LTable") LTable.SetActive(true);
            if (decor == "RTable") RTable.SetActive(true);
            if (decor == "LChairs") LChairs.SetActive(true);
            if (decor == "RChairs") RChairs.SetActive(true);
            if (decor == "ChessBoard") ChessBoard.SetActive(true);
            if (decor == "Candle") Candle.SetActive(true);
            if (decor == "Chandeleir") Chandelier.SetActive(true);
            if (decor == "Board") Board.SetActive(true);
            if (decor == "Curtain") Curtain.SetActive(true);
        }
    }

        
    public int GetItemUsage(string itemKey)
    {
        if (playerItemUsage.ContainsKey(itemKey))
        {
            return playerItemUsage[itemKey];
        }
        return 0;
    }

    
}
