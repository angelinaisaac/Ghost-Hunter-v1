using UnityEngine;

public class TutorialManagerCraft : MonoBehaviour
{
    [SerializeField] private DialogueTrigger craftDialogue;
    [SerializeField] private GameObject doorPointer;
    public static TutorialManagerCraft Instance;
    public bool hasShownCraftDialogue = false;

    void Awake()
    {
        Instance = this;
    }
    
    //shows appropriate dialogue once required item (fan) has been crafted
    //show door pointer
    public void TriggerCraftDialogue()
    {
        craftDialogue.TriggerDialogue();
        hasShownCraftDialogue = true;
        doorPointer.SetActive(true);
    }
}
