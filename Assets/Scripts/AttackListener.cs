using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AttackListener : MonoBehaviour
{
    [SerializeField]
    int m_damage = 1;
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Damagable>(out var damagable))
        {
            //Debug.Log(gameObject.name);
            if (!damagable.Invinsible || damagable.Invinsible && (Vector2.Dot(collision.transform.right, transform.right) == 1))
            {
                damagable.ApplyDamage(m_damage);
            }
        }
    }

    public void IncreaseDamage()
    {
        m_damage++;
    }
}
