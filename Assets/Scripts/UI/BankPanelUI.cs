using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BankPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject bankVisualUGUI;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI stoneAmount;
    [SerializeField] private TextMeshProUGUI woodAmount;
    [SerializeField] private TextMeshProUGUI goldAmount;

    private Dictionary<ResourceType, TextMeshProUGUI> textFields;

    private void Awake()
    {
        textFields = new Dictionary<ResourceType, TextMeshProUGUI>
        {
            { ResourceType.Stone, stoneAmount },
            { ResourceType.Wood, woodAmount },
            { ResourceType.Gold, goldAmount }
        };

        bankVisualUGUI.SetActive(false);
    }

    private void OnEnable()
    {
        Bank.OnBankChanged += OnBankResourceChanged;

        // Safety check: If the Bank already has data when this UI turns on, sync it immediately
        if (Bank.Instance != null)
        {
            UpdateAllFields();
        }
    }

    private void OnDisable()
    {
        Bank.OnBankChanged -= OnBankResourceChanged;
    }

    // Handles individual resource updates via events
    private void OnBankResourceChanged(ResourceType type, int newAmount)
    {
        if (textFields.TryGetValue(type, out TextMeshProUGUI targetText) && targetText != null)
        {
            targetText.text = newAmount.ToString();

            // Only request a UI pop-up if the game is actively running 
            // and we aren't completely hidden/suppressed by a loading screen.
            UIPanelManager.Instance.RequestShowPanel(bankVisualUGUI, UIPanelPriority.High);
        }
    }

    // Helper method to sync the entire UI data map at once without forcefully popping up the panel
    public void UpdateAllFields()
    {
        foreach (KeyValuePair<ResourceType, TextMeshProUGUI> pair in textFields)
        {
            if (pair.Value != null)
            {
                int currentAmount = Bank.Instance.GetAmount(pair.Key);
                pair.Value.text = currentAmount.ToString();
            }
        }
    }
}