using System.Collections.Generic;
using UnityEngine;

public class ResourceDatabase : MonoBehaviour
{
    public static ResourceDatabase Instance { get; private set; }

    [SerializeField] private List<ResourceData> resources;

    private Dictionary<ResourceType, ResourceData> lookup;

    private void Awake()
    {
        Instance = this;

        lookup = new Dictionary<ResourceType, ResourceData>();

        foreach (var res in resources)
        {
            lookup[res.type] = res;
        }
    }

    public GameObject GetPrefab(ResourceType type)
    {
        return lookup[type].prefab;
    }
}