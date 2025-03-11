using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopLayout : MonoBehaviour
{
    static ShopLayout m_instance;
    public static ShopLayout Instance => m_instance;

    GameObject m_player;

    bool[] m_sold = new bool[5];
    float[] m_prices= { 1000, 1500, 2000, 2000, 2500 };

    int m_maxHealth = 3;
    int m_dash = 2;
    int m_lDamage = 2;
    int m_hDamage = 2;
    bool m_jump = false;

    public float GetLowestPrice()
    {
        if (m_prices.Length == 0)
        {
            m_prices = new float[] { 1000, 1500, 2000, 2000, 2500 };
        }

        for (int i = 0; i < m_prices.Length; i++)
        {
            if (!m_sold[i])
                return m_prices[i];
        }

        return float.MaxValue;
    }

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }

        m_player = GameObject.FindGameObjectWithTag("Player");

    }

    public void AddHealth(TextMeshProUGUI price)
    {
        if (m_maxHealth <= 0)
            return;
        UIController.Instance.AddHeart();
        UIController.Instance.AddMoney(-m_prices[0]);
        m_player.GetComponent<Damagable>().IncreaseHealth();
        m_maxHealth--;
        if (m_maxHealth <= 0)
        {
            m_sold[0] = true;
            price.text = "SOLD";
        }
    }

    public void AddJump(TextMeshProUGUI price)
    {
        if (m_jump)
            return;

        m_player.GetComponent<PlayerController>().AddJump();
        UIController.Instance.AddMoney(-m_prices[1]);
        m_sold[1] = true;
        price.text = "SOLD";
        m_jump = true;
    }

    public void AddLightDamage(TextMeshProUGUI price)
    {
        if (m_lDamage <= 0)
            return;

        m_player.transform.GetChild(0).GetComponent<AttackListener>().IncreaseDamage();
        UIController.Instance.AddMoney(-m_prices[2]);
        m_lDamage--;
        if (m_lDamage <= 0)
        {
            m_sold[2] = true;
            price.text = "SOLD";
        }
    }

    public void AddHeavyDamage(TextMeshProUGUI price)
    {
        if (m_hDamage <= 0)
            return;

        m_player.transform.GetChild(1).GetComponent<AttackListener>().IncreaseDamage();
        UIController.Instance.AddMoney(-m_prices[3]);
        m_hDamage--;
        if (m_hDamage <= 0)
        {
            m_sold[3] = true;
            price.text = "SOLD";
        }
    }

    public void AddDash(TextMeshProUGUI price)
    {
        if (m_dash <= 0)
            return;

        m_player.GetComponent<PlayerController>().DecreaseDashCooldown();
        UIController.Instance.AddMoney(-m_prices[4]);
        m_dash--;
        if (m_dash <= 0)
        {
            m_sold[4] = true;
            price.text = "SOLD";
        }
    }
}
