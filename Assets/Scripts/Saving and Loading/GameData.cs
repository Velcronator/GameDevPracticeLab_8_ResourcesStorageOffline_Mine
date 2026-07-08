using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    public List<ResourceSaveData> bankResources = new();
    public List<ResourceSaveData> inventoryResources = new();

    public List<MachineSaveData> machines = new();

    public string saveTimeUtc;

    public int version = 1;
}