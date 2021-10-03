using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MenuControl : MonoBehaviour
{
    [SerializeField]
    public GameObject[] m_menuPanels = new GameObject[(int)MenuType.NUM_MENU_TYPES];

    public GameObject m_HUD;

    private GameController m_game;
    void Start()
    {
        m_game = GetComponent<GameController>();
        m_game.m_audio.clip = GameProperties.Instance.menuMusic[0];
        m_game.m_audio.Play();
        m_HUD.GetComponent<Canvas>().enabled = false;
        SwitchToMenu(MenuType.MAIN);
    }
    void Update()
    {
        if (m_game.m_gameInProgress)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (m_menuPanels[(int)MenuType.CONTROLS].activeSelf)
                    CloseAllMenus();
                else SwitchToMenu(MenuType.PAUSE);
            }
        }
        m_HUD.GetComponent<Canvas>().enabled = Time.timeScale == 0.0f ? false : true;
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
        m_game.StartGame();
        CloseAllMenus();
    }

    public void Quit()
    {
        Application.Quit();
    }


    //FOR EDITOR
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
        if (m_game.m_gameInProgress)
            OnPauseButtonPressed();
        else SwitchToMenu(MenuType.MAIN);
    }

    public void OnRestartButtonPressed()
    {
        SceneManager.LoadScene(0);
    }
}
