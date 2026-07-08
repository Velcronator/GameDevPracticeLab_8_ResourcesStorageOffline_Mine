using System.Collections.Generic;
using UnityEngine;

public class MachineDatabase : MonoBehaviour
{
    public static MachineDatabase Instance { get; private set; }

    [SerializeField] private List<MachineData> machines = new();


    private Dictionary<string, MachineData> machineLookup;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        BuildLookup();
    }


    private void BuildLookup()
    {
        machineLookup = new Dictionary<string, MachineData>();

        foreach (MachineData machine in machines)
        {
            if (machineLookup.ContainsKey(machine.MachineId))
            {
                Debug.LogError($"Duplicate Machine ID found: {machine.MachineId}");

                continue;
            }

            machineLookup.Add(machine.MachineId, machine);
        }
    }


    public MachineData GetMachine(string id)
    {
        if (machineLookup.TryGetValue(id, out MachineData machine))
        {
            return machine;
        }

        Debug.LogError($"Machine with ID '{id}' was not found.");

        return null;
    }
}