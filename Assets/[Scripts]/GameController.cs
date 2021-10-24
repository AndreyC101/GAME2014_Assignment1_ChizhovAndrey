using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// GameController.cs - Andrey Chizhov - 101255069
/// Controls game flow outside the menus, spawns enemies and handles victory and defeat conditions, implemented as a singleton
/// enemy units are spawned randomly on a timer. The timer depends on their spawn times found in gameproperties, and decreased based on how many units have already been spawned this game
/// the player earns currency at a rate of the same bpm as the gamestate music
/// </summary>
public class GameController : MonoBehaviour
{
    private static GameController _instance = null;
    public static GameController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType(typeof(GameController)) as GameController;
            }
            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(this);
    }

    public Camera m_camera;
    public int m_playerFunds;
    public Text m_fundsDisplay;
    public float m_fundsAddDelay;
    public float m_enemySpawnDelayMod = 0;

    [SerializeField]
    private HomeBase m_playerBase;

    //spawn transforms
    public Transform m_friendlySpawnPoint;
    public Transform m_enemySpawnPoint;

    public AudioSource m_audio;
    
    private bool enemySpawnBlocked = false;
    private bool fundsAddBlocked = false;

    private Vector2 m_touchStart, m_touchEnd;

    //enemy control
    private int enemyUnitsSpawned = 0;
    public float m_enemySpawnDelay = 0.0f;
    private float timeOfLastEnemySpawn = 0;
    private float timeOfLastFundsAdded = 0;
    private float playerUnitsKilled = 0;

    //gameplay flags
    public bool m_gameInProgress = false;
    private bool m_playerVictory = false;

    private MenuControl m_menuController;
    private UnitManager m_unitManager;

    [SerializeField]
    public int[] activeMaxPlayerUnits = new int[(int)UnitType.NUM_UNIT_TYPES];
    public int[] playerUnitCounts = new int[(int)UnitType.NUM_UNIT_TYPES];
    public List<Unit> unitsInPlay = new List<Unit>();

    private int m_resourceDropsLayerMask = 1 << 8;
    void Start()
    {
        m_camera = FindObjectOfType<Camera>();
        m_audio = GetComponent<AudioSource>();
        m_menuController = GetComponent<MenuControl>();
        m_unitManager = GetComponent<UnitManager>();
        m_fundsDisplay.color = GameProperties.Instance.textColors[(int)TextType.VALID];
    }
    public void OnStartGame()
    {
        GameProperties.Instance.maxPlayerUnits.CopyTo(activeMaxPlayerUnits, 0);
        m_gameInProgress = true;
        m_audio.clip = GameProperties.Instance.menuMusic[1];
        m_audio.Play();
        StartCoroutine("InitiateEnemySpawn");
    }
    void FixedUpdate()
    {
        if (!m_gameInProgress) return;
        GetUnitCounts();
        UpdateHUD();
        TryEarnFunds();
    }
    private void Update()
    {
        UpdateTouchInput();
    }
    public void SpawnUnit(UnitType type, bool friendly)
    {
        unitsInPlay.Add(m_unitManager.GetUnit(friendly, (int)type, friendly ? m_friendlySpawnPoint : m_enemySpawnPoint).GetComponent<Unit>());
    }
    IEnumerator InitiateEnemySpawn()
    {
        UnitType enemyToSpawn;
        int attempts = 0;
        do
        {
            enemyToSpawn = (UnitType)Random.Range(0, (int)UnitType.NUM_UNIT_TYPES);
            if (attempts++ % 10 == 0)
            {
                yield return new WaitForSeconds(0.5f);
            }
        } while (m_unitManager.UnitsAvailable(false, (int)enemyToSpawn) == 0);
        float spawnDelay = GameProperties.Instance.spawnTimes[(int)enemyToSpawn] +
            (m_enemySpawnDelayMod > -GameProperties.Instance.spawnTimes[(int)enemyToSpawn] ? m_enemySpawnDelayMod : GameProperties.Instance.spawnTimes[(int)enemyToSpawn] * 0.07f);
        StartCoroutine(ExecuteEnemySpawn(enemyToSpawn, spawnDelay));
        StopCoroutine("InitiateEnemySpawn");
    }
    IEnumerator ExecuteEnemySpawn(UnitType type, float spawnDelay)
    {
        yield return new WaitForSeconds(spawnDelay);
        timeOfLastEnemySpawn = Time.time;
        SpawnUnit(type, false);
        StartCoroutine("InitiateEnemySpawn");
    }
    private void GetUnitCounts()
    {
        for (int i = 0; i < (int)UnitType.NUM_UNIT_TYPES; i++)
        {
            playerUnitCounts[i] = activeMaxPlayerUnits[i] - m_unitManager.UnitsAvailable(true, i);
        }
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
            Ray ray = m_camera.ScreenPointToRay(m_touchEnd);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, m_resourceDropsLayerMask);
            Debug.Log("Raycasting touch position");
            if (hit)
            {
                Debug.Log("Raycast hit");
                ResourceDrop drop = hit.transform.gameObject.GetComponent<ResourceDrop>();
                if (drop)
                {
                    Debug.Log("Drop Detected");
                    drop.OnPickedUp();
                }
            }

            float swipeDistance = m_touchEnd.x - m_touchStart.x;
            Vector2 cameraAcceleration = new Vector2((-swipeDistance) / 50, 0f);
            m_camera.GetComponent<Rigidbody2D>().AddForce(cameraAcceleration, ForceMode2D.Impulse);
            //Debug.Log($"Swipe of {swipeDistance} units detected");
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
    private void OnGameOver()
    {
        CleanGame();
        if (m_playerVictory)
        {
            m_menuController.SwitchToMenu(MenuType.VICTORY);
            m_audio.clip = GameProperties.Instance.menuMusic[2];
            m_audio.loop = false;
        }
        else
        {
            m_menuController.SwitchToMenu(MenuType.DEFEAT);
            m_audio.clip = GameProperties.Instance.menuMusic[3];
        }
        m_audio.Play();
    }
    public void OnPlayerUnitKilled()
    {
        if (playerUnitsKilled++ % 5 == 0)
        {
            m_unitManager.AddUnitToQueue(false, Random.Range(0, (int)UnitType.NUM_UNIT_TYPES));
        }
    }
    public void CleanGame()
    {
        foreach (Unit unit in unitsInPlay)
        {
            m_unitManager.ReturnUnit(unit.gameObject);
        }
    }
    public void TrySpawnResourceDrop(Vector3 position)
    {
        float rand = Random.Range(0, 100f);
        if (rand < GameProperties.Instance.playerDropRate)
        {
            GameObject drop = Instantiate(GameProperties.Instance.resourceDrop, position, Quaternion.identity);
            drop.GetComponent<ResourceDrop>().OnSpawn();
        }
    }
    public void RepairBase(float dropValue)
    {
        if (m_playerBase)
        {
            m_playerBase.TakeDamage(-dropValue);
        }
    }
    public void UpgradePlayerUnitLimit(int unitType)
    {
        activeMaxPlayerUnits[unitType]++;
        m_unitManager.AddUnitToQueue(true, unitType);
    }
}
