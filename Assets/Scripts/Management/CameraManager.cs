using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    /***** SINGLETON SETUP *****/
    private static CameraManager _instance;
    public static CameraManager Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } 
        else
        {
            _instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
    }
    private void OnDestroy()
    {
        if (this == _instance)
        {
            _instance = null;
        }
    }
    /******************************/

    [SerializeField]
    private GameObject followCamera;
    [SerializeField]
    private GameObject freelookCamera;
    private CinemachineFreeLook freeLook;

    public void Initialize(bool startSuspended)
    {
        freeLook = freelookCamera.GetComponent<CinemachineFreeLook>();

        if (startSuspended)
        {
            EnableFreelookCamera();
        }
        else
            EnableFollowCamera();
    }

    public void EnableFollowCamera()
    {
        followCamera.SetActive(true);
        freelookCamera.SetActive(false);
    }

    public void EnableFreelookCamera()
    {
        followCamera.SetActive(false);
        freelookCamera.SetActive(true);
    }

    public Quaternion GetCameraRotation()
    {
        return Camera.main.transform.rotation;
    }
}
