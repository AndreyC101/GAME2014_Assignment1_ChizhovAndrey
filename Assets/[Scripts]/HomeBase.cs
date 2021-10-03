using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeBase : MonoBehaviour, IDamageable
{
    public float m_maxBaseHealth = 150;
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
        m_baseHealth = m_maxBaseHealth;
        m_healthBarFill = m_healthBar.transform.Find("HealthBar");

        m_hqSign.sortingLayerName = "Friendlies";
        m_hqSign.sortingOrder = 3;
    }

    void UpdateUI()
    {
        float fillPerc = m_baseHealth / m_maxBaseHealth;
        m_healthBarFill.localScale = new Vector3(fillPerc, m_healthBarFill.localScale.y, 1.0f);
    }
    public bool TakeDamage(int incomingDamage)
    {
        m_baseHealth -= incomingDamage;
        if (m_baseHealth > 0)
        {
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
        //disable all sprites and mesh renderer
    }


}
