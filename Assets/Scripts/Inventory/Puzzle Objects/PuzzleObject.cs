using UnityEngine;

//creates item instance
[CreateAssetMenu(fileName = "PuzzleObject", menuName = "PuzzleObject")]
public class PuzzleObject : ScriptableObject
{
    //information needed for a puzzle object
    public string hint;
    public ItemScript requiredItem;
    public GameObject prefab;
}
