using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    /***** SINGLETON SETUP *****/
    private static AudioManager _instance;
    public static AudioManager Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } 
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
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
    private AudioSource windSfx;

    public void SetWindVolume(float newVolume)
    {
        windSfx.volume = Mathf.Lerp(windSfx.volume, newVolume, Time.deltaTime * 20f);
    }

    public float GetWindVolume()
    {
        return windSfx.volume;
    }
}
