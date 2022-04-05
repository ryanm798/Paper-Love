using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public GameObject menu;
    public MenuNavigator navigator;

    void Start()
    {
        menu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        bool enabling = !menu.activeSelf;
        menu.SetActive(enabling);
        if (enabling)
        {
            GameManager.Instance.ScaleTime(0f);
            Cursor.visible = true;
            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
            if (navigator != null)
                navigator.ReturnToMain();
        }
        else
        {
            GameManager.Instance.ScaleTime(1f);
            Cursor.visible = false;
        }
    }

    public void ReturnToMain()
    {
        LevelChanger.Instance.LoadScene(0);
    }
}
