using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using static UnityEditor.Progress;
using UnityEngine.SceneManagement;

public class Inventory : MonoBehaviour
{
    //player movement
    PlayerInput playerInput;
    CharacterController controller;
    //items
    public ItemScript knifeItem;
    public ItemScript scrapItem;
    public ItemScript stoneItem;
    public ItemScript flashlightItem;
    public ItemScript fanItem;
    public ItemScript keyItem;
    public ItemScript fishLure1Item;
    public ItemScript fishLure2Item;
    public ItemScript fishLure3Item;
    private Lure1Functionality[] lures;
    public ItemScript Scissors;
    public ItemScript fabricItem;
    public ItemScript flourItem;
    public bool isFlourEquipped = false;
    public Recipe fanRecipe;
    //inventory
    public GameObject hotbarObj;
    public GameObject inventorySlotParent;
    public GameObject container;
    [SerializeField] private TextMeshProUGUI craftWarning;
    //display text
    public TextMeshProUGUI warningText;
    //pickup
    public Image dragIcon;
    public bool inventoryOpen;
    public float pickupRange = 3f;
    //private Item lookedAtItem = null;
    public Material highlightMaterial;
    private Material originalMaterial;
    private Renderer lookedAtRenderer = null;
    private Item lookedAtItem;
    //slots
    private List<Slot> inventorySlots = new List<Slot>();
    private List<Slot> hotbarSlots = new List<Slot>();
    private List<Slot> allSlots = new List<Slot>();
    //hotbar index 0-5
    private int equippedHotbarIndex = 0;
    public float equippedOpacity = 0.9f;
    public float normalOpacity = 0.5f;
    public Transform hand;
    public GameObject currentHandItem;
    public ItemScript currentItem;
    public Slot currentSlot;
    private Slot draggedSlot = null;
    private bool isDragging = false;
    //describes item
    public GameObject itemDescriptionParent;
    public Image itemDescriptionImage;
    public TextMeshProUGUI descriptionNameText;
    public TextMeshProUGUI itemDescriptionText;
    //crafting
    public List<Recipe> allRecipes = new List<Recipe>();
    public Transform craftingGrid;
    public GameObject neededItemUIprefab;
    public GameObject CraftButtonprefab;
    //save player position
    Vector3 savedPosition;
    Quaternion savedRotation;
    bool checkingInv;
    //sound
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioClip dropSound;
    [SerializeField] private AudioClip chandelierDrop;
    [SerializeField] private AudioClip craftSound;
    [SerializeField] private AudioClip invOpenSound;
    [SerializeField] private AudioClip invCloseSound;
    [SerializeField] private SFXPlayer sfxPlayer;
    private SFXPlayer SFXPlayer;
    [SerializeField] private Camera mainCam;
    [SerializeField] private Canvas list;
    public static Inventory Instance;

    private void Awake()
    {
        Instance = this;

        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        inventorySlots.AddRange(inventorySlotParent.GetComponentsInChildren<Slot>());
        hotbarSlots.AddRange(hotbarObj.GetComponentsInChildren<Slot>());
        allSlots.AddRange(hotbarSlots);
        allSlots.AddRange(inventorySlots);
        
        PopulateCraftingGrid();

        lures = FindObjectsByType<Lure1Functionality>();
        craftWarning.enabled = false;
    }


