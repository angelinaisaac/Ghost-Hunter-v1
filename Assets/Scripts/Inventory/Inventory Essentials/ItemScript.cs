using UnityEngine;

//creates item instance
[CreateAssetMenu(fileName = "Item", menuName = "NewItem")]

//the required data to create an item
public class ItemScript : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int maxStackSize;
    public GameObject itemPrefab;
    public GameObject handItemPrefab;
    public string description;
    public AudioClip SFX;
}
