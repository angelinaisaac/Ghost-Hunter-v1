using TMPro;
using UnityEngine;

public class RegisterFunctionality : MonoBehaviour
{
    [SerializeField] private TextMeshPro total;
    [SerializeField] private GameObject register;
    [SerializeField] private ItemScript dollar;
    [SerializeField] private ItemScript quarter;
    [SerializeField] private ItemScript stone;
    [SerializeField] private AudioClip coinClip;
    [SerializeField] private AudioClip billClip;
    float MaxDist = 10f;
    double trackTotal = 0.00;
    bool itemAdded = false;

    void Update()
    {
        //adds currency to register if register is clicked on with currency equipped
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, MaxDist))
        {
            if (hit.collider.gameObject == register)
            {
                //updates register total based on the currency used
                if(Inventory.Instance.currentItem == dollar && Input.GetMouseButton(0))
                {
                    trackTotal += 1.00;
                    total.text = trackTotal.ToString("0.00")+'$';
                    Inventory.Instance.ConsumeEquippedItem();
                    SFXPlayer.Instance.PlaySFX(billClip);
                }
                else if (Inventory.Instance.currentItem == quarter && Input.GetMouseButton(0))
                {
                    trackTotal += 0.25;
                    total.text = trackTotal.ToString("0.00") + '$';
                    Inventory.Instance.ConsumeEquippedItem();
                    SFXPlayer.Instance.PlaySFX(coinClip);
                }
            }
        }
        //if sufficient amount of money is put into the register, grant the player their item
        if (trackTotal >= 2.50 && !itemAdded)
        {
            Inventory.Instance.AddItem(stone,3);
            itemAdded = true;
        }
    }
}
