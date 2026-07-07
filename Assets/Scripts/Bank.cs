using System;
using System.Collections.Generic;
using UnityEngine;

public class Bank : MonoBehaviour, IInteractable
{
    public static event Action<ResourceType, int> OnBankChanged;

    private readonly Dictionary<ResourceType, int> resources = new();

    private void Awake()
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            resources[type] = 0;
        }
    }

    public void Interact(PlayerInteractionSystem interactor)
    {
        PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();

        if (inventory == null)
        {
            Debug.LogWarning("No PlayerInventory found.");
            return;
        }

        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            int amount = inventory.RemoveAll(type);

            Deposit(type, amount);

            Debug.Log($"Deposited {amount} {type}");
        }

        Debug.Log("Deposited all resources.");
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
}