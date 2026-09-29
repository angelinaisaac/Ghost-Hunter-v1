using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class PlayerJump : MonoBehaviour
{
    [SerializeField] float jumpSpeed = 3f;
    [SerializeField] float jumpBufferTime = .05f;
    [SerializeField] float JumpGraceTime = .15f;
    [SerializeField] private AudioClip jumpSound;
    Player player;
    PlayerInput playerInput;
    InputAction jumpAction;

    bool jumpTry;
    float lastJumpTime = -10f;
    float lastGroundedTime = -10f;

    private void Awake()
    {
        player = GetComponent<Player>();
        playerInput = GetComponent<PlayerInput>();
        jumpAction = playerInput.actions["jump"];
    }

    void OnEnable()
    {
        player.OnBeforeMove += OnBeforeMove;
        player.OnGroundStateChange += OnGroundStateChange;
        jumpAction?.Enable();
    }

    void OnDisable()
    {
        player.OnBeforeMove -= OnBeforeMove;
        player.OnGroundStateChange -= OnGroundStateChange;
        jumpAction?.Disable();
    }

    void Update()
    {
        // register jump input
        if (jumpAction.WasPressedThisFrame())
        {
            jumpTry = true;
            lastJumpTime = Time.time;
        }
    }

    void OnBeforeMove()
    {
        bool wasTryingJump = Time.time - lastJumpTime < jumpBufferTime;
        bool wasGrounded = Time.time - lastGroundedTime < JumpGraceTime;
        bool needsJump = jumpTry || wasTryingJump;
        bool isOrWasGrounded =player.isGrounded || wasGrounded;

        //if conditions are right, jump
        if (needsJump && isOrWasGrounded)
        {
            AudioManager.instance.PlaySFX(jumpSound, 0.7f);
            player.velocity.y = jumpSpeed;

            // clear the buffered jump
            lastJumpTime = -10f;
        }
        jumpTry = false;
    }

    void OnGroundStateChange(bool isGrounded)
    {
        //record the time the player lands
        if (isGrounded)
        {
            lastGroundedTime = Time.time;
        }
    }
}