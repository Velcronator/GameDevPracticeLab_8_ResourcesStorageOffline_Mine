using System.Collections.Generic;
using UnityEngine;

public class MachineManager : MonoBehaviour, ISaveable
{
    public static MachineManager Instance { get; private set; }

    private readonly List<MachineBase> machines = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        SaveSystem.Instance?.Register(this);
    }

    private void OnDisable()
    {
        SaveSystem.Instance?.Unregister(this);
    }

    public void Register(MachineBase machine)
    {
        if (!machines.Contains(machine))
        {
            machines.Add(machine);
        }
    }

    public void Unregister(MachineBase machine)
    {
        machines.Remove(machine);
    }

    public IReadOnlyList<MachineBase> GetMachines()
    {
        return machines;
    }

    public IEnumerable<MachineBase> GetMachines(ResourceType type)
    {
        foreach (MachineBase machine in machines)
        {
            if (machine.MachineData.OutputType == type)
                yield return machine;
        }
    }

    public MachineBase SpawnMachine(MachineData data, Vector3 position, Quaternion rotation)
    {
        GameObject obj = Instantiate(data.Prefab, position, rotation);

        MachineBase machine = obj.GetComponent<MachineBase>();

        if (machine == null)
        {
            Debug.LogError("Machine prefab missing MachineBase component.");

            Destroy(obj);
            return null;
        }

        machine.Initialise(data);

        return machine;
    }

    public void RemoveMachine(MachineBase machine)
    {
        if (!machines.Contains(machine))
            return;

        machines.Remove(machine);

        Destroy(machine.gameObject);
    }

    public void ClearMachines()
    {
        foreach (MachineBase machine in machines)
        {
            Destroy(machine.gameObject);
        }

        machines.Clear();
    }

    public int GetMachineCount()
    {
        return machines.Count;
    }

    public void PopulateSaveData(GameData data)
    {
        data.machines.Clear();

        foreach (MachineBase machine in machines)
        {
            MachineSaveData save = new MachineSaveData
            {
                machineId = machine.MachineId,
                position = machine.transform.position,
                rotation = machine.transform.rotation
            };

            data.machines.Add(save);
        }
    }

    public void LoadFromSaveData(GameData data)
    {
        ClearMachines();

        foreach (MachineSaveData save in data.machines)
        {
            MachineData machineData = MachineDatabase.Instance.GetMachine(save.machineId);

            SpawnMachine(machineData, save.position, save.rotation);
        }
    }
}
