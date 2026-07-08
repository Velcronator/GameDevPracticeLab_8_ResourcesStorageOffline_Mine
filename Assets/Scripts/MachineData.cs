using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "IdleGame/Machine Data")]
public class MachineData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string machineId;
    [SerializeField] private MachineType machineName;
    [SerializeField] private Sprite icon;

    [Header("Prefab")]
    [SerializeField] private GameObject prefab;

    [Header("Production")]
    [SerializeField] private ResourceType outputType;
    [SerializeField] private float productionInterval = 1f;
    [SerializeField] private int amountPerTick = 1;

    [Header("Building Costs")]
    [SerializeField] private List<ResourceCost> buildCosts = new();



    // Public Getters
    public string MachineId => machineId;
    public MachineType MachineName => machineName;
    public Sprite Icon => icon;
    public GameObject Prefab => prefab;
    public ResourceType OutputType => outputType;
    public float ProductionInterval => productionInterval;
    public int AmountPerTick => amountPerTick;
    public IReadOnlyList<ResourceCost> BuildCosts => buildCosts;
}
