using System.Runtime.CompilerServices;
using UnityEngine;

public class PrefabSFX : MonoBehaviour
{
    private SFXPlayer sfxPlayer;
    [SerializeField] private AudioClip chandelierSound;

    void Update()
    {
        //due to bug seperate script handles chandelier sound effect
        if (!gameObject.TryGetComponent<HingeJoint>(out HingeJoint hinge))
        {
            sfxPlayer.PlaySFX(chandelierSound);
        }
    }
}
