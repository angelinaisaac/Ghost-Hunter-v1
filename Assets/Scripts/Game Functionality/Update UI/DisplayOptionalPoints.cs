using TMPro;
using UnityEngine;

public class DisplayOptionalPoints : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI score;

    // Displays the amount of spendable points in shop
    void Update()
    {
        score.text = "Your spendable points: " + PuzzleManager.Instance.optionalPuzzleScore;
    }
}
