using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDrop : MonoBehaviour
{
    [SerializeField]
    public ResourceDropType m_type;
    public void OnSpawn()
    {
        m_type = (ResourceDropType)Random.Range(0f, (int)ResourceDropType.NUM_RESOURCE_DROP_TYPES);
    }
    public void OnPickedUp()
    {
        switch (m_type)
        {
            case ResourceDropType.BASE_HEALTH:
                break;
            case ResourceDropType.CURRENCY:

                break;
        }
    }
}
