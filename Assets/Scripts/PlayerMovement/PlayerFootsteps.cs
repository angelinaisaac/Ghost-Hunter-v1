using System.Collections;
using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public static PlayerFootsteps Instance;
    private PlayerSprint playerSprint;
    public AudioClip leftFootSound;
    public AudioClip rightFootSound;
    private Player movement;
    private bool leftFoot = true;
    public float volume = 0.6f;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        movement = GetComponent<Player>();
        playerSprint = GetComponent<PlayerSprint>();
        StartCoroutine(PlayFootsteps());
    }

    IEnumerator PlayFootsteps()
    {
        while (true)
        {
            //while walking alternate footstep soundeffects
            if (movement.isMoving && movement.isGrounded)
            {
                if (leftFoot)
                {
                      AudioManager.instance.PlaySFX(leftFootSound, volume);
                }
                else
                {
                       AudioManager.instance.PlaySFX(rightFootSound, volume);
                }
                leftFoot = !leftFoot;

                //if player is sprinting speed up soundeffect
                if (playerSprint.isSprint)
                {
                    yield return new WaitForSeconds(0.3f);
                }
                //otherwise keep it at normal pace
                else
                {
                    yield return new WaitForSeconds(0.5f);
                }
            }
            else
            {
                yield return null;
            }
        }
    }
}
