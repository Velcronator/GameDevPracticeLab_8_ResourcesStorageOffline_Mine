using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    public List<ResourceSaveData> bankResources;
    public List<ResourceSaveData> inventoryResources;

    public List<BuildAreaSaveData> buildAreas;

    public string saveTimeUtc;
}