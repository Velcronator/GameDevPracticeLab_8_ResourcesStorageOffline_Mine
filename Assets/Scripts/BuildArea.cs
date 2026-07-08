using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildArea : MonoBehaviour, IInteractable, ISaveable
{
    [Header("Build Settings")]
    [SerializeField] private List<MachineData> availableMachines;
    [SerializeField] private Transform machineSpawnPoint;
    [SerializeField] private GameObject[] visuals;

    [Header("Save Settings")]
    [SerializeField] private string buildAreaId; 

    private BoxCollider boxCollider;
    private readonly WaitForSeconds buildDelay = new(3.0f);
    private bool hasMachineBuilt;

    private Coroutine registerRoutine;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnEnable()
    {
        registerRoutine = StartCoroutine(RegisterWhenSaveSystemReady());
    }

    private IEnumerator RegisterWhenSaveSystemReady()
    {
        while (SaveSystem.Instance == null)
            yield return null;

        SaveSystem.Instance.Register(this);

        if (string.IsNullOrWhiteSpace(buildAreaId))
        {
            Debug.LogError($"{name}: buildAreaId is empty. This BuildArea will not be saved.");
        }
    }

    private void OnDisable()
    {
        if (registerRoutine != null)
        {
            StopCoroutine(registerRoutine);
            registerRoutine = null;
        }

        if (SaveSystem.Instance != null)
            SaveSystem.Instance.Unregister(this);
    }

    public void Interact(PlayerInteractionSystem interactor)
    {
        if (!hasMachineBuilt)
        {
            Build();
        }
    }

    private void Build()
    {
        BuildMenuUI.Instance.Show(this, availableMachines);
    }

    private IEnumerator BuildMachineCoroutine(MachineData machineData)
    {
        GameObject obj = Instantiate(machineData.Prefab, machineSpawnPoint.position, machineSpawnPoint.rotation);
        MachineBase machine = obj.GetComponent<MachineBase>();
        machine.Initialise(machineData);

        yield return buildDelay;
    }

    public bool BuildMachine(MachineData machineData)
    {
        if (!Bank.Instance.TrySpend(machineData.BuildCosts))
        {
            Debug.Log("Not enough resources.");
            return false;
        }

        SetBuiltState(true); // mark immediately so save is consistent
        StartCoroutine(BuildMachineCoroutine(machineData));
        return true;
    }

    private void SetBuiltState(bool built)
    {
        hasMachineBuilt = built;
        boxCollider.enabled = !built;

        foreach (GameObject go in visuals)
        {
            go.SetActive(!built);
        }
    }

    public void PopulateSaveData(GameData data)
    {
        if (string.IsNullOrWhiteSpace(buildAreaId))
        {
            Debug.LogWarning($"{name} has no buildAreaId. It will not be saved.");
            return;
        }

        data.buildAreas.RemoveAll(x => x.buildAreaId == buildAreaId);
        data.buildAreas.Add(new BuildAreaSaveData
        {
            buildAreaId = buildAreaId,
            hasMachineBuilt = hasMachineBuilt
        });
    }

    public void LoadFromSaveData(GameData data)
    {
        if (string.IsNullOrWhiteSpace(buildAreaId))
            return;

        BuildAreaSaveData saved = data.buildAreas.Find(x => x.buildAreaId == buildAreaId);
        if (saved != null)
        {
            SetBuiltState(saved.hasMachineBuilt);
        }
    }
}