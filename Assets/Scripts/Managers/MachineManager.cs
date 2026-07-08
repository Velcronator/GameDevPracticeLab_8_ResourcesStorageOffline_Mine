using System.Collections.Generic;
using UnityEngine;

public class MachineManager : MonoBehaviour
{
    public static MachineManager Instance { get; private set; }

    private readonly List<MachineBase> machines = new();

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

    //public MachineBase SpawnMachine(MachineData data, Vector3 position, Quaternion rotation);

    //public void RemoveMachine(MachineBase machine);

    //public void ClearMachines();

    public int GetMachineCount()
    {
        return machines.Count;
    }

}
