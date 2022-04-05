using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    [Header("Basic Controls")]
    [SerializeField]
    private float startSpeed = 4.0f;
    [SerializeField]
    private float maxSpeed = 10.0f;
    private float currSpeed = 0f;
    [SerializeField]
    private float yawSpeed = 3.0f;
    [SerializeField]
    private float pitchSpeed = 1.0f;
    [SerializeField]
    private float pitchRange = 50;
    [SerializeField]
    [Tooltip("How much a collision will slow down the plane's speed.")]
    private float collisionSlowMultiplier = 0.01f;

    [Header("Aerodynamics")]
    [SerializeField]
    private float airDrag = 1.0f;
    [SerializeField]
    private float gravity = 1.0f;
    [SerializeField]
    private float climbingDeceleration = 0.01f;
    [SerializeField]
    private float divingAcceleration = 0.08f;
    [SerializeField]
    private float horizontalSpeedForMaxLift = 5.0f;

    [Header("Boost")]
    [SerializeField]
    private float speedBoost = 2f;
    [SerializeField]
    private float initialBoostMultiplier = 9f;
    private float boostToDrop = 0f;
    private float boostRemaining = 0f;
    private float timeOfLastBoost = 0f;
    [SerializeField]
    private float boostFalloff = 0.085f;

    [Header("Suspension")]
    [SerializeField]
    private bool startSuspended = true;
    [SerializeField]
    private float suspendedSlowMultiplier = 7f;
    [SerializeField]
    private float rotateWithCameraSpeed = 5f;
    private bool suspended = false;
    private float nextBoost = -1f;
    [SerializeField]
    private float speedBoostMultiplier = 2f;


    private CharacterController charController;
    private SpeedEffects speedEffects;
    private Vector3 currPos;
    private Vector3 prevPos;


    void Start()
    {
        charController = GetComponent<CharacterController>();
        speedEffects = GetComponent<SpeedEffects>();
        pitchRange = Mathf.Clamp(pitchRange, 0, 90);
        initialBoostMultiplier = Mathf.Max(initialBoostMultiplier, 1f);
        currPos = transform.position;
        prevPos = transform.position;
        if (startSuspended)
            currSpeed = 0f;
        else
            currSpeed = startSpeed;

        CameraManager.Instance.Initialize(startSuspended);
    }

    void Update()
    {
        if (Time.timeScale == 0f)
            return;

        if (suspended && Input.GetKeyDown(KeyCode.Space))
        {
            Unsuspend();
        }

        if (!suspended || (suspended && currSpeed != 0))
        {
            Vector3 moveVector = Vector3.zero;
            Vector3 lookVector = Vector3.zero;

            // update boost
            if (boostRemaining > 0f && !suspended)
            {
                float boostReduction = Time.deltaTime / boostFalloff * boostToDrop;
                boostReduction = Mathf.Clamp(boostReduction, 0f, boostRemaining);
                //float boostReduction = boostRemaining - Mathf.Lerp(boostToDrop, 0f, (Time.timeSinceLevelLoad - timeOfLastBoost) / boostFalloff);
                currSpeed -= boostReduction;
                boostRemaining -= boostReduction;
            }
            else boostRemaining = 0f;

            // gravity
            if (!suspended)
            {
                currPos = transform.position;
                Vector3 vel = (currPos - prevPos) / Time.deltaTime;
                prevPos = currPos;
                Vector3 xzVel = vel;
                xzVel.y = 0;
                float xzSpeed = xzVel.magnitude;
                float gravScale = 1 - Mathf.Clamp(xzSpeed / horizontalSpeedForMaxLift, 0f, 1f);

                // have gravity constantly pull plane down (with adjustment based on current xz-plane speed to simulate lift)
                Vector3 gravVec = Vector3.zero;
                gravVec.y = -1f * gravity;
                moveVector += gravVec * gravScale;

                // when not grounded, also adjust speed based on how much the plane is facing up or down,
                // and have gravity influence the plane's orientation/look direction to a small degree
                // (will help plane turn downward to gain speed when speed becomes 0 midair)
                if (!charController.isGrounded)
                {
                    lookVector += gravVec * gravScale * 0.005f;

                    float dot = Vector3.Dot(transform.forward, gravVec);
                    float gravSpeedEffect = climbingDeceleration;
                    if (dot > 0)
                        gravSpeedEffect = divingAcceleration;
                    currSpeed += dot * gravSpeedEffect;
                }
            }

            // air drag
            if (!suspended)
                currSpeed -= airDrag * Time.deltaTime;

            // move based on current orientation, speed, and input
            if (!suspended)
                currSpeed = Mathf.Clamp(currSpeed - boostRemaining, 0f, maxSpeed) + boostRemaining;
            else // bring plane to halt if suspended
            {
                currSpeed = Mathf.Lerp(currSpeed, 0f, Time.deltaTime * suspendedSlowMultiplier);
            }
            float turnScale = currSpeed/3f;
            Vector3 yaw = Input.GetAxis("Horizontal") * transform.right * yawSpeed * Time.deltaTime * turnScale;
            Vector3 pitch = -1 * Input.GetAxis("Vertical") * transform.up * pitchSpeed * Time.deltaTime * turnScale;
            Vector3 dir = yaw;
            float rotSpeed = 100f;
            if (currSpeed > 0.01f)
            {
                moveVector += transform.forward * currSpeed;
                lookVector += transform.forward * currSpeed;

                // yaw/pitch input
                float maxPitch = Quaternion.LookRotation(lookVector + yaw + pitch).eulerAngles.x;
                bool tooHigh = (maxPitch > 270) && (maxPitch < (360 - pitchRange));
                bool tooLow = (maxPitch > pitchRange) && (maxPitch < 90);
                float pitchDir = Vector3.Dot(pitch, Vector3.up);
                if (  (!tooHigh && !tooLow)  ||  (tooLow && pitchDir > 0)  ||  (tooHigh && pitchDir < 0)  )
                {
                    dir += pitch;
                }
                moveVector += dir;
                lookVector += dir;
            }
            else
            {
                currSpeed = 0;
                rotSpeed = 2f;
            }

            if (lookVector.magnitude != 0 && !suspended)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookVector), Time.deltaTime * rotSpeed);
            }
            charController.Move(moveVector * Time.deltaTime);
        }

        if (suspended)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, CameraManager.Instance.GetCameraRotation(), Time.deltaTime * rotateWithCameraSpeed);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Vector3 vel = currSpeed * hit.moveDirection;
        currSpeed -= collisionSlowMultiplier * Vector3.Dot(vel, -1 * hit.normal);
    }

    public float GetSpeed() { return currSpeed; }

    public bool IsGrounded() { return charController.isGrounded; }

    public void Boost(float boostAmount = -1f, float initialMultiplier = -1f)
    {
        if (boostAmount == -1f) boostAmount = speedBoost;
        if (initialMultiplier == -1f) initialMultiplier = initialBoostMultiplier;

        speedEffects.ApplyBoostColor();

        if (boostRemaining > 0)
            currSpeed -= boostRemaining;
        
        // extra boost if very slow
        float extraBoost = Mathf.Max(boostAmount - currSpeed, 0f);

        float baselineBoost = Mathf.Min(boostAmount + extraBoost + currSpeed, maxSpeed) - currSpeed;

        float boostToMultiply = Mathf.Min(boostAmount + currSpeed, maxSpeed) - currSpeed;
        currSpeed += initialMultiplier * boostToMultiply + (baselineBoost - boostToMultiply);
        boostToDrop = (initialMultiplier - 1) * boostToMultiply;
        boostRemaining = boostToDrop;
        timeOfLastBoost = Time.timeSinceLevelLoad;
    }

    public void Suspend(float boostAmount = -1f)
    {
        suspended = true;
        CameraManager.Instance.EnableFreelookCamera();
        speedEffects.ApplySuspendColor();
        nextBoost = boostAmount;
    }

    public void Unsuspend()
    {
        suspended = false;
        CameraManager.Instance.EnableFollowCamera();
        float boostAmount = speedBoost * speedBoostMultiplier;
        if (nextBoost != -1f)
            boostAmount = nextBoost;
        Boost(boostAmount);
        nextBoost = -1f;
    }

    public bool IsSuspended()
    {
        return suspended;
    }
}
