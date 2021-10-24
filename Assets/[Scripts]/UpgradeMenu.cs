/*Andrey Chizhov - 101255069
 * This menu allows the player to buy upgrades for unit capacities
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject[] buttons = new GameObject[(int)UnitType.NUM_UNIT_TYPES];

    [SerializeField]
    private Text[] costDisplays = new Text[(int)UnitType.NUM_UNIT_TYPES];

    [SerializeField]
    private Text fundsDisplay;

    private void Awake() //each button should upgrade unit based on index
    {
        int unitIndex = 0;
        foreach (GameObject button in buttons)
        {
            int upgradeIndex = unitIndex;
            button.GetComponent<Button>().onClick.AddListener(delegate { UpgradeUnitCapacity(upgradeIndex); });
            unitIndex++;
        }
        fundsDisplay.color = GameProperties.Instance.textColors[(int)TextType.VALID];
        for (int i = 0; i < (int)UnitType.NUM_UNIT_TYPES; i++)
        {
            costDisplays[i].text = GameProperties.Instance.capacityUpgradeCosts[i].ToString();
        }
    }
    void Update() //display cost in red if player funds insufficient 
    {
        if (GetComponent<Canvas>().enabled)
        {
            for (int i = 0; i < (int)UnitType.NUM_UNIT_TYPES; i++)
            {
                if (GameController.Instance.m_playerFunds >= GameProperties.Instance.capacityUpgradeCosts[i]) 
                    costDisplays[i].color = GameProperties.Instance.textColors[(int)TextType.VALID];
                else costDisplays[i].color = GameProperties.Instance.textColors[(int)TextType.INVALID];
            }
            fundsDisplay.text = GameController.Instance.m_playerFunds.ToString();
        }
    }

    //upgrade action, deducts currency and upgrades cap
    public void UpgradeUnitCapacity(int type)
    {
        if (GameController.Instance.m_playerFunds >= GameProperties.Instance.capacityUpgradeCosts[type])
        {
            GameController.Instance.m_playerFunds -= GameProperties.Instance.capacityUpgradeCosts[type];
            GameController.Instance.UpgradePlayerUnitLimit(type);
        }
        else Debug.Log("Insufficient Funds");
    }
}
