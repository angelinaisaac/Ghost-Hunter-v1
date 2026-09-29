using TMPro;
using UnityEngine;
using System.Collections;

public class BookDrag : MonoBehaviour
{
    public static BookDrag Instance;
    //tracks if the mouse is being held down
    private bool isBeingDragged = false;
    //required object to drag book
    public GameObject RequiredObj;
    [SerializeField] private ItemScript requiredItem;
    [SerializeField] private Transform book;
    [SerializeField] private Transform fishingLine;
    [SerializeField] private Transform rodTip;
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject bookGO;
    //used to update the list
    private TextMeshProUGUI list;
    //variables influencing movement
    public float swayAmount = 0.05f;
    public float swaySpeed = 2f;
    public float scrollSpeed = 5f;
    public float minHeight = 0.5f;
    public float maxHeight = 5f;
    private float currentHeight = 2f;
    [SerializeField] private AudioClip castSound;
    [SerializeField] private AudioClip reelSound;
    [SerializeField] private SFXPlayer sfxPlayer;
    Rigidbody rb;
    Collider bookCollider;
    public bool beenLifted = false;
    private bool puzzleCompleted = false;

    void Awake()
    {
        Instance = this;
        rb = book.GetComponent<Rigidbody>();
        bookCollider = book.GetComponent<Collider>();
        list = GameObject.Find("Book").GetComponent<TextMeshProUGUI>();
    }

    private void OnMouseDown()
    {
        // tracks when mouse is held down
        if(Inventory.Instance.currentItem == requiredItem)
        {
            isBeingDragged = true;
            Inventory.Instance.currentItem = requiredItem;
            sfxPlayer.PlaySFX(castSound);
        }
        else
        {
            isBeingDragged= false;
        }
    }

    private void OnMouseUp()
    {
        // tracks when mouse button has been released
        // completing the interaction, and returning to the games previous state
        if(beenLifted == true)
        {
            TutorialManagerPickedItemUpFirst.Instance.CheckTaskSuccess();
            isBeingDragged = false;
            rb.isKinematic = false;
            bookCollider.enabled = true;
            fishingLine.GetComponent<MeshRenderer>().enabled = false;
            Inventory.Instance.currentItem = null;
            inventory.ConsumeEquippedItem();
        }
    }

    private void Update()
    {

        fishingLine.GetComponent<MeshRenderer>().enabled = false;
        if (!isBeingDragged) return;

        //stop dragging if dragging has stopped and the required item is not equipped
        if (isBeingDragged && Inventory.Instance.currentItem != requiredItem)
        {
            StopDragging();
            return;
        }

        // enter drag state if required item equipped
        if (isBeingDragged && Inventory.Instance.currentItem == requiredItem)
        {
            rb.isKinematic = true;
            bookCollider.enabled = false;
            fishingLine.GetComponent<MeshRenderer>().enabled = true;

            // Captures scroll wheel movement
            float scrollInput = Input.mouseScrollDelta.y;

            if (scrollInput !=0)
            {
                //calculates current height
                currentHeight += scrollInput * scrollSpeed; 

                //ensures height does not go infinetely high or low
                currentHeight = Mathf.Clamp(currentHeight, minHeight, maxHeight);
                if(scrollInput < 0f)
                {
                    //updates list + disables hints when book has successfully been lifted
                    list.text = "<s>Make book float</s>";
                    Destroy(bookGO.GetComponent<Hint>());
                    beenLifted = true;
                    StartCoroutine(DropBook());
                }
            }
          
            Vector3 targetPosition = rodTip.position - Vector3.up * currentHeight;

            // sways book
            float swayX = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
            float swayZ = Mathf.Cos(Time.time * swaySpeed) * swayAmount;
            targetPosition += new Vector3(swayX, 0, swayZ);

            //smoothly transitions from the objects inital position to its target position
            book.position = Vector3.Lerp(book.position, targetPosition, Time.deltaTime*3.5f);

            //determines where the line should point
            Vector3 direction = book.position - rodTip.position;

            //sits between target object and fishing rod tip
            Vector3 lineTarget = (rodTip.position + book.position) / 2;

            fishingLine.position = Vector3.Lerp(fishingLine.position, lineTarget, Time.deltaTime * 10f);
            fishingLine.rotation = Quaternion.FromToRotation(Vector3.up, direction);
            fishingLine.localScale = new Vector3(fishingLine.localScale.x, direction.magnitude / 2, fishingLine.localScale.z);
            return;

        }
        else
        {
            //non-drag state
            rb.isKinematic = false;
            bookCollider.enabled = true;
            fishingLine.GetComponent<MeshRenderer>().enabled = false;
            return;
        }
    }
    //resets drag state
    private void StopDragging()
    {
        isBeingDragged = false;
        rb.isKinematic = false;
        bookCollider.enabled = true;
        fishingLine.GetComponent<MeshRenderer>().enabled = false;
    }

    //stops dragging
    private void OnDisable()
    {
        StopDragging();
    }

    IEnumerator DropBook()
    {
        //updates puzzle manager
        if (!puzzleCompleted)
        {
            PuzzleManager.Instance.L2FinalScore += 35;
            PuzzleManager.Instance.L2PuzzlesCompleted++;
            PuzzleManager.Instance.CompletePuzzle(35);
            puzzleCompleted = true;
        }
        sfxPlayer.PlaySFX(reelSound);
        //allows player to drag for 2.5 seconds before being dropped
        yield return new WaitForSeconds(2.5f);
        //drop book
        isBeingDragged = false;
        rb.isKinematic = false;
        bookCollider.enabled = true;
        fishingLine.GetComponent<MeshRenderer>().enabled = false;
        //consume fishing rod
        Inventory.Instance.currentItem = null;
        inventory.ConsumeEquippedItem();
    }
}
