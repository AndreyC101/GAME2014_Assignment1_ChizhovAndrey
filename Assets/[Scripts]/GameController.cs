using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour
{
    [SerializeField]
    private AudioClip m_gameMusic;

    public Camera m_camera;
    public int m_playerFunds;
    public Text m_fundsDisplay;
    public float m_fundsAddDelay;
    public float m_enemySpawnDelayMod = 0;

    public Transform m_friendlySpawnPoint;
    public Transform m_enemySpawnPoint;

    public AudioSource m_audio;

    public float m_enemySpawnDelay = 0.0f;
    private float timeOfLastEnemySpawn = 0;
    private float timeOfLastFundsAdded = 0;
    private bool enemySpawnBlocked = false;
    private bool fundsAddBlocked = false;

    private List<Unit> unitsInPlay = new List<Unit>();

    private Vector2 m_touchStart, m_touchEnd;

    private UnitType enemyToSpawn;

    private int enemyUnitsSpawned = 0;

    public bool m_gameInProgress = false;

    private bool m_playerVictory = false;

    private MenuControl m_menuController;
    void Start()
    {
        m_camera = FindObjectOfType<Camera>();
        m_audio = GetComponent<AudioSource>();
        m_menuController = GetComponent<MenuControl>();
        m_fundsDisplay.color = GameProperties.Instance.textColors[(int)TextType.VALID];
    }

    public void StartGame()
    {
        m_gameInProgress = true;
        m_audio.clip = m_gameMusic;
        m_audio.Play();
        PickUnitToSpawn();
    }

    void FixedUpdate()
    {
        if (!m_gameInProgress) return;
        UpdateHUD();
        TrySpawnEnemy();
        TryEarnFunds();
        RemoveDeadUnits();
    }
    private void Update()
    {
        UpdateTouchInput();
    }

    public void SpawnUnit(UnitType type, bool friendly)
    {
        Unit newUnit = Instantiate(GameProperties.Instance.unitPrefabs[(int)type], (friendly ? m_friendlySpawnPoint : m_enemySpawnPoint)).GetComponent<Unit>();
        newUnit.m_friendly = friendly;
        if (!friendly)
        {
            newUnit.GetComponent<SpriteRenderer>().flipX = true;
            enemyUnitsSpawned++;
            m_enemySpawnDelayMod = -(0.034f * enemyUnitsSpawned);
        }
        newUnit.gameObject.layer = (friendly ? 6 : 7);
        newUnit.m_type = type;
        unitsInPlay.Add(newUnit);
    }

    private void TrySpawnEnemy()
    {
        if (Time.time < timeOfLastEnemySpawn + m_enemySpawnDelay || enemySpawnBlocked)
        {
            return;
        }
        timeOfLastEnemySpawn = Time.time;
        SpawnUnit(enemyToSpawn, false);
        PickUnitToSpawn();
    }

    private void UpdateHUD()
    {
        m_fundsDisplay.text = m_playerFunds.ToString();
    }

    public void OnBaseDestroyed(bool friendly)
    {
        foreach (Unit unit in unitsInPlay)
        {
            unit.dead = true;
        }
        enemySpawnBlocked = true;
        fundsAddBlocked = true;
        m_gameInProgress = false;
        m_playerVictory = !friendly;
        if (friendly)
        {
            Debug.Log("Base Destroyed");
        }
        else
        {
            Debug.Log("Enemy Base Destroyed");
        }
        Invoke("OnGameOver", 2.5f);
    }

    public void RemoveDeadUnits()
    {
        for (int i = 0; i < unitsInPlay.Count; i++)
        {
            if (unitsInPlay[i].dead)
            {
                unitsInPlay.Remove(unitsInPlay[i]);
            }
        }
    }

    /// <summary>
    /// Lets the user scroll the camera across the field by swiping
    /// </summary>
    private void UpdateTouchInput()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            m_touchStart = Input.GetTouch(0).position;
        }
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            m_touchEnd = Input.GetTouch(0).position;

            float swipeDistance = m_touchEnd.x - m_touchStart.x;
            Vector2 cameraAcceleration = new Vector2((-swipeDistance) / 50, 0f);
            m_camera.GetComponent<Rigidbody2D>().AddForce(cameraAcceleration, ForceMode2D.Impulse);
            Debug.Log($"Swipe of {swipeDistance} units detected");
        }
    }

    private void TryEarnFunds()
    {
        if (Time.time < timeOfLastFundsAdded + m_fundsAddDelay || fundsAddBlocked)
        {
            return;
        }
        timeOfLastFundsAdded = Time.time;
        m_playerFunds += 1;
    }

    private void PickUnitToSpawn()
    {
        enemyToSpawn = (UnitType)Random.Range(0, (int)UnitType.NUM_UNIT_TYPES);
        m_enemySpawnDelay = GameProperties.Instance.spawnTimes[(int)enemyToSpawn] + (m_enemySpawnDelayMod > -GameProperties.Instance.spawnTimes[(int)enemyToSpawn] ? m_enemySpawnDelayMod : GameProperties.Instance.spawnTimes[(int)enemyToSpawn] * 0.07f);
    }

    private void OnGameOver()
    {
        if (m_playerVictory)
        {
            m_menuController.SwitchToMenu(MenuType.VICTORY);
        }
        else
        {
            m_menuController.SwitchToMenu(MenuType.DEFEAT);
        }
    }
}
