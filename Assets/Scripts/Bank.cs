using System;
using System.Collections.Generic;
using UnityEngine;

public class Bank : MonoBehaviour, IInteractable, ISaveable
{
    public static Bank Instance { get; private set; }

    public static event Action<ResourceType, int> OnBankChanged;

    private readonly Dictionary<ResourceType, int> resources = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

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

    public void Interact(PlayerInteractionSystem interactor)
    {
        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();

        if (inventory == null)
            return;

        DepositAll(inventory);
    }

    public void DepositAll(PlayerInventory inventory)
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            int amount = inventory.RemoveAll(type);

            if (amount > 0)
            {
                Deposit(type, amount);
            }
        }
    }

    public void Deposit(ResourceType type, int amount)
    {
        if (amount <= 0)
            return;

        resources[type] += amount;

        OnBankChanged?.Invoke(type, resources[type]);
    }

    public int GetAmount(ResourceType type)
    {
        return resources[type];
    }

    public bool HasEnough(ResourceType type, int amount)
    {
        return resources[type] >= amount;
    }

    public bool Spend(ResourceType type, int amount)
    {
        if (!HasEnough(type, amount))
            return false;

        resources[type] -= amount;

        OnBankChanged?.Invoke(type, resources[type]);

        return true;
    }

    public void Spend(IReadOnlyList<ResourceCost> costs)
    {
        foreach (ResourceCost cost in costs)
        {
            Spend(cost.resourceType, cost.amount);
        }
    }

    public bool CanAfford(IReadOnlyList<ResourceCost> costs)
    {
        foreach (ResourceCost cost in costs)
        {
            if (!HasEnough(cost.resourceType, cost.amount))
                return false;
        }
        return true;
    }

    public bool TrySpend(IReadOnlyList<ResourceCost> costs)
    {
        if (!CanAfford(costs))
            return false;

        Spend(costs);

        return true;
    }

    public void PrintContents()
    {
        foreach (var pair in resources)
        {
            Debug.Log($"{pair.Key}: {pair.Value}");
        }
    }

    public void PopulateSaveData(GameData data)
    {
        data.bankResources.Clear();
        foreach (KeyValuePair<ResourceType, int> kvp in resources)
        {
            ResourceSaveData resourceData = new ResourceSaveData
            {
                type = kvp.Key,
                amount = kvp.Value
            };
            data.bankResources.Add(resourceData);
        }
    }

    public void LoadFromSaveData(GameData data)
    {
        // 1. Wipe the current runtime balances and guarantee all types exist
        InitialiseResources();

        // 2. Repopulate your runtime dictionary from the loaded list
        foreach (ResourceSaveData savedResource in data.bankResources)
        {
            resources[savedResource.type] = savedResource.amount;
        }

        // 3. UI Update Trigger: Notify listeners about the new balance for every resource
        foreach (KeyValuePair<ResourceType, int> kvp in resources)
        {
            OnBankChanged?.Invoke(kvp.Key, kvp.Value);
        }
    }
}