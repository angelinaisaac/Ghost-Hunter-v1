using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PulleySystem : MonoBehaviour
{
    [SerializeField] private Transform basket;
    //where rope attaches to basket
    [SerializeField] private Transform basketAttachPoint;
    //point where player grabs rope
    [SerializeField] private Transform playerAttachPoint;
    //points on either side of the pulley
    //allows rope to wrap around pulley
    [SerializeField] private Transform pulleyLeftPoint;
    [SerializeField] private Transform pulleyRightPoint;
    //displays rope
    [SerializeField] private LineRenderer rope;
    //used to track if player is looking at rope
    [SerializeField] private Collider ropeCollider;
    //used to elongate players side of rope as the scroll wheel is interacted with
    private Vector3 playerRopeStartPos;
    // basket movement
    //how quick basket moves
    [SerializeField] private float pullSpeed = 0.5f;
    //lowest position basket can be
    [SerializeField] private float minHeight = 0f;
    //highest position basket can be
    [SerializeField] private float maxHeight = 3f;
    // maximum distance from where the player can use the rope
    [SerializeField] private float maxDist = 5f;
    //item that gets activated when the basket reaches the top
    [SerializeField] private GameObject objectAtTop;
    //if the basket reached the top
    private bool reachedTop = false;
    // current vertical movement of basket
    private float currentHeight;
    // original position of the basket
    private Vector3 basketStartPosition;
    //basket rotation
    private Quaternion basketStartRotation;
    private Vector3 basketBasePosition;
    //basket RB
    [SerializeField] private Rigidbody basketRB;
    //if the player is currently using the pulley
    private bool isUsingPulley = false;
    [SerializeField] private Camera mainCam;
    //sway settings
    [SerializeField] private float swayAmount = 0.03f;
    [SerializeField] private float swaySpeed = 2f;
    [SerializeField] private float swayDamping = 2f;
    private float swayStrength = 0f;
    private float swayTime;
    [SerializeField] private float gravityPull = 0.2f;
    //ball in basket
    [SerializeField] private Transform ballStartPoint;
    [SerializeField] private GameObject ballGO;
    //access list for tutorial level
    [SerializeField] private TextMeshProUGUI list;

    private void Awake()
    {
        if (mainCam == null) mainCam = Camera.main;
        
        //where basket started
        basketStartPosition = basket.position;
        basketBasePosition = basket.position;
        basketStartRotation = basket.rotation;

        //start basket at min height
        currentHeight = minHeight;

        //inital player rope position
        playerRopeStartPos = playerAttachPoint.position;

        UpdateBasketPosition();
        UpdateRope();
    }

    private void Update()
    {
        //updates visual rope every frame
        UpdateRope();

        if (!IsPlayerCloseEnough()) {
            currentHeight -= gravityPull * Time.deltaTime;
            currentHeight = Mathf.Clamp(currentHeight, minHeight, maxHeight);
            UpdateBasketPosition();
            return;
                
                
        }

        //scroll wheel to control basket
        float scrollInput = Input.mouseScrollDelta.y;

        if (!Mathf.Approximately(scrollInput, 0f))
        {
            HandlePulling(scrollInput);
        }
        else
        {
            // let the basket fall when the player isn't pulling
            currentHeight -= gravityPull * Time.deltaTime;
            currentHeight = Mathf.Clamp(currentHeight, minHeight, maxHeight);
            UpdateBasketPosition();
        }
       
    }
    private void HandlePulling(float scrollInput)
    {
        //positive scroll = raise basket
        //negative scroll = lower basket
        currentHeight += scrollInput * pullSpeed;

        //keeps basket within limits
        currentHeight = Mathf.Clamp(currentHeight, minHeight, maxHeight);

        //more movement results in more sway
        swayStrength = Mathf.Clamp01(Mathf.Abs(scrollInput) / 2f);

        UpdateBasketPosition();
        CheckTopReached();
    }

    private void UpdateBasketPosition()
    {
        Vector3 newPosition = basketStartPosition;

        //vertical position
        newPosition.y += currentHeight;

        //reduces sway
        swayStrength = Mathf.Lerp(
            swayStrength,
            0f,
            swayDamping * Time.deltaTime
        );

        swayTime += Time.deltaTime * swaySpeed;

        //creates a sway effect
        float swayX = Mathf.Sin(swayTime) * swayAmount * swayStrength;
        float swayZ = Mathf.Cos(swayTime * 0.8f) * swayAmount * swayStrength;

        newPosition.x += swayX;
        newPosition.z += swayZ;

        //update position
        basket.position = newPosition;
        if (!reachedTop)
        {
            basket.rotation = basketStartRotation;
        }
    }

    private void UpdateRope()
    {
        if (rope == null)
            return;

        rope.positionCount = 4;

        //player's original rope position
        Vector3 playerSide = playerRopeStartPos;

        //move the player's end downward as the basket rises
        playerSide.y -= currentHeight;

        //left side of pulley
        Vector3 leftPulley = pulleyLeftPoint.position;

        //right side of pulley
        Vector3 rightPulley = pulleyRightPoint.position;

        //basket side
        Vector3 basketSide = basketAttachPoint.position;

        //set the rope positions
        rope.SetPosition(0, playerSide);
        rope.SetPosition(1, leftPulley);
        rope.SetPosition(2, rightPulley);
        rope.SetPosition(3, basketSide);
        
    }

    private bool IsPlayerCloseEnough()
    {
        //unable to run if the main camera or rope collider is null
        if (mainCam == null)
        {
            return false;
        }

        if (ropeCollider == null)
        {
            return false;
        }

        Ray ray = new Ray(mainCam.transform.position,mainCam.transform.forward);

        //returns if the player is close enough to interact with the pulley or not
        if (Physics.Raycast(ray, out RaycastHit hit,maxDist))
        {
            //player is close enough
            if (hit.collider == ropeCollider)
            {
                return true;
            }
        }
        return false;
    }

    private void CheckTopReached()
    {
        if (reachedTop)
            return;

        //if it has reached the top tip the bucket and release the ball
        if (currentHeight >= maxHeight - 0.2f)
        {
            reachedTop = true;

            //mark task as complete if in level 1
            string currentScene = SceneManager.GetActiveScene().name;
            if(currentScene == "Level 1")
            {
                TutorialManagerPickedItemUpFirst.Instance.PulleyComplete();
                list.text = "<s>Use pulley system</s>";
            }  
            StartCoroutine(FlipBasket());

        }

        IEnumerator FlipBasket()
        {
            //rotate basket
            Vector3 rotation = basket.eulerAngles;
            rotation.x = -90f;
            basket.rotation = Quaternion.Euler(rotation);

            //release ball
            ballGO.transform.position = ballStartPoint.position;

            yield return new WaitForSeconds(1.1f);
            //return basket upright
            basket.rotation = basketStartRotation;
        }
    }
}
