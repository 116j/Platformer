using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoarListener : MonoBehaviour
{
    [SerializeField]
    int m_time = 1;
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Damagable>(out var damagable))
        {
            Debug.Log(gameObject.name);
            if (!damagable.Freezed)
            {
                damagable.Freezed = true;
                damagable.Freeze(m_time);
            }
        }
    }
}
