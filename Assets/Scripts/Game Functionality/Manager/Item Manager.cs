using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private ItemScript flour;

    //if additional item is purchased in shop
    //add it to the inventory when the third level loads
    void Start()
    {
        if (PurchaseItem.Instance.boughtAdditionalItem)
        {
            Inventory.Instance.AddItem(flour, 1);
        }
    }
}
