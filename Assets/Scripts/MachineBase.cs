using UnityEngine;

public class MachineBase : MonoBehaviour
{
    private MachineData machineData;

    private float timer;
    private int storedAmount;
    private Bank bank;

    [SerializeField] private float yOffset = 1.5f;
    [SerializeField] private float zOffset = 1.5f;


    public void Initialise(MachineData machineData)
    {
        this.machineData = machineData;
    }

    private void Start()
    {
        bank = FindFirstObjectByType<Bank>();
    }

    private void Update()
    {
        if (machineData == null) return;

        timer += Time.deltaTime;

        if (timer >= machineData.ProductionInterval)
        {
            timer = 0f;
            Produce();
        }
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

        Vector3 spawnPos = transform.position + Random.insideUnitSphere * 0.5f;

        // Ensure the resource spawns above the ground and towards randomly
        spawnPos.y = Mathf.Max(spawnPos.y, yOffset); // Ensure it spawns above the ground
        // Ensure it spawns 2 units away from the machine towards the bank
        Vector3 directionToBank = (bank.transform.position - transform.position).normalized;
        spawnPos += directionToBank * zOffset;

        Instantiate(prefab, spawnPos, Quaternion.identity);

    }

    public int Collect()
    {
        int amount = storedAmount;
        storedAmount = 0;
        return amount;
    }

    public ResourceType GetOutputType()
    {
        return machineData.OutputType;
    }
}