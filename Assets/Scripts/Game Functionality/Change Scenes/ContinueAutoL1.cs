using System.Collections;
using UnityEngine;

public class ContinueAutoL1 : MonoBehaviour
{

    [SerializeField] private DialogueTrigger allTasksComplete;
    [SerializeField] private DialogueTrigger rewardItemsTrigger;
    [SerializeField] private ItemScript scrap;
    [SerializeField] private ItemScript blade;
    private bool successStarted = false;
    private bool itemsAwarded = false;

    void Update()
    {
        if (successStarted) return;

        //rewards required items to craft fan for final task
        if (TutorialManagerPickedItemUpFirst.Instance.bookSuccessUsedBefore &&
            TutorialManagerPickedItemUpFirst.Instance.knifeSuccessUsedBefore &&
            TutorialManagerPickedItemUpFirst.Instance.mannequinUsedBefore &&
            TutorialManagerPickedItemUpFirst.Instance.pulleyUsedBefore &&
            TutorialManagerPickedItemUpFirst.Instance.stoneHitBefore && !itemsAwarded)
        {
            itemsAwarded = true;
            Inventory.Instance.AddItem(scrap,2);
            Inventory.Instance.AddItem(blade,1);
            StartCoroutine(GrantItem());
        }

        //if all tasks have been completed show success message and continue to next level
        if(TutorialManagerPickedItemUpFirst.Instance.fanUsedBefore &&
            TutorialManagerPickedItemUpFirst.Instance.bookSuccessUsedBefore &&
            TutorialManagerPickedItemUpFirst.Instance.knifeSuccessUsedBefore &&
            TutorialManagerPickedItemUpFirst.Instance.mannequinUsedBefore &&
            TutorialManagerPickedItemUpFirst.Instance.pulleyUsedBefore &&
            TutorialManagerPickedItemUpFirst.Instance.stoneHitBefore)
        {
            successStarted = true;
            StartCoroutine(ShowSuccess());
        }
    }

    //shows dialogue after final task
    IEnumerator ShowSuccess()
    {
        allTasksComplete.TriggerDialogue();
        yield return new WaitUntil(() => !DialogueManager.Instance.isDialogueActive);
        SceneChanger.Instance.LoadSceneViaName("Level 2");
    }

    //shows dialogue when items are granted
    IEnumerator GrantItem()
    {
        rewardItemsTrigger.TriggerDialogue();
        yield return new WaitUntil(() => !DialogueManager.Instance.isDialogueActive);
    }
}
