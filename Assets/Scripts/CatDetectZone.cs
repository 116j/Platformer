using UnityEngine;
using UnityEngine.Events;

public class CatDetectZone : DetectZone
{
    [SerializeField]
    UnityEvent<bool> m_onTrigger;

    Cat m_cat;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.CompareTag(m_tag))
        {
            m_cat = collision.GetComponent<Cat>();
            if(m_cat.CanPet)
                m_onTrigger.Invoke(true);
            TargetLocation = m_cat.PetPlayerLocation.position;
        }
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(m_tag))
        {
            TargetLocation = m_cat.PetPlayerLocation.position;
        }
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        if (collision.CompareTag(m_tag))
        {
            m_onTrigger.Invoke(false);
            m_cat = null;
        }
    }

    public void ApplyPet(bool pet)
    {
        if(m_cat != null)
        {
            if (Vector2.Dot(m_cat.transform.right, transform.right) == 1)
            {
                m_cat.TurnAround();
            }
            m_cat.Pet(pet);
            if (!pet)
            {
                m_onTrigger.Invoke(false);
            }
        }
    }
}
