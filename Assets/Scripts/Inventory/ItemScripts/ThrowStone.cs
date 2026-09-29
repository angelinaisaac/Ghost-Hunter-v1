using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThrowStone : MonoBehaviour
{
    public static ThrowStone Instance;
    [SerializeField] private float minForce = 5f;
    [SerializeField] private float maxForce = 25f;
    [SerializeField] private float totalChargeTime = 2f;
    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private ItemScript rock;
    private Transform throwPoint;
    private float currentChargeTime;
    private bool isCharging;

    //for line estimate
    [SerializeField] private LineRenderer trajectoryLine;
    [SerializeField] private int trajectoryPoints = 30;
    [SerializeField] private float trajectoryTimeStep = 0.1f;
    [SerializeField] private AudioClip clip;
    private bool playedSFX = false;

    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        //accesses throw point (point attached to player from where rock should exit)
        throwPoint = GameObject.Find("Throw Point").transform;
    }

    void Update()
    {
        HandleThrowInput();

        //if charging show trajectory line, otherwise hide trajectory line
        if (isCharging)
        {
            ShowTrajectory();
        }
        else
        {
            trajectoryLine.enabled = false;
        }

        if (Input.GetMouseButtonUp(0))
        {
        }
    }

    //handles user interaction with rock
    private void HandleThrowInput()
    {
        //start charge if mouse down
        if (Input.GetMouseButtonDown(0) && Inventory.Instance.currentItem == rock)
        {
            StartCharge();
        }
        if (isCharging)
        {
            ChargePower();
        }
        //if mouse released and a charge has been collected, release the throw
        if(Input.GetMouseButtonUp(0) && isCharging)
        {
            ReleaseThrow();
        }
    }

    //starts charge
    private void StartCharge()
    {
        isCharging = true;
        currentChargeTime = 0f;
    }

    private void ChargePower()
    {
        //update timer
        currentChargeTime += Time.deltaTime;
        //prevents from charging past maximum
        currentChargeTime = Mathf.Clamp(currentChargeTime, 0f, totalChargeTime);

        // Calculate a visual 0 to 1 percentage string for debugging
        float chargePercentage = currentChargeTime / totalChargeTime;
        isCharging = true;
    }

    private void ReleaseThrow()
    {
        isCharging = false;

        //normalize charge so its between 0-1
        float normalizedCharge = currentChargeTime / totalChargeTime;
        float calculatedForce = Mathf.Lerp(minForce, maxForce, normalizedCharge);
        GameObject thrownRock = Instantiate(rockPrefab, throwPoint.position, throwPoint.rotation );

        Rigidbody rockRB = thrownRock.GetComponent<Rigidbody>();

        if(rockRB == null)
        {
            return;
        }

        //make direction slightly upwards
        Vector3 throwDirection = (throwPoint.forward + Vector3.up * 0.2f).normalized;

        //play sfx
        SFXPlayer.Instance.PlaySFXVol(clip, 0.5f);
        

        //apply force
        rockRB.AddForce(throwDirection * calculatedForce, ForceMode.Impulse);

        //consumes rock, can be picked up again though
        Inventory.Instance.ConsumeEquippedItem();
    }

    private void ShowTrajectory()
    {
        trajectoryLine.enabled = true;

        //calculate charge between 0-1
        float normalizedCharge = currentChargeTime / totalChargeTime;

        //convert charge into a force
        float calculatedForce = Mathf.Lerp(minForce, maxForce, normalizedCharge);

        //sets the starting position of trajectory
        Vector3 startPosition = throwPoint.position + throwPoint.forward * 0.5f;

        //calculates direction rock will be thrown
        Vector3 throwDirection = (throwPoint.forward + Vector3.up * 0.2f).normalized;

        //rocks inital velocity
        Vector3 initialVelocity = throwDirection * calculatedForce;

        //defines how many points LineRenderer should have
        trajectoryLine.positionCount = trajectoryPoints;

        //calculates the position of every point along the predicted trajectory
        for (int i = 0; i < trajectoryPoints; i++)
        {
            //calculates how much time has passed for the given point
            float time = i * trajectoryTimeStep;

            //calculates where rock should be at the given point in time
            //gravity pulls rock downward over time, making the trajectory curved
            Vector3 position = startPosition + initialVelocity * time + 0.5f * Physics.gravity * time * time;
            //give position to LineRenderer
            trajectoryLine.SetPosition(i, position);
        }
    }
}
