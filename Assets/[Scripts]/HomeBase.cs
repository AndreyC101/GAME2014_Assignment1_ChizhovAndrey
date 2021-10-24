using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HomeBase.cs - Andrey Chizhov - 101255069
/// Each base implements the idamageable interface so npcs can destroy them
/// </summary>
public class HomeBase : MonoBehaviour, IDamageable
{
    public float m_maxBaseHealth;
    private float m_baseHealth;

    public Transform m_healthBar;
    private Transform m_healthBarFill;

    private GameController m_game;
    public bool m_friendly;

    [SerializeField]
    private MeshRenderer m_hqSign;

    void Start()
    {
        m_game = FindObjectOfType<GameController>();
        m_healthBarFill = m_healthBar.transform.Find("HealthBar");

        m_hqSign.sortingLayerName = "Friendlies";
        m_hqSign.sortingOrder = 3;
    }

    public void OnGameStart()
    {
        m_baseHealth = m_maxBaseHealth;
        UpdateUI();
    }
    void UpdateUI()
    {
        float fillPerc = m_baseHealth / m_maxBaseHealth;
        m_healthBarFill.localScale = new Vector3(fillPerc, m_healthBarFill.localScale.y, 1.0f);
    }
    public bool TakeDamage(float incomingDamage)
    {
        m_baseHealth -= incomingDamage;
        if (m_baseHealth > 0)
        {
            if (m_baseHealth > m_maxBaseHealth)
            {
                m_baseHealth = m_maxBaseHealth;
            }
            UpdateUI();
        }
        else HandleDestruction();
        return false;
    }

    public void HandleDestruction()
    {
        m_healthBarFill.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        m_game.OnBaseDestroyed(m_friendly);
    }

    public void ClearFromField()
    {
    }


}
