using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI machineNameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image icon;
    [SerializeField] private Button button;

    private MachineData machineData;


    public void Setup(MachineData data)
    {
        machineData = data;

        // Remove any existing listeners first to prevent double-firing bugs, then add it
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(BuildPressed);

        machineNameText.text = data.MachineName.ToString();

        if (data.Icon != null)
        {
            icon.sprite = data.Icon;
        }

        costText.text = GetCostText();

        button.interactable = Bank.Instance.CanAfford(data.BuildCosts);
    }


    private string GetCostText()
    {
        string text = "";

        foreach (ResourceCost cost in machineData.BuildCosts)
        {
            text += $"{cost.resourceType}: {cost.amount}\n";
        }

        return text;
    }


    public void BuildPressed()
    {
        BuildMenuUI.Instance.BuildSelectedMachine(machineData);
    }
}