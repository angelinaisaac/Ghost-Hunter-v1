using UnityEngine;
using UnityEngine.Audio;

public class KeyFunctionality : MonoBehaviour, IUsable
{
    private GameObject gateObject;
    private GameObject lockObject;
    private Inventory inventory;
    [SerializeField] ItemScript key;
    public float maxDist = 5f; 
    private Camera MainCam;
    private SFXPlayer SFXPlayer;

    public void Use()
    {
        MainCam = Camera.main;
        gateObject = GameObject.Find("gate");
        lockObject = GameObject.Find("lock");
        inventory = FindAnyObjectByType<Inventory>();

        //check if player is looking at lock
        Ray ray = new Ray(MainCam.transform.position, MainCam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDist)) {
            if(hit.collider.gameObject != lockObject)
            {
                return;
            }

            SFXPlayer.Instance.PlaySFX(key.SFX);
            //if so, remove gate so player can access chandelier
            gateObject.SetActive(false);
            lockObject.SetActive(false);
            //remove hint
            LockHint hints = lockObject.GetComponent<LockHint>();
            hints.hintParent.SetActive(false);
            //consume item
            inventory.ConsumeEquippedItem();
        }
    }
    
}
