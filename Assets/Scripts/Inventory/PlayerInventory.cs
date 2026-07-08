using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour, ISaveable
{
    public static event Action<ResourceType, int> OnResourceAmountChanged;

    [SerializeField] private int maxPerResource = 9;

    private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();
    private PlayerThirdPersonVisual playerThirdPersonVisual;

    private void Awake()
    {
        playerThirdPersonVisual = GetComponent<PlayerThirdPersonVisual>();

        InitialiseResources();
    }
    private void OnEnable()
    {
        SaveSystem.Instance?.Register(this);
    }

    private void OnDisable()
    {
        SaveSystem.Instance?.Unregister(this);
    }

    private void InitialiseResources()
    {
        resources.Clear();
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            resources[type] = 0;
        }
    }

    public void AddResource(ResourceType type, int amount)
    {
        if (amount <= 0) return;

        resources[type] += amount;

        playerThirdPersonVisual?.PlayPickupEffect();

        OnResourceAmountChanged?.Invoke(type, resources[type]);
    }

    public bool RemoveResource(ResourceType type, int amount)
    {
        if (!HasEnoughResource(type, amount)) return false;

        resources[type] -= amount;
        OnResourceAmountChanged?.Invoke(type, resources[type]);
        return true;
    }

    public bool HasEnoughResource(ResourceType type, int amount)
    {
        if (resources.TryGetValue(type, out int currentAmount))
        {
            return currentAmount >= amount;
        }
        return false;
    }

    public int GetResourceAmount(ResourceType type)
    {
        return resources.TryGetValue(type, out int amount) ? amount : 0;
    }

    public int RemoveAll(ResourceType type)
    {
        int amount = GetResourceAmount(type);

        if (amount <= 0)
            return 0;

        resources[type] = 0;

        OnResourceAmountChanged.Invoke(type, 0);

        return amount;
    }

    public void Clear()
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            resources[type] = 0;
            OnResourceAmountChanged?.Invoke(type, 0);
        }
    }

    public void PopulateSaveData(GameData data)
    {
        data.inventoryResources.Clear();
        foreach (KeyValuePair<ResourceType, int> kvp in resources)
        {
            ResourceSaveData resourceData = new ResourceSaveData
            {
                type = kvp.Key,
                amount = kvp.Value
            };
            data.inventoryResources.Add(resourceData);
        }
    }

    public void LoadFromSaveData(GameData data)
    {
        // 1. Wipe the current runtime balances and guarantee all types exist
        InitialiseResources();

        // 2. Repopulate your runtime dictionary from the loaded list
        foreach (ResourceSaveData savedResource in data.inventoryResources)
        {
            resources[savedResource.type] = savedResource.amount;
        }

        // 3. UI Update Trigger: Notify listeners about the new balance for every resource
        foreach (KeyValuePair<ResourceType, int> kvp in resources)
        {
            OnResourceAmountChanged?.Invoke(kvp.Key, kvp.Value);
        }
    }
}
