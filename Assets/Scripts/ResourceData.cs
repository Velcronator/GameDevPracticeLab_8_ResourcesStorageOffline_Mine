using UnityEngine;

[CreateAssetMenu(menuName = "IdleGame/Resource Data")]
public class ResourceData : ScriptableObject
{
    public ResourceType type;
    public GameObject prefab;
    //public Sprite icon;
}