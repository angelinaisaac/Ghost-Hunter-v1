using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class KnifeFunctionality : MonoBehaviour, IUsable
{
    public static KnifeFunctionality Instance;
    private GameObject chandelierObject;
    private Inventory inventory;
    private TextMeshProUGUI list;
    private Camera MainCam;
    public float maxDist = 10f;
    public bool knifeUsed = false;
    [SerializeField] private ItemScript knife;

    public void Awake()
    {
        Instance = this;
    }
    public void Use()
    {
        MainCam = Camera.main;
        chandelierObject = GameObject.Find("Hanging Light");
        list = GameObject.Find("Chandelier").GetComponent<TextMeshProUGUI>();
        inventory = FindAnyObjectByType<Inventory>();
        Ray ray = new Ray(MainCam.transform.position, MainCam.transform.forward);

        //If the player is close enough to the chandelier (with knfie equipped and mouse down) destroy hinge joint
        //(causing the chandelier to fall)
        if (Physics.Raycast(ray, out RaycastHit hit, maxDist))
        {
            SFXPlayer.Instance.PlaySFX(knife.SFX);
            Destroy(chandelierObject.GetComponent<HingeJoint>());
            inventory.ConsumeEquippedItem();
            knifeUsed = true;
            //update tutorial state
            TutorialManagerPickedItemUpFirst.Instance.CheckTaskSuccess();
            list.text = "<s>Make the chandelier fall</s>";
            //update puzzle manager
            PuzzleManager.Instance.L2FinalScore += 30;
            PuzzleManager.Instance.L2PuzzlesCompleted++;
            PuzzleManager.Instance.CompletePuzzle(30);
            //remove hint
            Destroy(chandelierObject.GetComponent<Hint>());
        }
    }
}
