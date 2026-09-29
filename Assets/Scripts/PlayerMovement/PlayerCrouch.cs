using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class PlayerCrouch : MonoBehaviour
{
    [SerializeField] float crouchHeight = 1f;
    [SerializeField] float crouchTransitionSpeed = 10f;
    [SerializeField] float crouchSpeedMultiplier = .5f;
    private AudioManager audioManager;
    public AudioSource audioSource;
    [SerializeField] private AudioClip crouchUp;
    [SerializeField] private AudioClip crouchDown;
    Player player;
    PlayerInput playerInput;
    InputAction crouchAction;
    float currentHeight;
    float standingHeight;
    Vector3 initalCamPos;
    private bool wasTryingCrouch = false;
    bool isCrouching => standingHeight - currentHeight > .1f;

    private void Awake()
    {
        player = GetComponent<Player>();
        playerInput = GetComponent<PlayerInput>();
        crouchAction = playerInput.actions["crouch"];
    }

    void Start()
    {
        initalCamPos = player.transformCamera.localPosition;
        standingHeight = currentHeight =  player.Height;
    }

    void OnEnable() => player.OnBeforeMove += OnBeforeMove;

    void OnDisable() => player.OnBeforeMove -= OnBeforeMove;
   
    void OnBeforeMove()
    {
        bool tryingCrouch = crouchAction.ReadValue<float>() > 0;

        // plays sound when crouch state changes
        if (tryingCrouch && !wasTryingCrouch)
        {
            AudioManager.instance.PlaySFX(crouchDown, 0.7f);
        }
        else if (!tryingCrouch && wasTryingCrouch)
        {
            AudioManager.instance.PlaySFX(crouchUp, 0.7f);
        }

        wasTryingCrouch = tryingCrouch;

        var heightTarget = tryingCrouch ? crouchHeight : standingHeight;

        // check if trying to uncrouch when under a surface
        if (isCrouching && !tryingCrouch)
        {
            var castOrigin = transform.position + new Vector3(0, currentHeight / 2, 0);

            if (Physics.Raycast(castOrigin, Vector3.up, out RaycastHit hit, 0.2f))
            {
                var cellingDist = hit.point.y - castOrigin.y;
                heightTarget = Mathf.Max(currentHeight + cellingDist - 0.1f,crouchHeight);
            }
        }

        // only calculate this if able to crouch
        if (!Mathf.Approximately(heightTarget, currentHeight))
        {
            var crouchSpeed = Time.deltaTime * crouchTransitionSpeed;
            //updates player height
            currentHeight = Mathf.Lerp(currentHeight,heightTarget,crouchSpeed);
            var halfHeightDifference =new Vector3(0, (standingHeight - currentHeight) / 2, 0);
            var newCamPos = initalCamPos - halfHeightDifference;
            player.transformCamera.localPosition = newCamPos;
            player.Height = heightTarget;
        }

        // slows players walking down if crouching
        if (isCrouching)
        {
            player.movementMultiplier *= crouchSpeedMultiplier;
        }

    }

     
}
