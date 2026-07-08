using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildArea : MonoBehaviour, IInteractable
{
    [Header("Build Settings")]
    [SerializeField] private List<MachineData> availableMachines;
    [SerializeField] private Transform machineSpawnPoint;

    [SerializeField] GameObject[] visuals;

    BoxCollider boxCollider;

    private readonly WaitForSeconds buildDelay = new WaitForSeconds(3.0f);

    private bool hasMachineBuilt;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    public void Interact(PlayerInteractionSystem interactor)
    {
        if (!hasMachineBuilt)
        {
            Build();
        }
    }

    void Build()
    {
        BuildMenuUI.Instance.Show(this, availableMachines);
    }



    private IEnumerator BuildMachineCoroutine(MachineData machineData)
    {
        GameObject obj = Instantiate(machineData.Prefab, machineSpawnPoint.position, machineSpawnPoint.rotation);

        MachineBase machine = obj.GetComponent<MachineBase>();
        machine.Initialise(machineData);
        
        yield return buildDelay;
        hasMachineBuilt = true;
    }

    public bool BuildMachine(MachineData machineData)
    {
        if (!Bank.Instance.TrySpend(machineData.BuildCosts))
        {
            Debug.Log("Not enough resources.");
            return false;
        }

        boxCollider.enabled = false;
        DisableVisualsForBuildArea();

        StartCoroutine(BuildMachineCoroutine(machineData));
        return true;
    }

    private void DisableVisualsForBuildArea()
    {
        foreach (GameObject go in visuals)
        {
            go.SetActive(false);
        }

    }
}