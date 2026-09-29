using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance;
    public bool isOnL2 = false;
    public bool isOnL3 = false;
    public string lastGameplayScene;
    
    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        //subscribe to event when created
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    //tracks which level the player is on
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //only update when entering a gameplay level, keep the previous level when loading other scenes (e.g., shop, etc)
        if (scene.name == "Level 2")
        {
            lastGameplayScene = "Level 2";
            isOnL2 = true;
            isOnL3 = false;
        }
        else if (scene.name == "Level 3")
        {
            lastGameplayScene = "Level 3";
            isOnL2 = false;
            isOnL3 = true;
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            //unsubscribe from event when destroyed
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
