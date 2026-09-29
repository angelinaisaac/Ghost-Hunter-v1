using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class Lure1Functionality : MonoBehaviour, IUsable
{
    //lures
    [SerializeField] private ItemScript correctLure;
    [SerializeField] private ItemScript incorrectLure1;
    [SerializeField] private ItemScript incorrectLure2;
    //placement point
    [SerializeField] private Transform placementPoint;
    //lights
    [SerializeField] private GameObject greenLight;
    [SerializeField] private GameObject redLight;
    //audio
    [SerializeField] private AudioClip lurePlace;
    [SerializeField] private SFXPlayer sfxPlayer;

    private LurePuzzleManager puzzleManager;
    private bool filled = false;
    public bool itemOnStool = false;
    private GameObject placedLure;

    void Awake()
    {
        //access lure puzzle manager
        puzzleManager =FindAnyObjectByType<LurePuzzleManager>();
        //deactivate lights to start
        greenLight.SetActive(false);
        redLight.SetActive(false);
    }

    public void Use()
    {
        //cant place a lure if the holder is already filled
        if (filled)
            return;

        //cant place item if the player does not have an equipped item
        if (Inventory.Instance.currentItem == null)
            return;

        ItemScript heldItem = Inventory.Instance.currentItem;
        //determines correctness of lure placement
        if (heldItem == correctLure)
        {
            PlaceCorrectLure();
        }
        else if (heldItem == incorrectLure1)
        {
            PlaceIncorrectLure(incorrectLure1);
        }
        else if (heldItem == incorrectLure2)
        {
            PlaceIncorrectLure(incorrectLure2);
        }
        else
        {
            // wrong item shows red light
            greenLight.SetActive(false);
            redLight.SetActive(true);
        }
    }

    private void PlaceCorrectLure()
    {
        if (filled)
            return;

        sfxPlayer.PlaySFX(lurePlace);

        //plcae lure on its placement point
        placedLure = Instantiate(correctLure.itemPrefab, placementPoint.position, placementPoint.rotation);

        //get the rigidbody from the instantiated object
        Rigidbody rb = placedLure.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        //prevent the placed lure from being picked up
        Item item = placedLure.GetComponent<Item>();
        if (item != null)
        {
            item.canPickup = false;
        }

        //consume lure
        Inventory.Instance.ConsumeEquippedItem();
        Inventory.Instance.currentItem = null;

        //update holder state
        filled = true;
        itemOnStool = true;

        //activate green light to signify corectness
        greenLight.SetActive(true);
        redLight.SetActive(false);

        //mark holder as filled
        puzzleManager.HolderFilled(this);
    }

    private void PlaceIncorrectLure(ItemScript lure)
    {
        sfxPlayer.PlaySFX(lurePlace);
        //plcae lure on its placement point
        placedLure = Instantiate(lure.itemPrefab, placementPoint.position, placementPoint.rotation);
        //get the rigidbody from the instantiated object
        Rigidbody rb = placedLure.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        //incorrect lures can still be picked up
        Item item = placedLure.GetComponent<Item>();
        if (item != null)
        {
            item.canPickup = true;
        }

        //consume lure
        Inventory.Instance.ConsumeEquippedItem();
        Inventory.Instance.currentItem = null;

        //update holder state
        itemOnStool = true;

        //activate red light
        greenLight.SetActive(false);
        redLight.SetActive(true);
    }

    //called when incorrect lure is picked up from holder
    public void RemoveIncorrectLure()
    {
        //properly resets holder starw
        if (filled)
            return;

        if (placedLure != null)
        {
            Destroy(placedLure);
            placedLure = null;
        }

        itemOnStool = false;

        greenLight.SetActive(false);
        redLight.SetActive(false);
    }
}
