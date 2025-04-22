using System;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ShopLayout : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI m_dialogueText;

    static ShopLayout m_instance;
    public static ShopLayout Instance => m_instance;

    GameObject m_player;
    AudioSource m_buySound;

    bool[] m_clicked = new bool[5];
    int[] m_itemsCount = { 3, 2, 2, 2, 1 };
    float[] m_prices = { 1000, 1500, 2000, 2000, 2500 };
    string m_startText = "Hello, Stranger! Welcome to my shop! What would you like to purchase?";
    string[] m_dialogueTexts =
    {
        "Add extra health heart.",
        "Decrease dash cooldown time.",
        "Increase light attack damage.",
        "Increase heavy attack damage.",
        "Add extra jump. You will be able to do triple jump."
    };

    public float GetLowestPrice()
    {
        if (m_prices.Length == 0)
        {
            m_prices = new float[] { 1000, 1500, 2000, 2000, 2500 };
        }

        for (int i = 0; i < m_prices.Length; i++)
        {
            if (m_itemsCount[i] > 0)
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
        m_buySound = GetComponent<AudioSource>();
    }

    void Buy(int index, TextMeshProUGUI price, Action func)
    {
        if (m_itemsCount[index] <= 0)
            return;
        if (!m_clicked[index])
        {
            m_dialogueText.text = m_dialogueTexts[index];
            m_dialogueText.text += UIController.Instance.GetMoney() >= m_prices[index] ? " Click again to buy." : " But you don't have enough money, beggar.";
            for (int i = 0; i < m_clicked.Length; i++)
            {
                m_clicked[i] = false;
            }
            m_clicked[index] = true;
        }
        else if (UIController.Instance.GetMoney() >= m_prices[index])
        {
            m_buySound.Play();
            UIController.Instance.AddMoney(-m_prices[index]);
            m_dialogueText.text = m_startText;
            func();
            m_clicked[index] = false;
            m_itemsCount[index]--;
            if (m_itemsCount[index] <= 0)
            {
                price.text = "SOLD";
            }
        }
    }

    public void AddHealth(TextMeshProUGUI price)
    {
        Action func = UIController.Instance.AddHeart;
        func += m_player.GetComponent<Damagable>().IncreaseHealth;
        Buy(0, price, func);
    }


    public void AddDash(TextMeshProUGUI price)
    {
        Buy(1, price, m_player.GetComponent<PlayerController>().DecreaseDashCooldown);
    }

    public void AddLightDamage(TextMeshProUGUI price)
    {
        Buy(2, price, m_player.transform.GetChild(0).GetComponent<AttackListener>().IncreaseDamage);
    }

    public void AddHeavyDamage(TextMeshProUGUI price)
    {
        Buy(3, price, m_player.transform.GetChild(1).GetComponent<AttackListener>().IncreaseDamage);
    }

    public void AddJump(TextMeshProUGUI price)
    {
        Action func = m_player.GetComponent<PlayerController>().AddJump;
        func += LevelBuilder.Instance.SetTripleJump;
        Buy(4, price, func);
    }
}
