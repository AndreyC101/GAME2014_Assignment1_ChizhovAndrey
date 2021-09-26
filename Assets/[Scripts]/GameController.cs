using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameController : MonoBehaviour
{
    public int m_playerFunds;

    public Text m_fundsDisplay;

    public Transform m_friendlySpawnPoint;
    public Transform m_enemySpawnPoint;

    public float m_enemySpawnDelay = 0.0f;
    private float timeOfLastEnemySpawn = 0;
    private bool enemySpawnBlocked = false;

    private List<Unit> unitsInPlay = new List<Unit>();
    void Start()
    {
        m_fundsDisplay.color = GameProperties.Instance.textColors[(int)TextType.VALID];
    }

    void FixedUpdate()
    {
        UpdateUI();
        TrySpawnEnemy();
        RemoveDeadUnits();
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
        unitsInPlay.Add(newUnit);
    }

    private void TrySpawnEnemy()
    {
        if (Time.time < timeOfLastEnemySpawn + m_enemySpawnDelay || enemySpawnBlocked)
        {
            return;
        }
        timeOfLastEnemySpawn = Time.time;
        m_enemySpawnDelay = Random.Range(4.0f, 12.0f);
        SpawnUnit((UnitType)Random.Range(0, (int)UnitType.NUM_UNIT_TYPES), false);
    }

    private void UpdateUI()
    {
        m_fundsDisplay.text = m_playerFunds.ToString();
    }

    public void OnBaseDestroyed()
    {
        Debug.Log("Base Destroyed");
        foreach (Unit unit in unitsInPlay)
        {
            unit.TakeDamage(500);
        }
        enemySpawnBlocked = true;
        //Do some UI shit
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
}
