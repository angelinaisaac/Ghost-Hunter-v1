using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;

public class OpenDoor : MonoBehaviour
{
    public static OpenDoor Instance;
    [SerializeField] private ItemScript RequiredItem;
    [SerializeField] private Rigidbody FanRB;
    [SerializeField] private GameObject Door;
    [SerializeField] private BoxCollider DoorCollider;
    [SerializeField] private TextMeshProUGUI List;
    [SerializeField] private float MaxDist = 40f;
    [SerializeField] private Camera MainCam;
    [SerializeField] private GameObject Player;
    [SerializeField] private Transform DirectPosition;
    [SerializeField] private Transform IndirectPosition;
    [SerializeField] private HingeJoint Hinge;
    [SerializeField] private int Side1 = -54;
    [SerializeField] private int Side2 = -49;
    [SerializeField] private DialogueTrigger fanDialogue;
    [SerializeField] private Rigidbody playerRB;
    public bool fanUsed = false;
    public bool fanRequestedUse = false;
    string currentScene;
    private GameObject placedFan;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;
    }

    void Update()
    {
        //defines potential fan positions based on current level
        if (currentScene == "Level 1")
        {
            Side1 = -21;
            Side2 = -19;
        }else if(currentScene == "Level 2")
        {
            Side1 = -54;
            Side2 = -49;
        }

        //tracks if player is close enough to use fan item
        Ray ray = new Ray(MainCam.transform.position, MainCam.transform.forward);
        if(Physics.Raycast(ray, out RaycastHit hit, MaxDist))
        {
            if (hit.collider.gameObject == Door)
            {
                //player position considered when determining which side of the door the fan is placed on
                if (Player.transform.position.z <= Side1 && Inventory.Instance.currentItem == RequiredItem && Input.GetMouseButton(0))
                {
                    //consume item
                    Inventory.Instance.ConsumeEquippedItem();
                    Inventory.Instance.currentItem = null;
                    //place fan
                    placedFan = Instantiate(RequiredItem.itemPrefab, DirectPosition.position, DirectPosition.rotation);
                   
                    if (currentScene == "Level 1")
                    {
                        //update tutorial state when in level 1
                        placedFan.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
                        fanRequestedUse = true;
                        TutorialManagerPickedItemUpFirst.Instance.ItemPickedUp(null);
                        SFXPlayer.Instance.PlaySFX(RequiredItem.SFX);
                        playerRB.isKinematic = true;
                        FanRB.isKinematic = true;

                        //swing the door open
                        JointMotor motor = Hinge.motor;
                        motor.force = 100;
                        motor.targetVelocity = 100;
                        Hinge.motor = motor;
                        Hinge.useMotor = true;
                        StartCoroutine(OpenDoor());
                    }
                    else if(currentScene == "Level 2")
                    {
                        //open door
                        SFXPlayer.Instance.PlaySFX(RequiredItem.SFX);
                        FanRB.isKinematic = true;
                        JointMotor motor = Hinge.motor;
                        motor.force = 100;
                        motor.targetVelocity = -100;
                        Hinge.motor = motor;
                        Hinge.useMotor = true;
                        StartCoroutine(OpenDoor());
                    }
                    
                }
                //same process, only on other side of the door
                else if (Player.transform.position.z >= Side2 && Inventory.Instance.currentItem == RequiredItem && Input.GetMouseButton(0))
                {

                    if (currentScene == "Level 2")
                    {
                        Inventory.Instance.ConsumeEquippedItem();
                        Inventory.Instance.currentItem = null;
                        placedFan = Instantiate(RequiredItem.itemPrefab, IndirectPosition.position, IndirectPosition.rotation);
                        FanRB.isKinematic = true;
                        JointMotor motor = Hinge.motor;
                        motor.force = 100;
                        motor.targetVelocity = 100;
                        Hinge.motor = motor;
                        Hinge.useMotor = true;
                        StartCoroutine(OpenDoor());
                    }
                }
            }
        }

        IEnumerator OpenDoor()
        {
            //play sound effect
            SFXPlayer.Instance.PlaySFX(RequiredItem.SFX);
            List.text = "<s>Have a door close on its own</s>";
            //update puzzle manager
            PuzzleManager.Instance.L2FinalScore += 15;
            PuzzleManager.Instance.L2PuzzlesCompleted++;
            PuzzleManager.Instance.CompletePuzzle(15);
            //destroy hint component so hints are not seen after puzzle is completed
            Destroy(Door.GetComponent<Hint>());
            yield return new WaitForSeconds(3f);
            //update tutorial values
            if (currentScene == "Level 1")
            {
                playerRB.isKinematic = false;
                fanUsed = true;
            }
            //destroy fan 
            Destroy(placedFan);
            //destory door box collider so player can move through the door
            Destroy(Door.GetComponent<BoxCollider>());

        }
    }
}
