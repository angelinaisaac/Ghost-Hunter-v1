using TMPro;
using UnityEngine;

public class FlourFunctionality : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI list;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private ItemScript flour;
    [SerializeField] private GameObject flourParticlesPrefab;
    [SerializeField] private float MaxDist = 5f;
    Quaternion TargetRoatation = Quaternion.Euler(90f, 0f, 0f);
    private bool flourSpilt = false;


    //spawn flour in if conditions are right (looking at ground with flour i
    void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, MaxDist, groundLayer))
        {
                if ( Input.GetMouseButtonDown(0) && Inventory.Instance.currentItem != null && !flourSpilt && PurchaseItem.Instance.boughtAdditionalItem)
                {
                    Instantiate(flourParticlesPrefab, hit.point, TargetRoatation);
                    list.text = "<s>Create ghost tracks with flour</s>";
                    //update puzzle manager
                    PuzzleManager.Instance.L3FinalScore += 20;
                    PuzzleManager.Instance.L3PuzzlesCompleted++;
                    PuzzleManager.Instance.CompletePuzzle(20);
                    Inventory.Instance.ConsumeEquippedItem();
                    flourSpilt = true;
                }
                else 
                {
                    return;
                }
            }
    }
}
