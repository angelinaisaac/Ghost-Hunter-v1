using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance;

    void Awake()
    {
        Instance = this;
    }
    
    public void LoadSceneViaName(string sceneName)
    {
        //saves player's level 2 score
        if(sceneName=="Level 2")
        {
            PuzzleManager.Instance.L2FinalScore = PuzzleManager.Instance.score;
            PuzzleManager.Instance.L2FinalOptionalScore = PuzzleManager.Instance.optionalPuzzleScore;
        }
        //saves player level 3 score and calculates overall performance
        else if (sceneName == "Level 3")
        {
            PuzzleManager.Instance.L3FinalScore = PuzzleManager.Instance.score;
            PuzzleManager.Instance.CalculateFinalPerformance();
            SceneManager.LoadScene(sceneName);
        }
        //loads requested scene
        SceneManager.LoadScene(sceneName);
    }
}
