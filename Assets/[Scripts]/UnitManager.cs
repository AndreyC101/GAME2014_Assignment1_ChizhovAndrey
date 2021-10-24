using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [SerializeField]
    private Queue<GameObject>[] m_playerUnits = new Queue<GameObject>[(int)UnitType.NUM_UNIT_TYPES];
    [SerializeField]
    private Queue<GameObject>[] m_enemyUnits = new Queue<GameObject>[(int)UnitType.NUM_UNIT_TYPES];
    void Awake()
    {
        BuildUnitPools();
    }

    private void BuildUnitPools()
    {
        for (int i = 0; i < (int)UnitType.NUM_UNIT_TYPES; i++)
        {
            Debug.Log($"Loading pool of {((UnitType)i).ToString()}");
            m_playerUnits[i] = new Queue<GameObject>();
            Debug.Log($"Player {((UnitType)i).ToString()} pool created");
            m_enemyUnits[i] = new Queue<GameObject>();
            Debug.Log($"Enemy {((UnitType)i).ToString()} pool created");

            for (int j = 0; j < GameProperties.Instance.maxPlayerUnits[i]; j++)
            {
                AddUnitToQueue(true, i);
            }

            for (int j = 0; j < GameProperties.Instance.maxEnemyUnits[i]; j++)
            {
                AddUnitToQueue(false, i);
            }
        }
    }

    public int UnitsAvailable(bool friendly, int type)
    {
        return friendly ? m_playerUnits[type].Count : m_enemyUnits[type].Count;
    }

    public GameObject GetUnit(bool friendly, int type, Transform spawn)
    {
        var newUnit = friendly ? m_playerUnits[type].Dequeue() : m_enemyUnits[type].Dequeue();
        newUnit.SetActive(true);
        newUnit.transform.position = spawn.position;
        newUnit.GetComponent<Unit>().OnSpawn();
        return newUnit;
    }

    public void ReturnUnit(GameObject unit)
    {
        unit.SetActive(false);
        if (unit.GetComponent<Unit>().m_friendly) m_playerUnits[(int)unit.GetComponent<Unit>().m_type].Enqueue(unit);
        else m_enemyUnits[(int)unit.GetComponent<Unit>().m_type].Enqueue(unit);
    }

    public void AddUnitToQueue(bool friendly, int type)
    {
        var newUnit = Instantiate(GameProperties.Instance.unitPrefabs[type]);
        newUnit.GetComponent<Unit>().m_friendly = friendly;
        newUnit.GetComponent<SpriteRenderer>().flipX = !friendly;
        newUnit.gameObject.layer = friendly ? 6 : 7;
        newUnit.GetComponent<Unit>().m_type = (UnitType)type;
        if (friendly)
            m_playerUnits[type].Enqueue(newUnit);
        else m_enemyUnits[type].Enqueue(newUnit);
    }
}
