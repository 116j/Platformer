using UnityEngine;

public class Coin : MonoBehaviour
{
    Rigidbody2D m_rb;
    Collider2D m_col;
    AudioSource m_coinGain;

    int m_cost;
    // Start is called before the first frame update
    void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_coinGain = GetComponent<AudioSource>();
        m_col = GetComponent<Collider2D>();
    }

    public void SetCost(int cost, bool destroy = true)
    {
        m_cost = cost;
        if (destroy)
        {
            m_rb.AddForce(new Vector2(Random.Range(-2f, 2f), 7f), ForceMode2D.Impulse);
            Destroy(gameObject, 10f);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            m_coinGain.Play();
            m_col.isTrigger = true;
            UIController.Instance.AddMoney(m_cost);
            Destroy(gameObject,0.2f);
        }
    }

}
