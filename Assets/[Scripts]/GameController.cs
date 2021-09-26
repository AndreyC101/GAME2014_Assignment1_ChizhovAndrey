using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameController : MonoBehaviour
{
    public int m_playerFunds;

    public int m_maxBaseHealth = 150;
    public int m_baseHealth;

    public Image m_healthBar;
    public Image m_healthBarFill;

    public Text m_fundsDisplay;
    void Start()
    {
        m_baseHealth = m_maxBaseHealth;
        m_healthBarFill = m_healthBar.transform.Find("HealthBar").GetComponent<Image>();
        m_fundsDisplay.color = GameProperties.Instance.textColors[(int)TextType.VALID];
    }

    void FixedUpdate()
    {
        UpdateUI();
    }

    public void SpawnUnit(UnitType type)
    {

    }

    private void UpdateUI()
    {
        m_fundsDisplay.text = m_playerFunds.ToString();
        //set health bar fill
        float fillPerc = m_baseHealth/m_maxBaseHealth;
        RectTransform maxSize = m_healthBar.rectTransform;
        m_healthBarFill.rectTransform.sizeDelta = new Vector2(maxSize.rect.width * fillPerc, maxSize.rect.height);
    }
}
