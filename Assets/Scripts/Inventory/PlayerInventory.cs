using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static event Action<ResourceType, int> OnResourceAmountChanged;

    [SerializeField] private int maxPerResource = 9;

    private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();
    private PlayerThirdPersonVisual playerThirdPersonVisual;

    private void Awake()
    {
        playerThirdPersonVisual = GetComponent<PlayerThirdPersonVisual>();

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

    public List<ResourceSaveData> GetSaveData()
    {
        return null; // TODO
    }

    public void LoadData(List<ResourceSaveData> data)
    {
        // TODO
    }
}
