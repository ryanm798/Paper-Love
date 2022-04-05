using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedEffects : MonoBehaviour
{
    public float maxEffectSpeed = 10f;
    public float minEffectSpeed = 1f;
    public float maxTurnEffectSpeed = 2f;
    public float maxRollAngle = 30f;
    public float rollSpeed = 5f;
    public float maxPitchAngle = 20f;
    public float pitchSpeed = 5f;

    [SerializeField]
    private Transform planeModel;
    private Vector3 defaultModelRotation = Vector3.zero;

    [SerializeField]
    private Material paperMaterial;
    private Color defaultColor;
    public Color boostColor;
    public Color suspendColor;
    private Color targetColor;
    [SerializeField]
    private float colorChangeSpeed = 1f;

    [SerializeField]
    private TrailRenderer[] windTrails;
    [SerializeField]
    private Material trailMaterial;

    public float maxWindVolume = 0.1f;
    public float maxVolumeSpeed = 10f;
    public float minVolumeSpeed = 1f;

    private PlaneController planeController;
    private Vector3 currPos;
    private Vector3 prevPos;
    private Quaternion currRot;
    private Quaternion prevRot;

    void Start()
    {
        planeController = GetComponent<PlaneController>();
        defaultModelRotation = planeModel.localEulerAngles;
        currPos = transform.position;
        prevPos = transform.position;
        currRot = transform.rotation;
        prevRot = transform.rotation;
        AudioManager.Instance.SetWindVolume(0f);
        defaultColor = paperMaterial.color;
        targetColor = defaultColor;
    }

    void OnDestroy()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetWindVolume(0f);
        paperMaterial.color = defaultColor;
    }

    void LateUpdate()
    {
        if (Time.timeScale == 0f || Time.deltaTime == 0f)
            return;
        
        /*** Turn Speed Effects ***/
        currRot = transform.rotation;
        Quaternion deltaRot = currRot * Quaternion.Inverse(prevRot);
        deltaRot.ToAngleAxis(out var angle, out var axis);
        angle *= Mathf.Deg2Rad;
        Vector3 angularVelRad = (1.0f / Time.deltaTime) * angle * axis;
        Vector3 localAngularVelRad = transform.InverseTransformDirection(angularVelRad).normalized * angularVelRad.magnitude;

        float zRotAngle = Mathf.Clamp((-1f * angularVelRad.y) / maxTurnEffectSpeed, -1f, 1f) * maxRollAngle;
        float xRotAngle = Mathf.Clamp((2f * localAngularVelRad.x) / maxTurnEffectSpeed, -1f, 1f) * maxPitchAngle;
        float rSpeed = rollSpeed;
        float pSpeed = pitchSpeed;
        if (planeController.IsSuspended())
        {
            zRotAngle /= 1.5f;
            rSpeed *= 2.5f;
            xRotAngle /= 1f;
            pSpeed *= 3.5f;
        }
        Vector3 targetEuler = new Vector3(defaultModelRotation.x, defaultModelRotation.y, defaultModelRotation.z + zRotAngle);
        planeModel.localRotation = Quaternion.Slerp(planeModel.localRotation, Quaternion.Euler(targetEuler), Time.deltaTime * rSpeed);
        targetEuler = new Vector3(defaultModelRotation.x + xRotAngle, defaultModelRotation.y, planeModel.localEulerAngles.z);
        planeModel.localRotation = Quaternion.Slerp(planeModel.localRotation, Quaternion.Euler(targetEuler), Time.deltaTime * pSpeed);


        /*** Linear Speed Effects ***/
        currPos = transform.position;
        float speed = (currPos - prevPos).magnitude / Time.deltaTime;
        float effectScale = Mathf.Clamp((speed - minEffectSpeed) / (maxEffectSpeed - minEffectSpeed), 0, 1);

        // wind sfx volume
        float volumeScale = Mathf.Clamp((speed - minVolumeSpeed) / (maxVolumeSpeed - minVolumeSpeed), 0, 1);
        AudioManager.Instance.SetWindVolume(Mathf.Lerp(AudioManager.Instance.GetWindVolume(), volumeScale * volumeScale * maxWindVolume, Time.deltaTime * 30f));
        
        // wind trail vfx
        foreach (TrailRenderer tr in windTrails)
        {
            //tr.widthCurve.keys[1] = new Keyframe(tr.widthCurve.keys[1].time, effectScale);
            tr.emitting = !(effectScale == 0);
        }
        trailMaterial.color = new Color(trailMaterial.color.r, trailMaterial.color.g, trailMaterial.color.b, effectScale);

        prevPos = currPos;
        prevRot = currRot;


        // paper material coloring
        if (paperMaterial.color != targetColor)
        {
            if (targetColor == boostColor)
            {
                paperMaterial.color = Color.Lerp(paperMaterial.color, targetColor, Time.deltaTime * colorChangeSpeed * 8);
                if (paperMaterial.color == boostColor)
                    targetColor = defaultColor;
            }
            else if (targetColor == suspendColor)
            {
                paperMaterial.color = Color.Lerp(paperMaterial.color, targetColor, Time.deltaTime * colorChangeSpeed * 8);
            }
            else // default color
            {
                paperMaterial.color = Color.Lerp(paperMaterial.color, targetColor, Time.deltaTime * colorChangeSpeed);
            }
        }
    }

    public void ApplyBoostColor()
    {
        targetColor = boostColor;
    }

    public void ApplySuspendColor()
    {
        targetColor = suspendColor;
    }

    public void ApplyDefaultColor()
    {
        targetColor = defaultColor;
    }
}
