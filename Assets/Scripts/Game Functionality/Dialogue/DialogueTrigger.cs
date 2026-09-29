using UnityEngine;
using System.Collections.Generic;

public class DialogueTrigger : MonoBehaviour
{   
    private bool hasPlayed = false;

    //shows the owner of the message
    [System.Serializable] public class DialogueCharacter
    {
        public string name;
    }

    //represents the dialogue message
    [System.Serializable] public class DialogueLine
    {
        public DialogueCharacter character;
        [TextArea(3,10)]
        public string dialogue;
    }

    //stores dialogue
    [System.Serializable] public class Dialogue
    {
        public List<DialogueLine> dialogueLines = new List<DialogueLine>();
    }

    public Dialogue dialogue;

    //allows dialogue to be triggered
    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue);
    }

    private void OnTriggerEnter(Collider collision)
    {
        //do not play dialogue if it has already been played once
        if (hasPlayed) return;
       
        //plays dialogue
        if(collision.tag == "Player")
        {
            hasPlayed = true;
            TriggerDialogue();
        }
    }

}
