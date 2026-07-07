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

    private void OnEnable() => Bank.OnBankChanged += UpdateBankUI;
    private void OnDisable() => Bank.OnBankChanged -= UpdateBankUI;

    private void UpdateBankUI(ResourceType type, int newAmount)
    {
        if (textFields.TryGetValue(type, out TextMeshProUGUI targetText) && targetText != null)
        {
            targetText.text = newAmount.ToString();

            // Hand control off to the manager with High priority
            UIPanelManager.Instance.RequestShowPanel(bankVisualUGUI, UIPanelPriority.High);
        }
    }
}