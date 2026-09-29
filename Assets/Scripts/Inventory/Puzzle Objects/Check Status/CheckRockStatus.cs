using UnityEngine;

public class CheckRockStatus : MonoBehaviour
{
    [SerializeField] private LayerMask bottleLayer;
    [SerializeField] private AudioClip clip;
    private float maxDist = 1.5f;

    //returns true if rock has hit bottle
    public bool hasHitBottle()
    {
        return Physics.Raycast(transform.position, Vector3.down, maxDist, bottleLayer);
    }

    //play sfx if rock hits the bottle
    void Update()
    {
        if (hasHitBottle())
        {
            SFXPlayer.Instance.PlaySFXVol(clip, 0.8f);
        }
    }
}
