using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ShowContinueButton : MonoBehaviour
{
    //show button to continue if player has enough points
    [SerializeField] private Button continueButton;

    void Update()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "Level 2" && PuzzleManager.Instance.score >= 150)
        {
            continueButton.gameObject.SetActive(true);
        }
        else if (currentSceneName == "Level 3" && PuzzleManager.Instance.score >= 225)
        {
            continueButton.gameObject.SetActive(true);
        }
    }
}
