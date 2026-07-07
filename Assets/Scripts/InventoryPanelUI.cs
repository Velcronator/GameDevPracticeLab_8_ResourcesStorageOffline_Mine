using System.Collections;
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

    // Track the active coroutine so we can reset it
    private Coroutine autoHideCoroutine;
    private readonly WaitForSeconds panelDisplayDelay = new WaitForSeconds(3f);

    private void Awake()
    {
        textFields = new Dictionary<ResourceType, TextMeshProUGUI>
        {
            { ResourceType.Stone, stoneAmount },
            { ResourceType.Wood, woodAmount },
            { ResourceType.Gold, goldAmount }
        };

        // Ensure the panel starts hidden when the game begins
        ShowHideVisualPanel(false);
    }

    private void OnEnable()
    {
        PlayerInventory.OnResourceAmountChanged += UpdateResourceUI;
    }

    private void OnDisable()
    {
        PlayerInventory.OnResourceAmountChanged -= UpdateResourceUI;

        // Safety check: stop the coroutine if the whole UI object gets disabled
        if (autoHideCoroutine != null)
        {
            StopCoroutine(autoHideCoroutine);
            autoHideCoroutine = null;
        }
    }

    private void UpdateResourceUI(ResourceType type, int newAmount)
    {
        if (textFields.TryGetValue(type, out TextMeshProUGUI targetText))
        {
            if (targetText != null)
            {
                targetText.text = newAmount.ToString();

                // Trigger the temporary visibility loop
                TriggerPanelFlash();
            }
        }
    }

    private void TriggerPanelFlash()
    {
        // If the panel is already counting down to close, cancel it
        if (autoHideCoroutine != null)
        {
            StopCoroutine(autoHideCoroutine);
        }

        // Start a fresh 3-second countdown
        autoHideCoroutine = StartCoroutine(FlashPanelRoutine());
    }

    private IEnumerator FlashPanelRoutine()
    {
        ShowHideVisualPanel(true);

        yield return panelDisplayDelay;

        ShowHideVisualPanel(false);
        autoHideCoroutine = null;
    }

    private void ShowHideVisualPanel(bool show)
    {
        if (inventoryVisualUGUI != null)
        {
            inventoryVisualUGUI.SetActive(show);
        }
    }
}