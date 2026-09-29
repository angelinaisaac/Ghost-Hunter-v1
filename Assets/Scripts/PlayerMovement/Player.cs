using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    public static Player Instance;
    public Transform transformCamera;
    [SerializeField] float mouseSensitivity;
    [SerializeField] float movementSpeed;
    [SerializeField] float acceleration = 15f;
    [SerializeField] float mass = 1f;
    public event Action OnBeforeMove;
    public event Action<bool> OnGroundStateChange;
    internal float movementMultiplier;
    public bool isGrounded => controller.isGrounded;
    public bool isMoving => new Vector3(velocity.x, 0, velocity.z).magnitude > 0.1f;
    public bool updatingRotation;
    public bool updatingMovement;

    //get controller height
    public float Height
    {
        get => controller.height;
        set => controller.height = value;
    }
    internal Vector3 velocity;
    Vector2 look;
    CharacterController controller;
    bool wasGrounded;
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction sprintAction;
    InputAction lookAction;


    private void Awake()
    {
        Instance = this;
        controller = GetComponent<CharacterController>(); 
        playerInput = GetComponent<PlayerInput>();
        lookAction = playerInput.actions["look"];
        moveAction = playerInput.actions["move"];
        sprintAction = playerInput.actions["sprinting"];
    }

    void Start()
    {
        //Ensures mouse is hidden when playing
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        UpdateGround();
        UpdateLook();
        UpdateMovement();
        UpdateGravity();
    }

    void UpdateLook()
    { 
        if (updatingRotation) return;
        var lookInput = lookAction.ReadValue<Vector2>();
        //tracks mouse movement
        look.x += lookInput.x * mouseSensitivity;
        look.y += lookInput.y * mouseSensitivity;

        //disallows players from looking up/down 360 degrees
        look.y = Mathf.Clamp(look.y, -75f, 75f);

        //rotates camera/player
        transformCamera.localRotation = Quaternion.Euler(-look.y, 0, 0);
        transform.localRotation = Quaternion.Euler(0, look.x, 0);
    }

    Vector3 GetMovementInput()
    {
        //get move input
        var moveInput = moveAction.ReadValue<Vector2>();

        var input = new Vector3();
        input += transform.forward * moveInput.y;
        input += transform.right * moveInput.x;
        input = Vector3.ClampMagnitude(input, 1f);
        input *= movementSpeed* movementMultiplier;
        return input;
    }

    void UpdateMovement()
    {
        if (updatingMovement) return;
        movementMultiplier = 1f;
        OnBeforeMove?.Invoke();
        var input = GetMovementInput();
        var acc = acceleration * Time.deltaTime;
        //smooths movement
        velocity.x = Mathf.Lerp(velocity.x, input.x, acc);
        velocity.z = Mathf.Lerp(velocity.z, input.z, acc);

        //move player
        controller.Move(velocity*Time.deltaTime);
    }
    
    void UpdateGravity()
    {
        //calculate gravity
        var gravity = Physics.gravity * mass * Time.deltaTime;

        //small force if grounded, otherwise accurate gravity is applied to simulate falling
        velocity.y = controller.isGrounded ? -1f : velocity.y + gravity.y;
    }

    //update grounded state
    void UpdateGround()
    {
        if(wasGrounded != isGrounded)
        {
            OnGroundStateChange?.Invoke(isGrounded);
            wasGrounded = isGrounded;
        }
    }
}


