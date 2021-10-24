using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDrop : MonoBehaviour
{
    [SerializeField]
    public ResourceDropType m_type;

    private SpriteRenderer m_sprite;

    float dropValue = 0;
    public void OnSpawn()
    {
        m_type = (ResourceDropType)Random.Range(0f, (int)ResourceDropType.NUM_RESOURCE_DROP_TYPES);
        dropValue = Random.Range(20f, 100f);
        m_sprite = GetComponent<SpriteRenderer>();
        m_sprite.sprite = GameProperties.Instance.dropIcons[(int)m_type];
    }
    public void OnPickedUp()
    {
        switch (m_type)
        {
            case ResourceDropType.BASE_HEALTH:
                GameController.Instance.RepairBase(dropValue);
                break;
            case ResourceDropType.CURRENCY:
                GameController.Instance.m_playerFunds += (int)dropValue;
                break;
        }
        Destroy(this.gameObject);
    }
}
