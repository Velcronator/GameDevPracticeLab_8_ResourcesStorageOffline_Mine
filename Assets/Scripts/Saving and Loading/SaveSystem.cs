using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    private string saveFilePath;
    private List<ISaveable> saveableObjects = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        //saveFilePath = Path.Combine(Application.persistentDataPath, "savegame.json");
        //TODO while testing and Debugging
        string folderPath = Path.Combine(Application.dataPath, "Saves");
        saveFilePath = Path.Combine(folderPath, "savegame.json");

    }

    private IEnumerator Start()
    {
        // wait one frame so Bank/Inventory/MachineManager can Register(this)
        yield return null;
        LoadGame();
    }

    public void Register(ISaveable saveable)
    {
        if (!saveableObjects.Contains(saveable))
        {
            saveableObjects.Add(saveable);
        }
    }

    public void Unregister(ISaveable saveable)
    {
        saveableObjects.Remove(saveable);
    }

    private void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");
        SaveGame();
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            Debug.Log("OnApplicationPause");
            SaveGame();
        }
    }

    public void SaveGame()
    {
        GameData data = new GameData();

        // 1. Let every system write its own data into the GameData container
        foreach (ISaveable saveable in saveableObjects)
        {
            saveable.PopulateSaveData(data);
        }

        // 2. Add a timestamp for offline calculations later
        data.saveTimeUtc = DateTime.UtcNow.ToString("O");

        // 3. Serialize to JSON and write to disk
        try
        {
            string json = JsonUtility.ToJson(data, true); // true = Pretty print
            File.WriteAllText(saveFilePath, json);
            Debug.Log($"Game successfully saved to: {saveFilePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save game: {e.Message}");
        }
    }

    public void LoadGame()
    {
        if (!HasSave())
        {
            Debug.LogWarning("No save file found to load.");
            return;
        }

        try
        {
            // 1. Read JSON from disk and deserialize
            string json = File.ReadAllText(saveFilePath);
            GameData data = JsonUtility.FromJson<GameData>(json);
            if (data == null)
            {
                Debug.LogError("Save file is invalid.");
                return;
            }

            // 2. Hand the data to every system to restore its state
            foreach (ISaveable saveable in saveableObjects)
            {
                saveable.LoadFromSaveData(data);
            }

            Debug.Log("Game successfully loaded.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load game: {e.Message}");
        }
    }

    public bool HasSave()
    {
        return File.Exists(saveFilePath);
    }

    public void DeleteSave()
    {
        if (HasSave())
        {
            File.Delete(saveFilePath);
            Debug.Log("Save file deleted.");
        }
    }
}