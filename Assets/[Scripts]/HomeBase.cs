using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeBase : MonoBehaviour, IDamageable
{
    public float m_maxBaseHealth = 150;
    private float m_baseHealth;

    public Image m_healthBar;
    private Image m_healthBarFill;

    private GameController m_game;

    [SerializeField]
    private MeshRenderer m_hqSign;

    void Start()
    {
        m_game = FindObjectOfType<GameController>();
        m_baseHealth = m_maxBaseHealth;
        m_healthBarFill = m_healthBar.transform.Find("HealthBar").GetComponent<Image>();

        m_hqSign.sortingLayerName = "Friendlies";
        m_hqSign.sortingOrder = 3;
    }

    void UpdateUI()
    {
        float fillPerc = m_baseHealth / m_maxBaseHealth;
        RectTransform maxSize = m_healthBar.rectTransform;
        m_healthBarFill.rectTransform.sizeDelta = new Vector2(maxSize.rect.width * fillPerc, maxSize.rect.height);
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
        m_healthBarFill.enabled = false;
        m_game.OnBaseDestroyed();
    }

    public void ClearFromField()
    {
        //disable all sprites and mesh renderer
    }


}
