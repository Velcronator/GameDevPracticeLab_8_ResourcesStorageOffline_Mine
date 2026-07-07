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
        foreach (GameObject go in visuals)
        {
            go.SetActive(false);
        }
        BuildMenuUI.Instance.Show(this, availableMachines);
    }



    private IEnumerator BuildMachineCoroutine(MachineData machineData)
    {
        GameObject obj = Instantiate(machineData.Prefab, machineSpawnPoint.position, machineSpawnPoint.rotation);

        MachineBase machine = obj.GetComponent<MachineBase>();
        machine.Initialise(machineData);
        boxCollider.enabled = false;
        yield return buildDelay;
        hasMachineBuilt = true;
    }

    public void BuildMachine(MachineData machineData)
    {
        StartCoroutine(BuildMachineCoroutine(machineData));
    }
}