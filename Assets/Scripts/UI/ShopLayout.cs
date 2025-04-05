using TMPro;
using UnityEngine;

public class ShopLayout : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI m_dialogueText;

    static ShopLayout m_instance;
    public static ShopLayout Instance => m_instance;

    GameObject m_player;

    bool[] m_sold = new bool[5];
    bool[] m_clicked = new bool[5];
    float[] m_prices = { 1000, 1500, 2000, 2000, 2500 };
    string[] m_dialogueTexts =
    {
        "Add extra health heart.",
        "Add extra jump. You will be able to do triple jump.",
        "Increase light attack damage.",
        "Increase heavy attack damage.",
        "Decrease dash cooldown time."
    };

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
        if (!m_clicked[0])
        {
            m_dialogueText.text = m_dialogueTexts[0];
            m_dialogueText.text += UIController.Instance.GetMoney() >= m_prices[0] ? " Click again to buy." : " But you don't have enough money, beggar.";
            m_clicked[0] = true;
            for (int i = 1; i < m_clicked.Length; i++)
            {
                m_clicked[i] = false;
            }
        }
        else if (UIController.Instance.GetMoney() >= m_prices[0])
        {
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
    }

    public void AddJump(TextMeshProUGUI price)
    {
        if (m_jump)
            return;
        if (!m_clicked[1])
        {
            m_dialogueText.text = m_dialogueTexts[1];
            m_dialogueText.text += UIController.Instance.GetMoney() >= m_prices[1] ? " Click again to buy." : " But you don't have enough money, beggar.";
            for (int i = 0; i < m_clicked.Length; i++)
            {
                m_clicked[i] = false;
            }
            m_clicked[1] = true;
        }
        else if (UIController.Instance.GetMoney() >= m_prices[1])
        {
            m_player.GetComponent<PlayerController>().AddJump();
            UIController.Instance.AddMoney(-m_prices[1]);
            m_sold[1] = true;
            price.text = "SOLD";
            m_jump = true;
        }
    }

    public void AddLightDamage(TextMeshProUGUI price)
    {
        if (m_lDamage <= 0)
            return;
        if (!m_clicked[2])
        {
            m_dialogueText.text = m_dialogueTexts[2];
            m_dialogueText.text += UIController.Instance.GetMoney() >= m_prices[2] ? " Click again to buy." : " But you don't have enough money, beggar.";
            for (int i = 0; i < m_clicked.Length; i++)
            {
                m_clicked[i] = false;
            }
            m_clicked[2] = true;
        }
        else if (UIController.Instance.GetMoney() >= m_prices[2])
        {
            m_player.transform.GetChild(0).GetComponent<AttackListener>().IncreaseDamage();
            UIController.Instance.AddMoney(-m_prices[2]);
            m_lDamage--;
            if (m_lDamage <= 0)
            {
                m_sold[2] = true;
                price.text = "SOLD";
            }
        }
    }

    public void AddHeavyDamage(TextMeshProUGUI price)
    {
        if (m_hDamage <= 0)
            return;
        if (!m_clicked[3])
        {
            m_dialogueText.text = m_dialogueTexts[3];
            m_dialogueText.text += UIController.Instance.GetMoney() >= m_prices[3] ? " Click again to buy." : " But you don't have enough money, beggar.";
            for (int i = 0; i < m_clicked.Length; i++)
            {
                m_clicked[i] = false;
            }
            m_clicked[3] = true;
        }
        else if (UIController.Instance.GetMoney() >= m_prices[3])
        {
            m_player.transform.GetChild(1).GetComponent<AttackListener>().IncreaseDamage();
            UIController.Instance.AddMoney(-m_prices[3]);
            m_hDamage--;
            if (m_hDamage <= 0)
            {
                m_sold[3] = true;
                price.text = "SOLD";
            }
        }
    }

    public void AddDash(TextMeshProUGUI price)
    {
        if (m_dash <= 0)
            return;
        if (!m_clicked[4])
        {
            m_dialogueText.text = m_dialogueTexts[4];
            m_dialogueText.text += UIController.Instance.GetMoney() >= m_prices[4] ? " Click again to buy." : " But you don't have enough money, beggar.";
            m_clicked[4] = true;
            for (int i = 0; i < m_clicked.Length - 1; i++)
            {
                m_clicked[i] = false;
            }
        }
        else if (UIController.Instance.GetMoney() >= m_prices[4])
        {
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
}
