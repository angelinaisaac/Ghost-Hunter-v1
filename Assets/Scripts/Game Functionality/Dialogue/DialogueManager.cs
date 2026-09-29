using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;
using static DialogueTrigger;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public TextMeshProUGUI dialogueName;
    public TextMeshProUGUI dialogueArea;
    public GameObject dialogueBox;
    [SerializeField] private AudioClip dialogueSFX;
    [SerializeField] private AudioClip secondaryDialogueSFX;
    [SerializeField] private GameObject triggerBox;
    public Player player;
    private Queue<DialogueLine> lines;
    public bool isDialogueActive = false;
    public bool lineFinished = false;
    public float typeSpeed = 0.4f;


    void Awake()
    {
        lines = new Queue<DialogueLine>();
        Instance = this;
    }
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Update()
    {
        //once dialogue is done displaying, if the user presses the mouse the next message is shown
        if (!isDialogueActive) return;
        
        if(lineFinished && Input.GetMouseButtonDown(0))
        {
            DisplayNextDialogue();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        //freeze movement
        Player.Instance.updatingMovement = true;
        Player.Instance.updatingRotation = true;

        //updates dialogue state
        isDialogueActive = true;

        //shows dialogue box
        dialogueName.enabled = true;
        dialogueArea.enabled = true;
        dialogueBox.SetActive(true);


        //removes any dialogue from previous interactions
        lines.Clear();
        //loads dialogue to queue
        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }
            DisplayNextDialogue();
    }

    public void DisplayNextDialogue()
    {
        //if no lines remaining end dialogue
        if(lines.Count == 0)
        {
            EndDialogue();
            return;
        }
        
        lineFinished = false;
        DialogueLine currentLine = lines.Dequeue();
        //updates text
        dialogueName.text = currentLine.character.name;
        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine));
    }
    //types dialogue out
    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        dialogueArea.text = "";
        //pass current line to other scripts
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "Producer Cutscene" || currentSceneName == "Opening Cutscene")
        {
            ManageProducerAnimation.Instance.currentLine = dialogueLine;
        }
        else if (currentSceneName == "Ending")
        {
            DetermineEnding.Instance.currentLine = dialogueLine;
            //has porducer act accordingly given the current line
            DetermineEnding.Instance.HandleDialogueLine(dialogueLine);
        }
        foreach (char letter in dialogueLine.dialogue.ToCharArray())
        {
            if (!char.IsWhiteSpace(letter))
            {
                //change voice based on character talking
                if(dialogueLine.character.name == "Producer")
                {
                    SFXPlayer.Instance.PlaySFX(secondaryDialogueSFX);
                }
                else
                {
                    SFXPlayer.Instance.PlaySFX(dialogueSFX);
                }
            }
            dialogueArea.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
        lineFinished = true;

    }

    //returns the current dialogue line
    public string accessCurrentLine(DialogueLine dialogueLine)
    {
        return dialogueLine.dialogue;
    }

    //hides dialogue
    void EndDialogue()
    {
        //unfreeze movement and remove dialogue
        Player.Instance.updatingMovement = false;
        Player.Instance.updatingRotation = false;
        dialogueName.enabled = false;
        dialogueArea.enabled = false;
        dialogueBox.SetActive(false);
        isDialogueActive = false;
    }
}
