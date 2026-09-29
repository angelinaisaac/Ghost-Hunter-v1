using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManageProducerAnimation : MonoBehaviour
{
    public static ManageProducerAnimation Instance;
    [SerializeField] private Animator animator;
    public DialogueTrigger.DialogueLine currentLine; 

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {

        //Change animation based on dialogue line
        if (currentLine.dialogue == "...so via email you told me all your evidence got corrupted?"||
            currentLine.dialogue == "......right")
        {
            //talking animation
            animator.SetInteger("AnimationStateProd", 1);
        }
        if(currentLine.dialogue == "Yeah. Happens all the time.. ghosts love to do it" ||
            currentLine.dialogue == "Just trust me. Give me 3 days and i'll bring you your proof! ")
        {
            //idle animation
            animator.SetInteger("AnimationStateProd", 3);
        }
        if (currentLine.dialogue == "..fine. I hope I won't be dissapointed. Goodluck")
        {
            //goodbye animation
            animator.SetInteger("AnimationStateProd", 2);
        }


        //switches scenes when each scenes final dialogue is displayed
        if (currentLine.dialogue == "...")
        {
            SceneManager.LoadScene("Producer Cutscene");
        }
        if(currentLine.dialogue == "....")
        {
            SceneManager.LoadScene("Level 1");
        }
        
    }
}
