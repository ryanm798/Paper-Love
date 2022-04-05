using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    /***** SINGLETON SETUP *****/
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }
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

    [HideInInspector] public int CurrentLevel = 1;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;

        ScaleTime(1);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ScaleTime(1);
    }
    
    public void Quit()
    {
        Application.Quit();
    }

    public void Play()
    {
        LevelChanger.Instance.LoadScene(CurrentLevel);
    }

    public void ScaleTime(float tscale)
    {
        Time.timeScale = tscale;
        Time.fixedDeltaTime = tscale * 0.02f;
    }
}
