using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject inventoryVisualUGUI;

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

        inventoryVisualUGUI.SetActive(false);
    }

    private void OnEnable() => PlayerInventory.OnResourceAmountChanged += UpdateResourceUI;
    private void OnDisable() => PlayerInventory.OnResourceAmountChanged -= UpdateResourceUI;

    private void UpdateResourceUI(ResourceType type, int newAmount)
    {
        if (textFields.TryGetValue(type, out TextMeshProUGUI targetText) && targetText != null)
        {
            targetText.text = newAmount.ToString();

            // Hand control off to the manager with Low priority
            UIPanelManager.Instance.RequestShowPanel(inventoryVisualUGUI, UIPanelPriority.Low);
        }
    }
}