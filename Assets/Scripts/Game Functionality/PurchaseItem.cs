using TMPro;
using UnityEngine;

public class PurchaseItem : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private AudioSource src;
    public static PurchaseItem Instance;
    [SerializeField] private int price;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI soldText;

    public bool boughtAdditionalItem = false;

    void Awake()
    {
        Instance = this;
    }

    //subtract points and add update bool flag(to grant item later) when player purchases item
    public void Purchase()
    {
        //ensure player has enough points
        if(PuzzleManager.Instance.optionalPuzzleScore >= price)
        {
            PuzzleManager.Instance.optionalPuzzleScore -= price;
            boughtAdditionalItem = true;
            src.PlayOneShot(clip);
            //update UI to show item has been purchased
            soldText.gameObject.SetActive(true);
            priceText.enabled = false;
        }
        else
        {
            return;
        }
    }
}
