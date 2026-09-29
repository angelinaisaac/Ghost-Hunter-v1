using TMPro;
using UnityEngine;
using System.Collections;

public class KeypadButtons : MonoBehaviour
{
    public int buttonFunctionality;
    [SerializeField] private TextMeshPro keypadText;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private AudioListener mainListener;
    [SerializeField] private AudioListener pulleyListener;
    [SerializeField] private Camera pulleyCamera;
    [SerializeField] private GameObject pulley;
    [SerializeField] private AudioClip revealClip;
    [SerializeField] private AudioClip pressClip;
    [SerializeField] private AudioClip incorrectClip;
    [SerializeField] private AudioClip correctClip;
    [SerializeField] private AudioClip clearClip;
    [SerializeField] private Canvas crosshair;
    Vector3 savedPosition;
    Quaternion savedRotation;
    string correct = "294";

    void Start()
    {
        //ensures correct start conditions
        if (mainCamera != null) mainCamera.enabled = true;
        if (mainListener != null) mainListener.enabled = true;
        if (pulleyCamera != null) pulleyCamera.enabled = false;
        if (pulleyListener != null) pulleyListener.enabled = false;
    } 
    private void OnMouseDown()
    {
        //clears input
        if(buttonFunctionality == 11)
        {
            KeypadFunctionality.Instance.inputtedCodeString = "000";
            keypadText.text = KeypadFunctionality.Instance.inputtedCodeString;
            SFXPlayer.Instance.PlaySFX(clearClip);
        }
        //submits answer
        else if (buttonFunctionality == 12)
        {
            //checks if answer is correct
            if (keypadText.text == correct)
            {
                StartCoroutine(RevealPulley());
            }
            else
            {
                StartCoroutine(IncorrectAnswer());
            }
        }
        //records number input
        else
        {
            SFXPlayer.Instance.PlaySFX(pressClip);
            KeypadFunctionality.Instance.inputtedCodeString += buttonFunctionality;
            keypadText.text = KeypadFunctionality.Instance.inputtedCodeString;
        }
    }

    void Update()
    {
        //clear input if input goes over length of 3
        if (KeypadFunctionality.Instance.inputtedCodeString.Length >= 3) KeypadFunctionality.Instance.inputtedCodeString = "";

    }

    private IEnumerator RevealPulley()
    {
        //flash green
        SFXPlayer.Instance.PlaySFX(correctClip);
        keypadText.color = Color.green;
        yield return new WaitForSeconds(0.5f);
        keypadText.color = Color.white;
        yield return new WaitForSeconds(0.5f);
        keypadText.color = Color.green;
        //switch to pulley camera
        mainCamera.enabled = false;
        mainListener.enabled = false;
        pulleyCamera.enabled = true;
        pulleyListener.enabled = true;
        //save player postion/rotation
        savedPosition = Player.Instance.transform.position;
        savedRotation = Player.Instance.transform.rotation;
        //freeze ability to update rotation/movement
        Player.Instance.updatingRotation = true;
        Player.Instance.updatingMovement = true;
        //show pulley spawn in 
        yield return new WaitForSeconds(1f);
        SFXPlayer.Instance.PlaySFX(revealClip);
        pulley.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        //return to main camera
        mainCamera.enabled = true;
        mainListener.enabled = true;
        pulleyCamera.enabled = false;
        pulleyListener.enabled = false;
        //return to original position/rotation
        Player.Instance.transform.position = savedPosition;
        Player.Instance.transform.rotation = savedRotation;
        //allow for updates in movement and rotation
        Player.Instance.updatingRotation = false;
        Player.Instance.updatingMovement = false;
        //return to mouse state
        Cursor.visible = false;
        Cursor.lockState = Cursor.lockState;
        crosshair.enabled = true;
    }

    //flashes red and clears input when answer is incorrect
    private IEnumerator IncorrectAnswer()
    {
        SFXPlayer.Instance.PlaySFX(incorrectClip);
        keypadText.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        keypadText.color = Color.white;
        yield return new WaitForSeconds(0.5f);
        keypadText.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        keypadText.color = Color.white;
        KeypadFunctionality.Instance.inputtedCodeString = "000";
        keypadText.text = KeypadFunctionality.Instance.inputtedCodeString;
    }
}
