using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameController : MonoBehaviour
{
    public Camera m_camera;
    public int m_playerFunds;
    public Text m_fundsDisplay;
    public float m_fundsAddDelay;

    public Transform m_friendlySpawnPoint;
    public Transform m_enemySpawnPoint;

    public float m_enemySpawnDelay = 0.0f;
    private float timeOfLastEnemySpawn = 0;
    private float timeOfLastFundsAdded = 0;
    private bool enemySpawnBlocked = false;
    private bool fundsAddBlocked = false;
    

    private List<Unit> unitsInPlay = new List<Unit>();

    private Vector2 m_touchStart, m_touchEnd;
    void Start()
    {
        m_camera = FindObjectOfType<Camera>();
        m_fundsDisplay.color = GameProperties.Instance.textColors[(int)TextType.VALID];
    }

    void FixedUpdate()
    {
        UpdateUI();
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
        m_enemySpawnDelay = Random.Range(5.0f, 15.0f);
        SpawnUnit((UnitType)Random.Range(0, (int)UnitType.NUM_UNIT_TYPES), false);
    }

    private void UpdateUI()
    {
        m_fundsDisplay.text = m_playerFunds.ToString();
    }

    public void OnBaseDestroyed(bool friendly)
    {
        foreach (Unit unit in unitsInPlay)
        {
            unit.TakeDamage(500);
        }
        enemySpawnBlocked = true;
        fundsAddBlocked = true;
        if (friendly)
        {
            Debug.Log("Base Destroyed");
            //Do some UI shit
        }
        else
        {
            Debug.Log("Enemy Base Destroyed");
        }
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
            Vector2 cameraAcceleration = new Vector2((-swipeDistance) / 100, 0f);
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
}
