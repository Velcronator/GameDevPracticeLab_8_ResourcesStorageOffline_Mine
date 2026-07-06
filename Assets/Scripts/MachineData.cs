using UnityEngine;

[CreateAssetMenu(menuName = "IdleGame/Machine Data")]
public class MachineData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private MachineType machineName;
    //[SerializeField] private Sprite icon;

    [Header("Prefab")]
    [SerializeField] private GameObject prefab;

    [Header("Production")]
    [SerializeField] private ResourceType outputType;
    [SerializeField] private float productionInterval = 1f;
    [SerializeField] private int amountPerTick = 1;

    public MachineType MachineName => machineName;
    // public Sprite Icon => icon;
    public GameObject Prefab => prefab;
    public ResourceType OutputType => outputType;
    public float ProductionInterval => productionInterval;
    public int AmountPerTick => amountPerTick;
}