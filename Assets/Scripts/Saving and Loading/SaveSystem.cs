using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    // Cap offline time so players can't abuse extended sessions
    private const double MaxOfflineSeconds = 8 * 3600; // 8 hours

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

        // 2. Stamp the current UTC time for offline progression calculations on next load
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

            // 3. Credit resources earned while the game was closed
            ApplyOfflineProgression(data);

            Debug.Log("Game successfully loaded.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load game: {e.Message}");
        }
    }

    private void ApplyOfflineProgression(GameData data)
    {
        if (string.IsNullOrEmpty(data.saveTimeUtc))
            return;

        if (!DateTime.TryParse(data.saveTimeUtc, null,
                DateTimeStyles.RoundtripKind, out DateTime lastSaveUtc))
        {
            Debug.LogWarning("[Offline] Could not parse saveTimeUtc. Skipping offline progression.");
            return;
        }

        double rawSeconds = (DateTime.UtcNow - lastSaveUtc).TotalSeconds;
        double offlineSeconds = Math.Min(rawSeconds, MaxOfflineSeconds);

        if (offlineSeconds <= 0)
            return;

        string rawFormatted = TimeSpan.FromSeconds(rawSeconds).ToString(@"hh\:mm\:ss");
        string capFormatted = TimeSpan.FromSeconds(offlineSeconds).ToString(@"hh\:mm\:ss");
        Debug.Log($"[Offline] Away for {rawFormatted}. Applying {capFormatted} of progression.");

        MachineManager.Instance?.ApplyOfflineProgression(offlineSeconds);
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