using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    public float boost = -1f;
    [SerializeField]
    private AudioSource boostSfx;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlaneController>().Boost(boost);
            if (boostSfx != null)
                boostSfx.Play();
        }
    }
}
