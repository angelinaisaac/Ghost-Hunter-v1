using UnityEngine;

public class BillboardObject : MonoBehaviour
{
    private Transform mainCam;

    void Start()
    {
        mainCam = Camera.main.transform;
    }

    private void LateUpdate()
    {
        //makes object face camera
        transform.LookAt(mainCam);
        //rotated 180 degrees so text is not displayed backwards
        transform.RotateAround(transform.position, transform.up, 180f);
    }
}
