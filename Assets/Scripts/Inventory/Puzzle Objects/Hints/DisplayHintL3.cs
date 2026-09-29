using System;
using TMPro;
using UnityEngine;

public class DisplayHintL3 : MonoBehaviour
{
    //new method to display hints. only in level 3 due to time constraints
    [SerializeField] private GameObject hint;
    [SerializeField] private GameObject puzzleObject;
    float maxDist = 10f;

    void Update()
    {
        //show hint when player is looking at puzzle object
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDist))
        {
            if (hit.collider.gameObject == puzzleObject)
            {
                hint.SetActive(true); 
            }
        }
        //deactivate hint when player is not looking at puzzle object
        else
        {
            hint.SetActive(false);
        }
    }
}