    void Update()
    {   
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Rigidbody rb = Player.Instance.GetComponent<Rigidbody>();
            
            //inspection mode
            if (!checkingInv)
            {
                sfxPlayer.PlaySFX(invOpenSound);

                //shows cursor
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                //hides task list
                list.enabled = false;
                //saves current position
                savedPosition = Player.Instance.transform.position;
                savedRotation = Player.Instance.transform.rotation;
                Player.Instance.updatingRotation = true;
                Player.Instance.updatingMovement = true;
             

                if (rb != null)
                {
                    rb.angularVelocity = Vector3.zero;
                    rb.linearVelocity = Vector3.zero;
                    rb.isKinematic = true;
                }

                //activate container
                container.SetActive(!container.activeInHierarchy);
                inventoryOpen = container.activeInHierarchy;
                checkingInv = true;
            }
            else
            {
                sfxPlayer.PlaySFX(invCloseSound);

                //reactivate task list
                list.enabled = true;

                if (rb != null) rb.isKinematic = false;

                //return to original position/rotation
                Player.Instance.transform.position = savedPosition;
                Player.Instance.transform.rotation = savedRotation;

                Player.Instance.updatingRotation = false;
                Player.Instance.updatingMovement = false;

                //hides cursor again
                Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = !Cursor.visible;

                //hides container
                container.SetActive(!container.activeInHierarchy);
                inventoryOpen = container.activeInHierarchy;
                checkingInv = false;
            }
        }
        DetectLookedAtItem();
        Pickup();
        StartDrag();
        UpdateDragItemPosition();
        EndDrag();
        UseItem();
        HandleHotbarSelection();
        HandleDropEquippedItem();
        UpdateHotbarOpactiy();
        UpdateItemDescription();
    }
    

    public void AddItem(ItemScript itemToAdd, int amount)
    {
        sfxPlayer.PlaySFX(pickupSound);
        int remaining = amount;
        
        //if there is a prexisting stack (hotbar/inventory) of the same item, add it to the stack
        foreach (Slot slot in allSlots)
        {
            if (slot.HasItem() && slot.GetItem() == itemToAdd)
            {
               
                int space = itemToAdd.maxStackSize - slot.GetAmount();

                //if theres more space add item to slot
                if (space > 0)
                {
                    int added = Mathf.Min(space, remaining);
                    slot.SetItem(itemToAdd, slot.GetAmount() + added);
                    remaining -= added;

                    if (remaining <= 0)
                    {
                        PopulateCraftingGrid();
                        return;
                    }
                }
            }
        }
        //otherwise add the item to the first available hotbar slot
        foreach (Slot slot in hotbarSlots)
        {
            if (!slot.HasItem())
            {
                int placeAmount = Mathf.Min(itemToAdd.maxStackSize, remaining);

                slot.SetItem(itemToAdd, placeAmount);
                remaining -= placeAmount;

                if (remaining <= 0)
                {
                    PopulateCraftingGrid();
                    // auto-equip first item picked up
                    EquipHandItem();

                    return;
                }
            }
        }
        //if the hotbar is full use the inventory slots
        foreach (Slot slot in inventorySlots)
        {
            if (!slot.HasItem())
            {
                int placeAmount = Mathf.Min(itemToAdd.maxStackSize, remaining);

                slot.SetItem(itemToAdd, placeAmount);
                remaining -= placeAmount;

                if (remaining <= 0)
                {
                    PopulateCraftingGrid();
                    return;
                }
            }
        }

        //inventory full
        if (remaining > 0)
        {
        }

        //refresh crafting menu
        PopulateCraftingGrid();
    }


    private void UseItem()
    {
        if (inventoryOpen) return;


        if (Input.GetMouseButtonDown(0)) {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 5f))
            {
                // Try using the object you clicked
                if (hit.collider.TryGetComponent<IUsable>(out IUsable usable))
                {
                    usable.Use();       
                    return;
                }
            }

            if (currentHandItem == null)
            {
                return;
            }
            
            //use the object youre currently holding
            currentHandItem.GetComponent<IUsable>()?.Use();
        }
    }

    private void StartDrag()
    {
        if (!inventoryOpen) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            Slot hovered = GetHoveredSlot();

            if (hovered != null && hovered.HasItem())
            {
                draggedSlot = hovered;
                isDragging = true;

                //show the item thats being dragged
                dragIcon.sprite = hovered.GetItem().icon;
                dragIcon.color = new Color(1, 1, 1, 0.5f);
                dragIcon.enabled = true;
            }
        }
    }

    private void EndDrag()
    {
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Slot hovered = GetHoveredSlot();
            if (hovered != null)
            {
                //stop showing the item thats being dragged
                HandleDrop(draggedSlot, hovered);
                dragIcon.enabled = false;
                draggedSlot = null;
                isDragging = false;
            }
        }
    }


    private void HandleDrop(Slot from, Slot to)
    {
        //no dragging
        if (from == to) return;

        //handles stacking
        if (to.HasItem() && to.GetItem() == from.GetItem())
        {
            int max = to.GetItem().maxStackSize;
            int space = max - to.GetAmount();

            if (space > 0)
            {
                int move = Mathf.Min(space, from.GetAmount());
                to.SetItem(to.GetItem(), to.GetAmount() + move);
                from.SetItem(from.GetItem(), from.GetAmount() - move);

                if (from.GetAmount() <= 0)
                    from.ClearSlot();
                return;
            }
        }

        //handles different items
        if (to.HasItem())
        {
            ItemScript tempItem = to.GetItem();
            int tempAmount = to.GetAmount();
            to.SetItem(from.GetItem(), from.GetAmount());
            from.SetItem(tempItem, tempAmount);
            return;
        }

        //empty slot
        to.SetItem(from.GetItem(), from.GetAmount());
        from.ClearSlot();
    }

    //drags icon
    private void UpdateDragItemPosition()
    {
        if (isDragging)
        {
            dragIcon.transform.position = Input.mousePosition;
        }
    }

    //returns the current hovered slot
    private Slot GetHoveredSlot()
    {
        foreach (Slot s in allSlots)
        {
            if (s.hovering)
                return s;
        }
        return null;
    }

    private void Pickup()
    {
        if(lookedAtItem != null && Input.GetKeyDown(KeyCode.E))
        {
            Item item = lookedAtItem;

            //if item cannot be picked up, return
            if (!item.canPickup) return;

            //tracks if stools on lure puzzle are empty
            if(item.item == fishLure1Item ||
                item.item == fishLure2Item ||
                item.item == fishLure3Item)
            {
                for(int i =0; i< lures.Length; i++)
                {
                    lures[i].itemOnStool = false;
                }
            }

            //turns outline off before being collected
            item.outline.enabled = false;

            AddItem(item.item, item.amount);
            //only track picked up objects if in tutorial
            string currentScene = SceneManager.GetActiveScene().name;
            if(currentScene == "Level 1")
            {
                TutorialManagerPickedItemUpFirst.Instance.ItemPickedUp(item.item);
            }
            //TutorialManager.Instance.ItemCollected(item.item); 
            Destroy(item.gameObject);
            EquipHandItem();
        }
    }
    private void DetectLookedAtItem()
    {
        //turn off previous item's outline
        if(lookedAtItem != null)
        {
            lookedAtItem.outline.enabled = false;
            lookedAtItem = null;
        }

        //update outline if items being looked at
        Ray ray = new Ray(mainCam.transform.position, mainCam.transform.forward);
        if(Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            Item item = hit.collider.GetComponentInParent<Item>();

            if(item != null)
            {
                lookedAtItem = item;
                lookedAtItem.outline.enabled = true;
            }
        }
    }

    //updates hotbar opacity if slots corresponding key (1-5) is pressed
    private void UpdateHotbarOpactiy()
    {
        for (int i = 0; i < hotbarSlots.Count; i++)
        {
            Image icon = hotbarSlots[i].GetComponent<Image>();
            if (icon != null)
            {
                icon.color = (i == equippedHotbarIndex) ? new Color(1, 1, 1, equippedOpacity) : new Color(1, 1, 1, normalOpacity);
            }
        }
    }

    //equips item based on hotbar selection
    private void HandleHotbarSelection()
    {
        for (int i = 0; i < 6; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                equippedHotbarIndex = i;
                UpdateHotbarOpactiy();
                EquipHandItem();
            }

        }
    }

    private void HandleDropEquippedItem()
    {
        //input must be q to drop item
        if (!Input.GetKeyDown(KeyCode.Q)) return;

        Slot equippedSlot = hotbarSlots[equippedHotbarIndex];
        if (!equippedSlot.HasItem()) return;
        ItemScript itemScript = equippedSlot.GetItem();
        GameObject prefab = itemScript.itemPrefab;

        if (prefab == null) return;

        //shows warning if player tries to drop fan
        if (itemScript == fanItem || itemScript == fabricItem)
        {
            StartCoroutine(ShowWarning("I am unable to drop this"));
            return;
        }

        //instantiate dropped item
        GameObject dropped = Instantiate(prefab, Camera.main.transform.position + Camera.main.transform.forward, Quaternion.identity);
        sfxPlayer.PlaySFX(dropSound);

        //associate dropped item with item and its values
        Item item = dropped.GetComponent<Item>();
        item.item = itemScript;
        item.amount = equippedSlot.GetAmount();

        //clear slot and refresh inventory state
        equippedSlot.ClearSlot();
        EquipHandItem();
        PopulateCraftingGrid();
    }
    
    //temporarily shows warning
    IEnumerator ShowWarning(string input)
    {
        warningText.text = input;
        yield return new WaitForSeconds(1.5f);
        warningText.text = "";
    }



    private void EquipHandItem()
    {
        //remove current object
        if (currentHandItem != null)
        {
            Destroy(currentHandItem);
            currentHandItem = null;
        }

        //reset equipped item
        currentItem = null;
        isFlourEquipped = false;

        Slot equippedSlot = hotbarSlots[equippedHotbarIndex];

        currentSlot = hotbarSlots[equippedHotbarIndex];

        //nothing in selected slot
        if (!equippedSlot.HasItem())
        {
            return;
        }

        ItemScript item = equippedSlot.GetItem();

        //what is actually equipped
        currentItem = item;

        if (item == flourItem)
        {
            isFlourEquipped = true;
        }

        // if item has no hand model
        if (item.handItemPrefab == null)
        {
            return;
        }

        //create hand model
        currentHandItem = Instantiate(item.handItemPrefab, hand);
        var itemUser = currentHandItem.GetComponent<IItemUser>();

        if (itemUser != null)
        {
            itemUser.SetItemData(item);
        }

        currentHandItem.transform.localPosition = Vector3.zero;
        currentHandItem.transform.localRotation = Quaternion.identity;
    }

    //displays item description when hovered over
    private void UpdateItemDescription()
    {
        Slot hoveredSlot = GetHoveredSlot();
        if (hoveredSlot != null)
        {
            ItemScript hoveredItem = hoveredSlot.GetItem();

            if (hoveredItem != null)
            {
                itemDescriptionParent.SetActive(true);
                itemDescriptionImage.sprite = hoveredItem.icon;
                itemDescriptionText.text = hoveredItem.description;
                descriptionNameText.text = hoveredItem.name;
                return;
            }
        }
        itemDescriptionParent.SetActive(false);
    }

    //refreshes crafting grid
    private void PopulateCraftingGrid()
    {
        for (int i = craftingGrid.childCount - 1; i >= 0; i--)
        {
            Destroy(craftingGrid.GetChild(i).gameObject);
        }

        foreach (Recipe recipe in allRecipes)
        {
            GameObject btnObj = Instantiate(CraftButtonprefab, craftingGrid);

            Image img = btnObj.transform.GetChild(0).GetComponent<Image>();
            img.sprite = recipe.result.icon;

            Button btn = btnObj.GetComponent<Button>();
            btn.interactable = CanCraft(recipe);
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => Craft(recipe));

            foreach (Ingredient ingredient in recipe.ingredients)
            {
                GameObject requiredObject = Instantiate(neededItemUIprefab, btnObj.transform.GetChild(1));
                requiredObject.GetComponent<Image>().sprite = ingredient.item.icon;
                requiredObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "x" + ingredient.amount.ToString();
            }
        }
    }

    //determines if player can craft,
    //if so items consume, resulting item added, and crafting grid is refreshed
    public void Craft(Recipe recipe)
    {
        if (!CanCraft(recipe))
        {
            StartCoroutine(ShowCraftWarning());
            return;
        }

        sfxPlayer.PlaySFX(craftSound);
        ConsumeIngredients(recipe);
        AddItem(recipe.result, recipe.resultAmount);
        PopulateCraftingGrid();

        //triggers craft dialogue if in tutorial
        if(recipe == fanRecipe && !TutorialManagerCraft.Instance.hasShownCraftDialogue)
        {
            TutorialManagerCraft.Instance.TriggerCraftDialogue();
        }
    }

    //temporarily shows warning if recipe cannot be crafted
    IEnumerator ShowCraftWarning()
    {
        craftWarning.enabled = true;
        yield return new WaitForSeconds(1.5f);
        craftWarning.enabled = false;
    }

    //takes all ingredients from a recipe when something is crafted
    private void ConsumeIngredients(Recipe recipe)
    {
        foreach (Ingredient ingredient in recipe.ingredients)
        {
            int remaining = ingredient.amount;
            foreach (Slot slot in allSlots)
            {
                if (!slot.HasItem()) continue;
                if (slot.GetItem() != ingredient.item) continue;

                int take = Mathf.Min(slot.GetAmount(), remaining);
                slot.SetItem(slot.GetItem(), slot.GetAmount() - take);

                if (slot.GetAmount() <= 0) slot.ClearSlot();

                remaining -= take;
                if (remaining <= 0) break;
            }
        }
    }

    //determines if the player has the correct amount of materials to 
    //craft a given recipe
    public bool CanCraft(Recipe recipe)
    {
        foreach (Ingredient ingredient in recipe.ingredients)
        {
            int totalFound = 0;

            foreach (Slot slot in allSlots)
            {
                if (slot.HasItem() && slot.GetItem() == ingredient.item)
                {
                    totalFound += slot.GetAmount();
                }
            }
            if (totalFound < ingredient.amount) return false;


        }
        return true;
    }

    public void ConsumeEquippedItem()
    {
        //accesses currrent hotbar slot
        Slot equippedSlot = hotbarSlots[equippedHotbarIndex];

        //functions irrelevant if the player is holding no item
        if (!equippedSlot.HasItem()) return;
        
        //check the amount the given item has
        int amount = equippedSlot.GetAmount();

        //if theres a stack of an item, subtract it by one
        if ((amount > 1))
        {
            equippedSlot.SetItem(equippedSlot.GetItem(), amount - 1);
        }
        //otherwise, if there is only one of an item, clear the slot
        else
        {
            equippedSlot.ClearSlot();
        }
        //refresh inventory state
        EquipHandItem();
        PopulateCraftingGrid();
    }
}
