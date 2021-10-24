/*Andrey Chizhov - 101255069
 * Resource drops generates itself randomly upon spawning to either give currency, repair base health, or upgrade a random unit capacity
 * drops will rise to the top of the screen and destruct upon reaching an upper boundary
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDrop : MonoBehaviour
{
    [SerializeField]
    public ResourceDropType m_type;

    [SerializeField]
    private float m_riseSpeed = 0;

    private SpriteRenderer m_sprite;

    float dropValue = 0;
    public void OnSpawn()
    {
        m_type = (ResourceDropType)Random.Range(0f, (int)ResourceDropType.NUM_RESOURCE_DROP_TYPES);
        dropValue = Random.Range(100f, 200f);
        m_sprite = GetComponent<SpriteRenderer>();
        m_sprite.sprite = GameProperties.Instance.dropIcons[(int)m_type];
    }
    public void OnPickedUp()
    {
        switch (m_type)
        {
            case ResourceDropType.BASE_HEALTH:
                GameController.Instance.RepairBase(dropValue * 2f);
                break;
            case ResourceDropType.CURRENCY:
                GameController.Instance.m_playerFunds += (int)dropValue;
                break;
            case ResourceDropType.UNIT_CAP_UPGRADE:
                GameController.Instance.UpgradePlayerUnitLimit(Random.Range(0, (int)UnitType.NUM_UNIT_TYPES));
                break;
        }
        Destroy(this.gameObject);
    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(0.0f, m_riseSpeed * Time.deltaTime, 0.0f);
        if (transform.position.y > 9.0f) Destroy(this.gameObject);
    }
}
