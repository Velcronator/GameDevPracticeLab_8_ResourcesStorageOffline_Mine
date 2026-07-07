using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "IdleGame/Machine Data")]
public class MachineData : ScriptableObject
{
    [Header("Identity")]
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

    public IReadOnlyList<ResourceCost> BuildCosts => buildCosts;

    public MachineType MachineName => machineName;
    public Sprite Icon => icon;
    public GameObject Prefab => prefab;
    public ResourceType OutputType => outputType;
    public float ProductionInterval => productionInterval;
    public int AmountPerTick => amountPerTick;
}
