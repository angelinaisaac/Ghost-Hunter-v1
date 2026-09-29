using TMPro;
using UnityEngine;

public class TutorialManagerPickedItemUpFirst : MonoBehaviour
{
    
    public static TutorialManagerPickedItemUpFirst Instance;
    [SerializeField] private ItemScript knifeItem;
    [SerializeField] private ItemScript scissorsItem;
    [SerializeField] private ItemScript fishingRodItem;
    [SerializeField] private ItemScript stoneItem;
    [Header("Dialogues")]
    [SerializeField] private DialogueTrigger knifeDialogue;
    [SerializeField] private DialogueTrigger scissorsDialogue;
    [SerializeField] private DialogueTrigger fishingRodDialogue;
    [SerializeField] private DialogueTrigger fabricDialogue;
    [SerializeField] private DialogueTrigger fanDialogue;
    [SerializeField] private DialogueTrigger stoneDialogue;
    [Header("Success Dialogues")]
    [SerializeField] private DialogueTrigger bookSuccess;
    [SerializeField] private DialogueTrigger knifeSuccess;
    [SerializeField] private DialogueTrigger mannequinSuccess;
    [SerializeField] private DialogueTrigger stoneSuccess;
    [SerializeField] private DialogueTrigger pulleySuccess;
    //activate pointers when item picked up, hide them when task is complete
    [Header("Pointers")]
    [SerializeField] private GameObject bottlePointer;
    [SerializeField] private GameObject bookPointer;
    [SerializeField] private GameObject chairPointer;
    [SerializeField] private GameObject mannequinPointer;
    [SerializeField] private GameObject lightPointer;
    private bool knifeDialoguePlayed = false;
    private bool scissorsDialoguePlayed = false;
    private bool fishingRodDialoguePlayed = false;
    private bool fabricChangedBefore = false;
    private bool stoneDialoguePlayed = false;
    public bool fanUsedBefore = false;
    public bool stoneHitBefore = false;
    public bool pulleyUsedBefore = false;
    public bool bookSuccessUsedBefore = false;
    public bool knifeSuccessUsedBefore = false;
    public bool mannequinUsedBefore = false;

    void Awake()
    {
        Instance = this;
    }


    //checks if item has been picked up for the first time and that required dialogue has not yet been displayed
    public void ItemPickedUp(ItemScript item)
    {
        if (item == knifeItem && !knifeDialoguePlayed)
        {
            knifeDialogue.TriggerDialogue();
            knifeDialoguePlayed = true;
            lightPointer.SetActive(true);
        }
        else if (item == scissorsItem && !scissorsDialoguePlayed)
        {
            scissorsDialogue.TriggerDialogue();
            scissorsDialoguePlayed = true;
            chairPointer.SetActive(true);
        }
        else if (item == fishingRodItem && !fishingRodDialoguePlayed)
        {
            fishingRodDialogue.TriggerDialogue();
            fishingRodDialoguePlayed = true;
            bookPointer.SetActive(true);
        }
        else if (item == stoneItem && !stoneDialoguePlayed)
        {
            stoneDialogue.TriggerDialogue();
            stoneDialoguePlayed = true;
            bottlePointer.SetActive(true);
        }
        else if (CollectChairFabric.Instance.fabricChanged && !fabricChangedBefore)
        {
            fabricDialogue.TriggerDialogue();
            fabricChangedBefore = true;
            chairPointer.SetActive(false);
            mannequinPointer.SetActive(true);
        }
        else if(OpenDoor.Instance.fanRequestedUse && !fanUsedBefore)
        {
            fanDialogue.TriggerDialogue();
            fanUsedBefore = true;
        }
    }

    //checks if task has been completed successfully and its dialogue has not yet been displayed, if so trigger dialogue
    public void CheckTaskSuccess()
    {
        if (BookDrag.Instance.beenLifted && !bookSuccessUsedBefore)
        {
            bookPointer.SetActive(false);
            bookSuccess.TriggerDialogue();
            bookSuccessUsedBefore = true;
        }
        else if (KnifeFunctionality.Instance.knifeUsed && !knifeSuccessUsedBefore)
        {
            knifeSuccess.TriggerDialogue();
            knifeSuccessUsedBefore = true;
            lightPointer.SetActive(false);
        }
    }

    //trigger dialogue when first stone successfully knocks over bottle
    public void StoneThrowComplete()
    {
        if (stoneHitBefore)
            return;

        stoneHitBefore = true;
        bottlePointer.SetActive(false);
        stoneSuccess.TriggerDialogue();
    }

    //trigger dialogue when pulley pulled all the way up
    public void PulleyComplete()
    {
        if (pulleyUsedBefore)
            return;
        pulleyUsedBefore = true;
        pulleySuccess.TriggerDialogue();
    }

    //triggers dialogue when the mannequin puzzle is initally completed
    public void MannequinPuzzleCompleted()
    {
        if (mannequinUsedBefore)
            return;

        mannequinUsedBefore = true;
        mannequinSuccess.TriggerDialogue();
        mannequinPointer.SetActive(false);
    }

}
