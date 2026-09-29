using UnityEngine;

public class LockFunctionality : MonoBehaviour, IInteractable
{
    private PuzzleObject Lock;
    Player player;
    Vector3 savedPosition;
    Quaternion savedRotation;
    public Camera mainCamera;
    public Camera lockCamera;
    private Transform objectToRotate;

    private void Start()
    {
        //ensures start conditions are correct
        if (lockCamera != null) lockCamera.enabled = false;
        if (mainCamera != null) mainCamera.enabled = true; 
    }

    public void Interact()
    {
        //enables secondary camera (lock camera)
        if (lockCamera == null || mainCamera == null)
        {
            return;
        }

        lockCamera.enabled = true;
        mainCamera.enabled = false;
        objectToRotate = transform; 
    }

    public void Deactivate()
    {
        if (lockCamera == null || mainCamera == null) return;
        // Switch back to main camera
        mainCamera.enabled = true;
        lockCamera.enabled = false;
    }
}
