using System;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

public class LockHint : MonoBehaviour
{
    //puzzle object
    public PuzzleObject Lock;

    // hint
    public GameObject hintParent;
    public TextMeshPro itemHintText;

    //distance to detect  object
    public float maxDist = 5f;

    //save player position
    Vector3 savedPosition;
    Quaternion savedRotation;
    //flag if player is inspecting lock
    public bool inspecting;
    //lock rotation speed
    public float rotationSpeed = 150f;
    [SerializeField] private Transform inspectPoint;
    [SerializeField] private Camera MainCam;
    private Transform objectToRotate;
    private Quaternion savedObjectRotation;
    private float rotX;
    private float rotY;
    //rotation clamps
    [SerializeField] private float maxXRotation = 45f;
    [SerializeField] private float maxYRotation = 90f;
    private String originalHint;


    private void Awake()
    {
        if(MainCam == null)
        {
            MainCam = Camera.main;
        }
    }

    void Update()
    {
        rotateInspectedObj();
        originalHint = Lock.hint;
        if (MainCam == null) return;
        Ray ray = new Ray(MainCam.transform.position, MainCam.transform.forward);

        //check if player is close enough to view hint/inspect
        if (Physics.Raycast(ray, out RaycastHit hit, maxDist))
        {
            Hint pObj = hit.collider.GetComponent<Hint>();

            if (pObj != null && pObj.puzzleobject != null)
            {
                if (pObj.puzzleobject == null)
                {
                    return;
                }

                //show hint
                hintParent.SetActive(true);
                itemHintText.text = pObj.puzzleobject.hint;

                //interact with object
                if(Input.GetKeyDown(KeyCode.I))
                {
                    Rigidbody rb = Player.Instance.GetComponent<Rigidbody>();
                    LockFunctionality lockScript = hit.collider.GetComponentInParent<LockFunctionality>();
                    //enter inspection mode
                    if (!inspecting)
                    {
                        //clear hint in inspection mode;
                        Lock.hint = "";

                        //lock movement
                        savedPosition = Player.Instance.transform.position;
                        savedRotation = Player.Instance.transform.rotation;
                        Player.Instance.updatingRotation = true;
                        Player.Instance.updatingMovement = true;


                        //save rotation values
                        objectToRotate = hit.collider.GetComponentInParent<LockFunctionality>().transform;
                        savedObjectRotation = objectToRotate.localRotation;

                        //avoid snapping
                        rotX = objectToRotate.localEulerAngles.x;
                        if (rotX > 180)
                            rotX -= 360;

                        rotY = objectToRotate.localEulerAngles.y;
                        if (rotY > 180)
                            rotY -= 360;

                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;

                        //allow rigibody to be manipulated
                        if (rb != null)
                        {
                            rb.angularVelocity = Vector3.zero;
                            rb.linearVelocity = Vector3.zero;
                            rb.isKinematic = true;
                        }

                        float mouseX = Input.GetAxis("Mouse X");
                        transform.Rotate(
                            Vector3.up,
                            -mouseX * rotationSpeed * Time.deltaTime,
                            Space.World
                        );
                        hit.collider.GetComponentInParent<IInteractable>()?.Interact();
                        inspecting = true;
                    }
                    else
                    {
                        
                        if (rb != null) rb.isKinematic = false;


                        if (objectToRotate != null)
                        {
                            objectToRotate.rotation = savedObjectRotation;
                        }

                        //display hint
                        Lock.hint = "What does this lock say?";

                        //return to original position and rotation
                        Player.Instance.transform.position = savedPosition;
                        Player.Instance.transform.rotation = savedRotation;
                        Player.Instance.updatingRotation = false;
                        Player.Instance.updatingMovement = false;

                        //hide cursor
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;

                        if (lockScript != null)
                        {
                            lockScript.Deactivate();
                        }
                        inspecting = false;
                    }
                }
                return;
            }
            else
            {
                //if far away do not show hint
                hintParent.SetActive(false);
            }
        }

    }

    //rotates lock
    private void rotateInspectedObj()
    {
        if (!inspecting ) return;

        //allows player to rotate lock by using the mouse
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        rotY += -mouseX * rotationSpeed * Time.deltaTime;
        rotX += mouseY * rotationSpeed * Time.deltaTime;

        //clamps rotations
        rotX = Mathf.Clamp(rotX, -maxXRotation, maxXRotation);
        rotY = Mathf.Clamp(rotY, -maxYRotation, maxYRotation);
        transform.localRotation = Quaternion.Euler(rotX, rotY, 0);
    }
}


