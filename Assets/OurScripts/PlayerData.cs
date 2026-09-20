using System.Collections.Generic; 

[System.Serializable]
public class PlayerData
{
    public int Day = 1;
    public List<string> items = new List<string> { "RTea", "EggT", "Espresso" };
    public int Coins = 10;
    public int susRating = 0;
    public int dayScore=0;
}

