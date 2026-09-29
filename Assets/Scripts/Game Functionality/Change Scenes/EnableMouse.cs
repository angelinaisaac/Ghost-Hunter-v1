using UnityEngine;

public class EnableMouse : MonoBehaviour
{
    void Start()
    {
        //unlocks cursor and makes it visible
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
