public interface ISaveable
{
    void PopulateSaveData(GameData data);
    void LoadFromSaveData(GameData data);
}