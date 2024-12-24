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
        //Get cat component and position
        if (collision.CompareTag(m_tag)&& collision.TryGetComponent<Cat>(out m_cat))
        {
            if (m_cat.CanPet)
            {
                m_onTrigger.Invoke(true);
                m_cat.Stop(true);
            }
            TargetLocation = m_cat.PetPlayerLocation.position;
        }
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        // update position
        if (collision.CompareTag(m_tag)&&m_cat)
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
            if (m_cat&&m_cat.CanPet)
                m_cat.Stop(false);
            m_cat = null;
        }
    }
    // start and end of petting
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
