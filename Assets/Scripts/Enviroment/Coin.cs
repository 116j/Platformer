using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Coin : MonoBehaviour
{
    Rigidbody2D m_rb;

    float m_cost;
    // Start is called before the first frame update
    void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
    }

    public void SetCost(float cost, bool destroy = true)
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
            UIController.Instance.AddMoney(m_cost);
            Destroy(gameObject);
        }
    }

}
