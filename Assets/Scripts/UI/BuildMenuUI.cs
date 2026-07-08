using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildMenuUI : MonoBehaviour
{
    public static BuildMenuUI Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private BuildButtonUI buttonPrefab;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bankResourcesText;

    private BuildArea currentBuildArea;


    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    private void UpdateBankResources()
    {
        titleText.text = "Build Machine";

        System.Text.StringBuilder builder = new();

        builder.AppendLine("In the bank you have: ");

        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            builder.AppendLine($"{type}: {Bank.Instance.GetAmount(type)}");
        }

        bankResourcesText.text = builder.ToString();
    }

    public void Show(BuildArea buildArea, List<MachineData> machines)
    {
        currentBuildArea = buildArea;

        UpdateBankResources();

        ClearButtons();

        foreach (MachineData machine in machines)
        {
            BuildButtonUI button = Instantiate(buttonPrefab, buttonContainer);

            button.Setup(machine);
        }


        panel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    private void ClearButtons()
    {
        for (int i = buttonContainer.childCount - 1; i >= 1; i--)
        {
            Destroy(buttonContainer.GetChild(i).gameObject);
        }
    }


    public void BuildSelectedMachine(MachineData machineData)
    {
        if (currentBuildArea == null)
            return;


        if (currentBuildArea.BuildMachine(machineData))
        {
            Hide();
        }
    }


    public void Hide()
    {
        currentBuildArea = null;

        panel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}