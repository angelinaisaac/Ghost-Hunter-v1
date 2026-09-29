using System.Runtime.CompilerServices;
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.Rendering.Universal;

public class DrapeFabricOnMannequin : MonoBehaviour
{
    public static DrapeFabricOnMannequin Instance;
    
    [SerializeField] private Camera MainCam;
    [SerializeField] private GameObject Mannequin;
    [SerializeField] private ItemScript requiredItem;
    [SerializeField] private GameObject FallingFabric;
    [SerializeField] private GameObject GhostMannequin;
    [SerializeField] private float maxDist = 20f;
    [SerializeField] private TextMeshProUGUI list;

    //sound
    [SerializeField] private AudioClip drapeSound;
    [SerializeField] private SFXPlayer sfxPlayer;
    public bool puzzleStarted = false;
    public bool mannequinSuccess = false;
    
    private void Awake()
    {
        Instance = this;

        if (MainCam == null)
        {
            MainCam = Camera.main;
        }
    }
    
    void Update()
    {
        //do not start again once started
        if (puzzleStarted) return;

        Ray ray = new Ray(MainCam.transform.position, MainCam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDist))
        {
            //ensures player is close enough with the object to interact with it
            if (hit.collider.gameObject == Mannequin)
            {
                //ensures correct item is being used and mouse is being pressed
                if (Inventory.Instance.currentItem == requiredItem && Input.GetMouseButton(0))
                {
                    //activate cloth and let it fall via coroutine
                    puzzleStarted = true;
                    sfxPlayer.PlaySFX(drapeSound);
                    Inventory.Instance.ConsumeEquippedItem();
                    FallingFabric.SetActive(true);
                    Destroy(Mannequin.GetComponent<Hint>());
                    StartCoroutine(SpawnGhost());
                    //trigger task complete for level 1
                    TutorialManagerPickedItemUpFirst.Instance.CheckTaskSuccess();

                }
            }

        }
    }

    //activates pre-draped ghost after letting cloth fall
    IEnumerator SpawnGhost()
    {
        yield return new WaitForSeconds(1.15f);
        FallingFabric.SetActive(false);
        GhostMannequin.SetActive(true);
        list.text = "<s>Make a ghost</s>";
        //update puzzle manager
        PuzzleManager.Instance.L2FinalScore += 20;
        PuzzleManager.Instance.L2PuzzlesCompleted++;
        PuzzleManager.Instance.CompletePuzzle(20);
        TutorialManagerPickedItemUpFirst.Instance.MannequinPuzzleCompleted();
        mannequinSuccess = true;   
    }
}
