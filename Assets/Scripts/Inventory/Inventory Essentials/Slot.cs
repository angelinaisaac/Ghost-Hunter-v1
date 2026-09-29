using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool hovering;
    private ItemScript heldItem;
    private int itemAmount;
    private Image iconImage;
    private TextMeshProUGUI amountText;


    //access icon and amount text
    private void Awake()
    {
        iconImage = transform.GetChild(0).GetComponent<Image>();
        amountText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    //allows other scripts to access heldItem and itemAmount
    public ItemScript GetItem()
    {
        return heldItem;
    }

    public int GetAmount()
    {
        return itemAmount;
    }

    //adds items to slot
    public void SetItem(ItemScript item, int amount = 1)
    {
        heldItem = item;
        itemAmount = amount;
        UpdateAmountDisplay();
        UpdateSlot();
    }

    //updates amount text
    private void UpdateAmountDisplay()
    {
        int amount = GetAmount();
        if (amount > 1)
        {
            amountText.enabled = true;

        }
        else
        {
            amountText.enabled = false;
        }
    }

    //refreshes icon image, amount text, sprite
    public void UpdateSlot()
    {
        if (iconImage == null)
        {
            iconImage = transform.GetChild(0).GetComponent<Image>();
            amountText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        }

        if (heldItem != null)
        {
            iconImage.enabled = true;
            iconImage.sprite = heldItem.icon;
            amountText.text = itemAmount.ToString();
        }
        else
        {
            iconImage.enabled = false;
            amountText.text = "";
        }
    }

    //adds additional items
    public int AddAmount(int amountToAdd)
    {
        itemAmount += amountToAdd;
        UpdateSlot();
        return itemAmount;
    }

    //removes items
    public int RemoveAmount(int amountToRemove)
    {
        itemAmount -= amountToRemove;
        if (itemAmount < 0)
        {
            ClearSlot();
        }
        else
        {
            UpdateAmountDisplay();
            UpdateSlot();
        }
        return itemAmount;
    }

    //clears the slot
    public void ClearSlot()
    {
        heldItem = null;
        itemAmount = 0;
        UpdateSlot();
    }

    //tells if player is holding an item
    public bool HasItem()
    {
        return heldItem != null;
    }

    //dictates when the players hovering over slot
    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
    }
    //dictates when the players not hovering over slot
    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }

}
