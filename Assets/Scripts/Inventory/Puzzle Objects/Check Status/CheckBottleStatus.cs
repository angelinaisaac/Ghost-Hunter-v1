using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckBottleStatus : MonoBehaviour
{
    public static CheckBottleStatus Instance;
    [SerializeField] private AudioClip clip;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float maxDist = 1.1f;
    [SerializeField] private TextMeshProUGUI list;
    private bool hasFallen = false;
    public bool hasHitTutorial = false;

    private void Awake()
    {
        Instance = this;
    }

    public bool IsGrounded()
    {
        //true if raycast hits ground layer
        return Physics.Raycast(transform.position, Vector3.down, maxDist, groundLayer);
    }

    void Update()
    {
        if (IsGrounded() && !hasFallen)
        {
            hasFallen = true;
            string currentScene = SceneManager.GetActiveScene().name;
            //complete task if in tutorial level (only one bottle has to be dropped)
            if (currentScene == "Level 1")
            {
                hasHitTutorial = true;
                SFXPlayer.Instance.PlaySFX(clip);
                TutorialManagerPickedItemUpFirst.Instance.StoneThrowComplete();
                list.text = $"<s>Make a bottle fall on its own</s>";
                return;
            }
            else if(currentScene == "Level 3")
            {
                SFXPlayer.Instance.PlaySFX(clip);
            }

            //track the amount of bottles dtropped
            PuzzleManager.Instance.numOfBottlesFell++;
            //puzzle complete if 3 bottles dropped
            if (PuzzleManager.Instance.numOfBottlesFell >= 3)
            {
                list.text = $"<s>Bottles dropped ({PuzzleManager.Instance.numOfBottlesFell}/3)</s>";
                //update puzzle manager state
                PuzzleManager.Instance.L3FinalScore += 50;
                PuzzleManager.Instance.L3PuzzlesCompleted++;
                PuzzleManager.Instance.CompletePuzzle(50);
            }
            else
            {
                //display number of bottles dropped
                list.text = $"Bottles dropped ({PuzzleManager.Instance.numOfBottlesFell}/3)";
            }
        }
    }
}
