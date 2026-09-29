using TMPro;
using UnityEngine;

public class CheckPianoStatus : MonoBehaviour
{
    [SerializeField] private LayerMask pianoLayer;
    [SerializeField] private float maxDist = 1.1f;
    [SerializeField] private TextMeshProUGUI list;
    [SerializeField] private AudioClip clip;
    [SerializeField] private AudioSource src;
    private bool hasFallen = false;

    public bool IsGrounded()
    {
        //true if raycast hits piano layer
        return Physics.Raycast(transform.position, Vector3.down, maxDist, pianoLayer);
    }

    void Update()
    {
        if (IsGrounded() && !hasFallen)
        {
            //checks off list if the book hits the piano
            hasFallen = true;
            src.PlayOneShot(clip, 0.5f);
            list.text = "<s>Drop something on the piano</s>";
            //update puzzle manager
            PuzzleManager.Instance.L3FinalScore += 50;
            PuzzleManager.Instance.L3PuzzlesCompleted++;
            PuzzleManager.Instance.CompletePuzzle(50);
        }
    }
}
