using UnityEngine;

public class MachineBase : MonoBehaviour
{
    [SerializeField] private Transform resourceSpawnPoint;

    private MachineData machineData;
    private float timer;
    
    public MachineData MachineData => machineData;
    public ResourceType OutputType => machineData.OutputType;
    public string MachineId => machineData.MachineId;
    public float ProductionInterval => machineData.ProductionInterval;



    private void Update()
    {
        if (machineData == null) return;

        timer += Time.deltaTime;

        while (timer >= machineData.ProductionInterval)
        {
            timer -= machineData.ProductionInterval;
            Produce();
        }
    }

    public void Initialise(MachineData machineData)
    {
        this.machineData = machineData;

        MachineManager.Instance.Register(this);
    }

    public void Produce(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            SpawnResource();
        }
    }

    public MachineSaveData GetSaveData()
    {
        return new MachineSaveData
        {
            machineId = MachineId,
            position = transform.position,
            rotation = transform.rotation
        };
    }

    private void Produce()
    {
        for (int i = 0; i < machineData.AmountPerTick; i++)
        {
            SpawnResource();
        }
    }

    private void SpawnResource()
    {
        GameObject prefab = ResourceDatabase.Instance.GetPrefab(machineData.OutputType);

        Instantiate(prefab, resourceSpawnPoint.position, Quaternion.identity);
    }

    private void OnDisable()
    {
        MachineManager.Instance.Unregister(this);
    }
}