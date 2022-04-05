using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossFadeManager : MonoBehaviour
{
    /***** SINGLETON SETUP *****/
    private static CrossFadeManager _instance;
    public static CrossFadeManager Instance { get { return _instance; } }
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

    public List<RawImage> images = new List<RawImage>();
    List<Fade> fades = new List<Fade>();
    public float timePerCam = 10f;
    int prevIndex;
    int index;
    int count;

    void Start()
    {
        foreach (RawImage image in images)
        {
            image.enabled = false;
            fades.Add(image.GetComponent<Fade>());
        }
        images[0].enabled = true;
        images[0].color = new Color(1f, 1f, 1f, 1f);
        images[0].transform.SetAsLastSibling();
        index = 0;
        count = images.Count;
        prevIndex = count - 1;

        InvokeRepeating("FadeToNext", timePerCam, timePerCam);
    }

    void FadeToNext()
    {
        prevIndex = index;
        index = (index + 1) % count;
        images[index].transform.SetAsLastSibling();
        fades[index].FadeIn();
    }

    public void DisablePrevious()
    {
        images[prevIndex].enabled = false;
    }
}
