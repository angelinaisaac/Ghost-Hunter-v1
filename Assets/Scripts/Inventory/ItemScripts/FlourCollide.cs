using UnityEngine;
using System.Collections;

public class FlourCollide : MonoBehaviour
{
    [SerializeField] public ParticleSystem flourParticles;
    [SerializeField] private float scatterDuration = 0.5f;
    [SerializeField] private float kickInterval = 0.1f;

    //accesses particle array
    private ParticleSystem.Particle[] particles;
    private bool scattering = false;
    private bool playerInside = false;
    private Transform player;
    private float kickTimer;



    private void Update()
    {
        if (!playerInside || player == null)
            return;

        kickTimer -= Time.deltaTime;

        if (kickTimer <= 0f)
        {
            MoveFlour(player);
            kickTimer = kickInterval;
        }
    }

    //when player runs into flour, scatter flour
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            player = other.transform;          
        }
    }

    //update variables when player leaves collider
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            player = null;
        }
    }

    private void MoveFlour(Transform player)
    {
        int count = flourParticles.particleCount;

        if (count == 0)
            return;

        //create new particle system if previous one did not meet requirements
        if (particles == null || particles.Length < count)
            particles = new ParticleSystem.Particle[count];

        int particleCount = flourParticles.GetParticles(particles);

        Vector3 playerPos = player.position;

        //access each inidivudal particle
        for (int i = 0; i < particleCount; i++)
        {

            //compare particle position with player position to determine direction
            Vector3 direction = particles[i].position - playerPos;
            direction.y = 0;

            if (direction.sqrMagnitude > 0.001f)
            {
                direction.Normalize();

                //add varitation to ensure scatter is not uniform
                direction += Random.insideUnitSphere * 0.3f;
                direction.y = Mathf.Abs(direction.y) * 0.5f;

                direction.Normalize();

                particles[i].velocity = direction * Random.Range(1f, 3f);
            }
        }

        flourParticles.SetParticles(particles, particleCount);

        // Start timer for freezing
        scattering = true;
        StartCoroutine(FreezeFlour());
    }
  
    private IEnumerator FreezeFlour()
    {
        yield return new WaitForSeconds(scatterDuration);

        // Get the particles at their current positions
        int particleCount = flourParticles.GetParticles(particles);

        // Stop all particle movement
        for (int i = 0; i < particleCount; i++)
        {
            particles[i].velocity = Vector3.zero;
        }
        flourParticles.SetParticles(particles, particleCount);

        // Stop the particle system from updating
        flourParticles.Pause();
    }
}
