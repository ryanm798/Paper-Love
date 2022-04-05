using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    /***** SINGLETON SETUP *****/
    private static LevelChanger _instance;
    public static LevelChanger Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } 
        else
        {
            _instance = this;
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


    public bool MainMenu = false;
    PlaneController planeController;

    [Header("Scene Transitions")]
    public Animator animator;
    public bool FadeOut = true;
    int levelToLoad = 0;

    [Header("Level Objectives")]
    [Tooltip("Number of suspension checkpoints, excluding final objective point")]
    public int numCheckpoints = 0;
    [HideInInspector] public int checkpointsPassed = 0;


    void Start()
    {
        levelToLoad = (SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings;
        if (MainMenu)
        {
            Cursor.visible = true;
        }
        else
        {
            Cursor.visible = false;

            planeController = FindObjectOfType<PlaneController>();
        }
    }

    void Update()
    {
        if (planeController != null && planeController.GetSpeed() == 0 && planeController.IsGrounded())
        {
            LoadScene(SceneManager.GetActiveScene().buildIndex, 3f);
        }
    }

    public void LoadNextScene()
    {
        LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
    }

    public void LoadLevel(int levelNum)
    {
        LoadScene(levelNum);
    }

    public void LoadScene(int buildIndex, float transitionSpeed = 1f)
    {
        animator.SetFloat("TransitionSpeed", transitionSpeed);

        levelToLoad = buildIndex;

        GameManager.Instance.CurrentLevel = buildIndex;
        if (buildIndex == 0)
            GameManager.Instance.CurrentLevel = 1;
        
        if (FadeOut)
        {
            animator.SetTrigger("FadeOut");
        }
        else
        {
            SceneManager.LoadScene(buildIndex);
        }
    }

    public void OnFadeInStart()
    {
        
    }

    public void OnFadeInEnd()
    {
        
    }

    public void OnFadeOutStart()
    {
        
    }

    public void OnFadeOutEnd()
    {
        SceneManager.LoadScene(levelToLoad);
    }

    public bool PassedCheckpoints()
    {
        return checkpointsPassed >= numCheckpoints;
    }
}
