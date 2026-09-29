using UnityEngine;
using UnityEngine.SceneManagement;

public class PerformRestart : MonoBehaviour
{
    //adds to total restarts when restart button is pressed
    public void Restart()
    {
        PuzzleManager.Instance.RestartLevel();
        string currentSceneName = SceneManager.GetActiveScene().name;

        //resets score when restarting
        if (ProgressManager.Instance.lastGameplayScene == "Level 3")
        {
            //reset scores
            PuzzleManager.Instance.score = PuzzleManager.Instance.L2FinalScore;
            PuzzleManager.Instance.optionalPuzzleScore = (PuzzleManager.Instance.L2FinalScore - 100) - 50; 
            //remove levels puzzle progress
            PuzzleManager.Instance.puzzlesCompleted = PuzzleManager.Instance.L2PuzzlesCompleted;
            PuzzleManager.Instance.L3PuzzlesCompleted = 0;

        }
        else if (ProgressManager.Instance.lastGameplayScene == "Level 2") 
        {
                //100 is guaranteed to be the default score in level 2, since the player must complete every puzzle in level 1
                PuzzleManager.Instance.score = 100;
                PuzzleManager.Instance.L2FinalScore = 100;
                PuzzleManager.Instance.optionalPuzzleScore = 0;
                //remove levels puzzle progress
                PuzzleManager.Instance.puzzlesCompleted = 4;
                PuzzleManager.Instance.L2PuzzlesCompleted = 0;
         }
            
    }
}
