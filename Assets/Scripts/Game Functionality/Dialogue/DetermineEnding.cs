using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class DetermineEnding : MonoBehaviour
{
    [SerializeField] DialogueTrigger goodEndingDialogue;
    [SerializeField] DialogueTrigger badEndingDialogue;
    [SerializeField] private Animator animator;
    [SerializeField] TextMeshProUGUI score;
    [SerializeField] TextMeshPro message;
    [SerializeField] AudioClip winClip;
    [SerializeField] AudioClip loseClip;
    public DialogueTrigger.DialogueLine currentLine;
    bool goodEnding;
    public static DetermineEnding Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //if evidence is scored equal or greater to 50 the player got the good ending
        if (PuzzleManager.Instance.finalPerformanceScore >= 50)
        {
            goodEndingDialogue.TriggerDialogue();
            goodEnding = true;


        }
        //otherwise the player got the bad ending
        else
        {
            badEndingDialogue.TriggerDialogue();
            goodEnding = false;
        }
    }

    private void Update()
    {
        //display evidence score
        score.text = "Evidence score: "+ PuzzleManager.Instance.finalPerformanceScore.ToString();
        //update win/lose text based on ending
        if (goodEnding)
        {
            message.text = "Congratulations!\nYou won!";
        }
        else if (!goodEnding)
        {
            message.text = "She's not convinced...!\nTry again?";
        }
    }

    //set animation  state based on dialogue
    public void HandleDialogueLine(DialogueTrigger.DialogueLine line)
    {
        if (line.dialogue == "This proof is undeniable!! Let's sign a deal right now! ")
        {
            SFXPlayer.Instance.PlaySFX(winClip);
            animator.SetInteger("movementState", 2);
        }
        else if (line.dialogue == "What is this?? This is clearly fake!")
        {
            animator.SetInteger("movementState", 1);
        }
        else if (line.dialogue == "Don't bother. I really wanted to sign a deal but i guess not :(")
        {
            SFXPlayer.Instance.PlaySFX(loseClip);
        }
    }
}
