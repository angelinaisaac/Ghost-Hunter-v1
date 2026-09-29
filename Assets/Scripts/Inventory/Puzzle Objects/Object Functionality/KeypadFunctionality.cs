using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class KeypadFunctionality : MonoBehaviour
{
    [SerializeField] private GameObject keypad;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera secondaryCamera;
    [SerializeField] private AudioListener mainListener;
    [SerializeField] private AudioListener secondaryListener;
    [SerializeField] private Canvas crosshair;
    float MaxDist = 10f;
    bool isInputting = false;
    public int inputtedCode = 000;
    public string correctCode = "294";
    public string inputtedCodeString = "";
    Vector3 savedPosition;
    Quaternion savedRotation;
    public static KeypadFunctionality Instance;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        //ensures correct start conditions
        if (mainCamera != null) mainCamera.enabled = true;
        if (mainListener != null) mainListener.enabled = true;
        if (secondaryCamera != null) secondaryCamera.enabled = false;
        if (secondaryListener != null) secondaryListener.enabled = false;
    }

    void Update()
    {
        //if the player is on the keypad camera and presses 'I' again, return to main camera
        if (isInputting)
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                Player.Instance.transform.position = savedPosition;
                Player.Instance.transform.rotation = savedRotation;
                Player.Instance.updatingRotation = false;
                Player.Instance.updatingMovement = false;
                
                toggleCameras();
                isInputting = false;
            }
            return;
        }

        //if the player is looking at the keypad and presses 'I' go to secondary camera
        if (mainCamera != null && mainCamera.enabled)
        {
            Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, MaxDist))
            {
                if (hit.collider.gameObject == keypad)
                {
                    if (Input.GetKeyDown(KeyCode.I))
                    {
                        savedPosition = Player.Instance.transform.position;
                        savedRotation = Player.Instance.transform.rotation;
                        Player.Instance.updatingRotation = true;
                        Player.Instance.updatingMovement = true;
                        toggleCameras();
                        isInputting = true;
                    }
                }
            }
        }
    }

    //toggles from main to secondary camera
    private void toggleCameras()
    {
        if (mainCamera !=null && secondaryCamera != null)
        {
            mainCamera.enabled = !mainCamera.enabled;
            mainListener.enabled = !mainListener.enabled;
            
            secondaryCamera.enabled = !secondaryCamera.enabled;
            secondaryListener.enabled = !secondaryListener.enabled;

            //disable crosshair when in secondary camera
            crosshair.enabled = !crosshair.enabled;
            bool switchingToKeypad = !isInputting;

            //show cursor when in secondary camera
            Cursor.visible = switchingToKeypad;
            Cursor.lockState = switchingToKeypad ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}
