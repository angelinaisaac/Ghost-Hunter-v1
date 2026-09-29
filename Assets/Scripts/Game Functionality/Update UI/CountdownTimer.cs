using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private float timeRemaining = 120f;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private AudioClip tickSFX;
    [SerializeField] private AudioClip tockSFX;
    [SerializeField] private SFXPlayer sfxPlayer;
    private bool isTimerRunning = false;
    private bool isTick = true;
    private int lastThreshold;

    void Start()
    {
        //starts timer
        isTimerRunning = true;
        lastThreshold = Mathf.CeilToInt(timeRemaining);
    }

    void Update()
    {
        if (isTimerRunning)
        {
            //if time has not run out
            if(timeRemaining > 0)
            {
                //calculates time passed since the last frame
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);

                //checks if timer has been lowered by a second
                if (Mathf.CeilToInt(timeRemaining) < lastThreshold && timeRemaining<= 60f) {
                    lastThreshold = Mathf.CeilToInt(timeRemaining);
                    PlayClockSFX();
                }
            }
            else
            {
                //if sufficient points when time runs out, move onto the next level
                if (PuzzleManager.Instance.score >= 150 && SceneManager.GetActiveScene().name == "Level 2")
                {
                    SceneChanger.Instance.LoadSceneViaName("Shop");
                }
                else if (PuzzleManager.Instance.score >= 225 && SceneManager.GetActiveScene().name == "Level 3")
                {
                    PuzzleManager.Instance.CalculateFinalPerformance();
                    SceneChanger.Instance.LoadSceneViaName("Ending");
                }
                else
                {
                    //handles if time runs out without sufficient points
                    timeRemaining = 0;
                    isTimerRunning = false;
                    DisplayTime(timeRemaining);
                    //switch to game over scene if time runs out
                    if(SceneManager.GetActiveScene().name == "Level 3")
                    {
                        SceneChanger.Instance.LoadSceneViaName("Time Run Out Screen 2");
                    }
                    else
                    {
                        SceneChanger.Instance.LoadSceneViaName("Time Run Out Screen");
                    }
                }
            }
        }
    }
    private void PlayClockSFX()
    {
        //alternates sound effects
        AudioClip correctClip = isTick ? tickSFX : tockSFX;
        sfxPlayer.PlaySFX(correctClip);
        isTick = !isTick;
    }

    //displays remaining time
    private void DisplayTime(float timeToDisplay)
    {
        //calculates remaining time in minutes and seconds
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        //formats double digits
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
