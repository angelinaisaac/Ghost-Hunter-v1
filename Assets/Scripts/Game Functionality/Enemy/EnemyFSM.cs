using System.Collections;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyFSM : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private AudioClip clip;
    [SerializeField] private AudioSource primarySource;
    [SerializeField] private AudioSource secondarySource;
    [SerializeField] private AudioSource thirdSource;
    [SerializeField] private float musicFadeDuration = 1.5f;
    [SerializeField] private float primaryMusicVolume = 0.6f;
    [SerializeField] private float chaseMusicVolume = 0.6f;
    private Coroutine musicCoroutine;
    public enum State { Idle, Roam, Chase}
    public State currentState;
    public Transform player;
    public float visionRange = 8f;
    public float roamRadius = 10f;
    public float idleDuration = 1f;
    private NavMeshAgent agent;
    private float timer;
    private Coroutine fadeCoroutine;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if(player == null)
        {
            //access player
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        //start on idle state by default
        TransitionToState(State.Idle);
    }
    void Update()
    {
        CheckTransitions();
        ExecuteStateBehaviour();
    }

    void TransitionToState(State newState)
    {
        //doesnt restart audio if already in current state
        if (currentState == newState) return;
        currentState = newState;

        if(currentState == State.Chase)
        {
            //plays from independent source so fading music doesnt impact it
            thirdSource.PlayOneShot(clip);

            //crossfade into chase music
            StartMusicCrossfade(0f, chaseMusicVolume);
        }
        else
        {
            //crossfade to normal music
            StartMusicCrossfade(primaryMusicVolume, 0f);
        }

        //if transitioned to idle reset timer and reset path
        if (currentState == State.Idle)
        {
            agent.ResetPath();
            timer = idleDuration;
        }
        //get random destination for roaming
        else if(currentState == State.Roam)
        {
            Vector3 newPos = GetRandomNavMeshLocation(roamRadius);
            agent.SetDestination(newPos);
        }
    }

    void CheckTransitions()
    {
        //distance from player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        //if player is close enough, switch to chase
        if(distanceToPlayer <= visionRange)
        {
            if(currentState != State.Chase)
            {
                
                TransitionToState(State.Chase);
            }
            return;
        }

        //if chasing but player got away, return to idle
        if(currentState == State.Chase && distanceToPlayer > visionRange)
        {
            TransitionToState(State.Idle);
            return;
        }

        //if idle timer has run out, return to idle
        if(currentState == State.Idle)
        {
            timer -= Time.deltaTime;
            if(timer <= 0f)
            {
                TransitionToState(State.Roam);
            }
        }
        //once ai has reached its random navmesh location it returns to idle
        else if(currentState == State.Roam)
        {
            if(!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                TransitionToState(State.Idle);
            }
        }
    }

    void ExecuteStateBehaviour()
    {
        //set appropriate destination/animation given the state
        if(currentState == State.Chase)
        {
            agent.SetDestination(player.position);
            agent.speed = 10;
            animator.SetInteger("MovementState", 2);
        }
        else if(currentState == State.Roam)
        {
            agent.speed = 4;
            animator.SetInteger("MovementState", 1);
        }
        else if(currentState == State.Idle)
        {
            animator.SetInteger("MovementState", 0);
        }

        //if the enemy intersects with the player in any state, game over
        if(currentState == State.Chase|| currentState == State.Idle || currentState == State.Roam)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer < 1.5)
            {
                SceneManager.LoadScene("Caught Screen");
            }
        }
    }

    //choose random location within the navmesh for enemy to walk to while in roam state
    Vector3 GetRandomNavMeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, radius, 1);
        return hit.position;
    }

    void StartMusicCrossfade(float primaryTarget, float chaseTarget)
    {
        //stop any previous fades
        if(musicCoroutine != null)
        {
            StopCoroutine(musicCoroutine);
        }

        musicCoroutine = StartCoroutine(CrossfadeMusic(primaryTarget, chaseTarget));
    }

    IEnumerator CrossfadeMusic(float primaryTarget, float chaseTarget)
    {
        //ensures default music is playing
        if (!primarySource.isPlaying)
        {
            primarySource.Play();
        }

        //start chase music
        if(chaseTarget >0f && !secondarySource.isPlaying)
        {
            secondarySource.volume = 0f;
            secondarySource.Play();
        }

        float startPrimary = primarySource.volume;
        float startChase = secondarySource.volume;
        float elapsed = 0f;

        //fade volume
        while(elapsed < musicFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / musicFadeDuration;

            primarySource.volume = Mathf.Lerp(startPrimary, primaryTarget, t);
            secondarySource.volume = Mathf.Lerp(startChase, chaseTarget, t);
            yield return null;
        }
        //ensures target is hit
        primarySource.volume = primaryTarget;
        secondarySource.volume = chaseTarget;

        //pause secondary music
        if(chaseTarget <= 0f)
        {
            secondarySource.Pause();
        }
        musicCoroutine = null;
    }
}
