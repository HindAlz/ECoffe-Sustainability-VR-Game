using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string SavePath => Application.persistentDataPath + "/save.json";
    static bool saving = false;
    public static void Save(PlayerData data)
    {
        if (!saving)
        {
            saving = true;
            Debug.Log("saved File");
            string json = JsonUtility.ToJson(data, true);
            Debug.Log(json);
            File.WriteAllText(SavePath, json);
            saving = false;
        }
    }

    public static PlayerData Load()
    {
        if (File.Exists(SavePath))
        {
            try
            {
                Debug.Log("file: ");
                string json = File.ReadAllText(SavePath);
                Debug.Log(json);

                return JsonUtility.FromJson<PlayerData>(json);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Failed to load save file: " + ex.Message);
                return new PlayerData(); // Fallback to defaults
            }
        }
        else
        {
            Debug.LogWarning("Save file not found. Creating default data.");
            return new PlayerData(); // Defaults if no file
        }
    }

    public static void Erase()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("Save file erased.");
        }
        else
        {
            Debug.Log("No save file to erase.");
        }
    }



}
