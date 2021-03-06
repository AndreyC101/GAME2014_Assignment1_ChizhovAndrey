using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// MenuControl.cs - Andrey Chizhov - 101255069
/// Manages the menus and provides basic button functions
/// </summary>
public class MenuControl : MonoBehaviour
{
    [SerializeField]
    public GameObject[] m_menuPanels = new GameObject[(int)MenuType.NUM_MENU_TYPES];

    public GameObject m_HUD;
    void Start()
    {
        OnStartNew();
    }

    void OnStartNew()
    {
        GameController.Instance.m_audio.clip = GameProperties.Instance.menuMusic[0];
        GameController.Instance.m_audio.Play();
        m_HUD.GetComponent<Canvas>().enabled = false;
        SwitchToMenu(MenuType.MAIN);
    }
    void FixedUpdate()
    {
        m_HUD.GetComponent<Canvas>().enabled = Time.timeScale == 0 ? false : true;
        if (GameController.Instance.m_gameInProgress)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                if (m_menuPanels[(int)MenuType.CONTROLS].activeSelf)
                    CloseAllMenus();
                else SwitchToMenu(MenuType.PAUSE);
            }
        }
    }

    public void SwitchToMenu(MenuType menu)
    {
        CloseAllMenus();
        m_menuPanels[(int)menu].SetActive(true);
        Time.timeScale = 0;
    }

    public void CloseAllMenus()
    {
        foreach (GameObject menu in m_menuPanels)
        {
            menu.SetActive(false);
        }
        Time.timeScale = 1;
    }

    public void BeginGame()
    {
        GameController.Instance.OnStartGame();
        m_HUD.GetComponent<Canvas>().enabled = true;
        CloseAllMenus();
    }

    public void Quit()
    {
        Application.Quit();
    }


    //FOR EDITOR
    public void OnUpgradeButtonPressed()
    {
        SwitchToMenu(MenuType.UPGRADE);
    }
    public void OnPauseButtonPressed()
    {
        SwitchToMenu(MenuType.PAUSE);
    }
    public void OnControlsButtonPressed()
    {
        SwitchToMenu(MenuType.CONTROLS);
    }

    public void OnMainMenuButtonPressed()
    {
        SwitchToMenu(MenuType.MAIN);
    }

    public void OnControlsReturnButtonPressed()
    {
        if (GameController.Instance.m_gameInProgress)
            OnPauseButtonPressed();
        else SwitchToMenu(MenuType.MAIN);
    }

    public void OnRestartButtonPressed()
    {
        GameController.Instance.m_gameInProgress = false;
        GameController.Instance.CleanGame();
        OnStartNew();
    }
}
