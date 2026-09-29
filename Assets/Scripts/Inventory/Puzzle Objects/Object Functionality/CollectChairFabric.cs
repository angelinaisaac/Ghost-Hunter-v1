using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;

public class CollectChairFabric : MonoBehaviour
{
    public static CollectChairFabric Instance;

    [SerializeField] private ItemScript requiredItem;
    [SerializeField] private Camera MainCam;
    [SerializeField] private GameObject Chair;
    [SerializeField] private ItemScript Fabric;

    [SerializeField] private Renderer fabricRenderer;
    [SerializeField] private Material newMaterial;

    //sound
    [SerializeField] private AudioClip ripSound;
    [SerializeField] private SFXPlayer sfxPlayer;

    public float maxDist = 60f;
    public bool fabricChanged = false;

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
        Ray ray = new Ray(MainCam.transform.position, MainCam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDist))
        {
            //ensures player is close enough with the object to interact with it
            if (hit.collider.gameObject == Chair)
            {
                //ensures correct item is being used and mouse is being pressed
                if (Inventory.Instance.currentItem == requiredItem && Input.GetMouseButtonDown(0))
                {
                    //change appeareance of chair, to indicate the interaction has successfully been completed
                    ChangeFabric();
                    Destroy(Chair.GetComponent<Hint>());
                    sfxPlayer.PlaySFX(ripSound);

                    //play tutorial dialogue if in level 1
                    fabricChanged = true;

                    //trigger for tutorial level
                    string currentScene = SceneManager.GetActiveScene().name;
                    if (currentScene == "Level 1")
                    {
                        TutorialManagerPickedItemUpFirst.Instance.ItemPickedUp(null);
                    }

                    //consume item, give player fabric item in return
                    Inventory.Instance.ConsumeEquippedItem();
                    Inventory.Instance.AddItem(Fabric, 1);
                }
            }

        }
    }
    void ChangeFabric()
    {
        //swap fabrics
        fabricRenderer.material = newMaterial;
    }
}
