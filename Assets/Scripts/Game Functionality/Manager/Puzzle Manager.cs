using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    //tracked variables
    public int numberOfRestarts;
    public int puzzlesCompleted;
    public int optionalPuzzleScore;
    public int score;
    public int L2FinalScore;
    public int L3FinalScore;
    public int L2FinalOptionalScore;
    public int L2PuzzlesCompleted;
    public int L3PuzzlesCompleted;
    public int numOfBottlesFell = 0;
    //overall performance
    public float finalPerformanceScore;
    //score weights
    public float evidenceWeight = 40f;
    public float optionalWeight = 25f;
    public float puzzleWeight = 15f;
    public float restartWeight = 20f;
    //score targets
    public int maxRequiredScore = 200;
    public int maxOptionalScore = 120;
    public int maxPuzzlesCompleted = 11;
    public int restartLimit = 5;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //updates score and puzzles completed when run
    public void CompletePuzzle(int points)
    {
        puzzlesCompleted++;
        score += points;

        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "Level 2" && score>= 150)
        {
            optionalPuzzleScore = score - 150;
        }
        else if (currentSceneName == "Level 3" && score>= 225)
        {
            optionalPuzzleScore = (score - 200) + L2FinalOptionalScore;
        }
    }

    //updates the number of restarts taken when run
    public void RestartLevel()
    {
        numberOfRestarts++; 
    }

    //calculates overall performance
    public void CalculateFinalPerformance()
    {
        // base evidence (40/100)
        float requiredScore = Mathf.Clamp(score - optionalPuzzleScore, 0, maxRequiredScore);

        //percentage of base evidence found
        float evidencePercentage = requiredScore / maxRequiredScore;

        //convert to points
        float evidencePoints = evidencePercentage * evidenceWeight;


        //optional evidence (25/100)
        //percentage of optional points collected
        float optionalPercentage = Mathf.Clamp01((float)optionalPuzzleScore / maxOptionalScore);
        //convert to points
        float optionalPoints = optionalPercentage * optionalWeight;


        //number of puzzles completed (15/100)
        //percentage of puzzles completed
        float puzzlePercentage = Mathf.Clamp01((float)puzzlesCompleted / maxPuzzlesCompleted);
        //convert to points
        float puzzlePoints = puzzlePercentage * puzzleWeight;


        // number of restarts taken (20/100), the less the better
        //percentahe of restarts take
        float restartPercentage = Mathf.Clamp01(1f - (float)numberOfRestarts / restartLimit);
        //convert to points
        float restartPoints = restartPercentage * restartWeight;

        //final score out of 100
        finalPerformanceScore = Mathf.Clamp(
            evidencePoints +
            optionalPoints +
            puzzlePoints +
            restartPoints,
            0f,
            100f
        );
    }
}
