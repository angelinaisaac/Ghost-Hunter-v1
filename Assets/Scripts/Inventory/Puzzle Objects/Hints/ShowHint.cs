using System;
using TMPro;
using UnityEngine;

public class ShowHint : MonoBehaviour
{
    public GameObject hintParent;
    public TextMeshPro itemHintText;
    [SerializeField] private Camera MainCam;
    [SerializeField] public float maxDist;
    [SerializeField] private float distance = 10;

    private void Awake()
    {
        if (MainCam == null)
        {
            MainCam = Camera.main;
        }
    }

    void Update()
    {
        //show hint if close enough to puzzle object
        Ray ray = new Ray(MainCam.transform.position, MainCam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDist))
        {
            Hint pObj = hit.collider.GetComponentInParent<Hint>();
            if (hit.distance >= distance) 
            {
                hintParent.SetActive(false);
                return;
            }
            else if (hit.distance < distance && pObj != null && pObj.puzzleobject != null) 
            {
                hintParent.SetActive(true);
                itemHintText.text = pObj.puzzleobject.hint;
                return;
            }
        }
    }
}
