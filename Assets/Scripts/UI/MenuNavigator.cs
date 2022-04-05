using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNavigator : MonoBehaviour
{
    public GameObject MainMenu;

    void Start()
    {
        ReturnToMain();
    }

    public void Play()
    {
        GameManager.Instance.Play();
    }

    public void Quit()
    {
        GameManager.Instance.Quit();
    }

    public void ReturnToMain()
    {
        NavigateTo(MainMenu);
    }

    public void NavigateTo(GameObject menu)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        menu.SetActive(true);
    }
}
