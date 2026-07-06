using System.Collections.Generic;
using UnityEngine;

public class BuildMenuUI : MonoBehaviour
{
    public static BuildMenuUI Instance { get; private set; }

    [SerializeField] private GameObject panel;

    private BuildArea currentBuildArea;
    private List<MachineData> currentMachines;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
        Hide();
    }

    public void Show(BuildArea buildArea, List<MachineData> machines)
    {
        currentBuildArea = buildArea;
        panel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Later:
        // Clear old buttons
        // Create one button for each MachineData
    }

    public void Hide()
    {
        currentBuildArea = null;
        panel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void BuildSelectedMachine(MachineData machineData)
    {
        if (currentBuildArea == null) { return; } 
        currentBuildArea.BuildMachine(machineData);
        Hide();
    }
}