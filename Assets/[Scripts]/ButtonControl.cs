using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonControl : MonoBehaviour
{
    [SerializeField]
    UnitType m_unitSpawnType;

    Image m_unitIcon;

    private float m_spawnTime;

    private int m_unitCost;

    private Transform m_costIcon;
    private Text m_costText;

    private Image m_loadingBar;
    private Image m_loadingBarFill;

    GameController m_game;

    private bool spawnAvailable = false;
    private float timeOfBeginSpawn = 0f;
    void Start()
    {
        //set internal values
        m_game = FindObjectOfType<GameController>();
        m_unitIcon = transform.Find("UnitIcon").GetComponent<Image>();
        m_unitIcon.sprite = GameProperties.Instance.unitIcons[(int)m_unitSpawnType];
        m_spawnTime = GameProperties.Instance.spawnTimes[(int)m_unitSpawnType];
        m_unitCost = GameProperties.Instance.unitCosts[(int)m_unitSpawnType];

        //setup extra UI elements
        m_costIcon = transform.Find("MoneyIcon");
        m_costText = m_costIcon.Find("CostText").GetComponent<Text>();
        m_costText.text = m_unitCost.ToString();
        m_loadingBar = transform.Find("LoadBarFrame").GetComponent<Image>();
        m_loadingBarFill = m_loadingBar.transform.Find("LoadBarFill").GetComponent<Image>();
        m_costIcon.gameObject.SetActive(true);
        m_loadingBar.gameObject.SetActive(false);

        spawnAvailable = (m_game.m_playerFunds >= m_unitCost);
    }

    private void Update()
    {
        UpdateDisplay();
    }

    public void OnPressed()
    {
        if (spawnAvailable && m_game.m_playerFunds >= m_unitCost)
        {
            spawnAvailable = false;
            timeOfBeginSpawn = Time.time;
            m_game.m_playerFunds -= m_unitCost;
            m_costIcon.gameObject.SetActive(false);
            m_loadingBar.gameObject.SetActive(true);
        }
    }

    private void SpawnUnit()
    {
        Debug.Log("Spawning Unit: " + m_unitSpawnType.ToString());
        m_costIcon.gameObject.SetActive(true);
        m_loadingBar.gameObject.SetActive(false);
        m_game.SpawnUnit(m_unitSpawnType, true);
        spawnAvailable = true;
    }

    /// <summary>
    /// If not loading unit, Set cost color to red if insufficient funds, else set yellow
    /// if loading unit, update loading bar fill using deltaTime and check timer
    /// </summary>
    private void UpdateDisplay()
    {
        if (m_loadingBar.gameObject.activeSelf)
        {
            if (Time.time > timeOfBeginSpawn + m_spawnTime)
                SpawnUnit();
            else
            {
                float fillPerc = (Time.time - timeOfBeginSpawn) / m_spawnTime;
                RectTransform maxSize = m_loadingBar.rectTransform;
                m_loadingBarFill.rectTransform.sizeDelta = new Vector2(maxSize.rect.width * fillPerc, maxSize.rect.height);
            }
        }
        if (m_game.m_playerFunds < GameProperties.Instance.unitCosts[(int)m_unitSpawnType])
            m_costText.color = GameProperties.Instance.textColors[(int)TextType.INVALID];
        else m_costText.color = GameProperties.Instance.textColors[(int)TextType.VALID];
    }
}
