using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpdateScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI score;
    
    //updates player score on UI
    void Update()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if(currentSceneName == "Level 2")
        {
            //keeps score in game manager but refreshes score when level 2 starts (subtracts level 1 score from level 2 score)
            int currentScore = PuzzleManager.Instance.score - 100;
            score.text = currentScore.ToString() + "/50 ";
        }
        else if(currentSceneName == "Level 1")
        {
            score.text = PuzzleManager.Instance.score.ToString() + "/100";
        }
        else if (currentSceneName == "Level 3")
        {
            float L3Score = PuzzleManager.Instance.L3FinalScore - PuzzleManager.Instance.L2FinalScore;
            score.text = L3Score.ToString() + "/50";
        }
    }
}
