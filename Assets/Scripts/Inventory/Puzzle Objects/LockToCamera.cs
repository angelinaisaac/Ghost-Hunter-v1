using UnityEngine;
using TMPro;

public class LockToCamera : MonoBehaviour
{
    public Transform cameraTransform;
    public Vector3 offset = new Vector3(0, 0, 2f);
    void LateUpdate()
    {
        //used to position hint text in front of camera
        // locks position in front of camera
        transform.position = cameraTransform.position + cameraTransform.forward * offset.z + cameraTransform.right * offset.x + cameraTransform.up * offset.y;

        // ensures text faces the camera
        transform.LookAt(cameraTransform.position);
        transform.Rotate(0, 180, 0); 
    }
}
