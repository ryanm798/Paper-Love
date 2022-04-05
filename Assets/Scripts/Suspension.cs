using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suspension : MonoBehaviour
{
    public float boost = -1f;

    private bool triggered = false;
    private float prevTriggerTime = -1f;
    private static float cooldown = 1.0f;

    [Header("Level Objective")]
    [Tooltip("Is this the final point to reach in the level?")]
    [SerializeField] bool isObjective = false;
    [Tooltip("Is this point required before reaching the objective?")]
    [SerializeField] bool isCheckpoint = false;
    bool passed = false;

    void Awake()
    {
        if (isObjective) isCheckpoint = false;
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !triggered && ((prevTriggerTime == -1f) || ((Time.time - prevTriggerTime) > cooldown)))
        {
            triggered = true;
            other.gameObject.GetComponent<PlaneController>().Suspend(boost);

            if (isCheckpoint && !passed)
            {
                passed = true;
                LevelChanger.Instance.checkpointsPassed++;
            }

            if (isObjective && LevelChanger.Instance.PassedCheckpoints())
            {
                LevelChanger.Instance.LoadNextScene();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            triggered = false;
            prevTriggerTime = Time.time;
        }
    }
}
