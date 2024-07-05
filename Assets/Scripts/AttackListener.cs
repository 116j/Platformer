using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AttackListener : MonoBehaviour
{
    [SerializeField]
    float m_damage = 1;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Damagable>(out var damagable))
        {
            Debug.Log(gameObject.name+" collided with "+collision.name);
            damagable.ApplyDamage(m_damage);
        }
    }
}
