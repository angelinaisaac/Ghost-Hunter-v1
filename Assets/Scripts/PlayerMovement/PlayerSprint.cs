using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Player))]
public class PlayerSprint : MonoBehaviour
{
    [SerializeField] float speedMultiplier = 2f;
    Player player;
    PlayerInput playerInput;
    InputAction sprintingAction;
    public AudioClip leftFootSound;
    public AudioClip rightFootSound;
    public bool isSprint = false;

    private void Awake()
    {
        player = GetComponent<Player>();
        playerInput = GetComponent<PlayerInput>();
        sprintingAction = playerInput.actions["sprinting"];
    }

    void OnEnable() => player.OnBeforeMove += OnBeforeMove;
    void OnDisable() => player.OnBeforeMove -= OnBeforeMove;

    void OnBeforeMove()
    {
        //if conditions are not right, do not sprint
        var sprintInput = sprintingAction.ReadValue<float>();
        if (sprintInput == 0 || !player.isMoving)
        {
            isSprint = false;
            return;
        }

        //allows forward sprinting only
        isSprint = true;
        //increases movement multiplier
        var forwardMovement = Mathf.Clamp01(Vector3.Dot(player.transform.forward, player.velocity.normalized));
        var multiplier = Mathf.Lerp(1f, speedMultiplier, forwardMovement);
        player.movementMultiplier *= multiplier;
    }    
}
