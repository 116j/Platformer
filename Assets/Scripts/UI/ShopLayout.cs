using System;
using System.Diagnostics;
using System.Linq;
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
    int[] m_prices = { 1000, 1500, 2000, 2000, 2500 };
    string[] m_startText = {
        "Hello, Stranger! Welcome to my shop! What would you like to purchase?",
        "Olá, Estranho! Bem-vindo à minha loja! O que você gostaria de comprar?",
        "Привет, Путник! Добро пожаловать в мой магазин! Что бы ты хотел приобрести?",
        "¡Hola, Forastero! Bienvenido a mi tienda! ¿Qué le gustaría comprar?",
        "Merhaba Yabancı! Benim dükkana hoşgeldiniz! Ne satın almak istersiniz?"
    };
    string[][] m_dialogueTexts =
    {
        new string[]{ 
            "Adds an extra health heart. ",
            "Adiciona saúde do coração extra. ",
            "Добавляет дополнительное сердце здоровья. ",
            "Agrega salud extra al corazón. ",
            "Ekstra bir sağlık kalbi ekler. "
        },
        new string[]{ 
            "Decreases the dash cooldown time. ",
            "Reduz o tempo de recarga do puxão. ",
            "Уменьшает время перезарядки рывка. ",
            "Reduce el tiempo de recarga del tirón. ",
            "Sarsıntının yeniden yükleme süresini azaltır. "
        },
        new string[]{ 
            "Increases the light attack's damage. ",
            "Aumenta o dano de ataque leve. ",
            "Увеличивает урон легкой атаки. ",
            "Aumenta el daño de ataque ligero. ",
            "Hafif saldırı hasarını artırır. "
        },
        new string[]{ 
            "Increases the heavy attack's damage. ",
            "Aumenta o dano de ataque pesado. ",
            "Увеличивает урон тяжелой атаки. ",
            "Aumenta el daño de ataque pesado. ",
            "Ağır saldırı hasarını artırır. "
        },
        new string[]{ 
            "Adds an extra jump. You will be able to make a triple jump. ",
            "Adiciona um salto extra. Você será capaz de fazer um salto triplo. ",
            "Adds an extra jump. You will be able to make a triple jump. ",
            "Agrega un salto extra. Podrás hacer un triple salto. ",
            "Ekstra bir sıçrama ekler. Sen üçlü bir sıçrama yapmak mümkün olacak. "
        }
    };

    string[] m_canBuyText =
    {
        "Click again to buy.",
        "Clique novamente para comprar.",
        "Нажмите еще раз, чтобы купить.",
        "Haga clic de nuevo para comprar.",
        "Satın almak için tekrar tıklayın."
    };

    string[] m_cantBuyText =
    {
        "But you don't have enough money, beggar.",
        "Mas não tens dinheiro suficiente, mendigo.",
        "Но у тебя недостаточно денег, нищий.",
        "Pero no tienes suficiente dinero, mendigo.",
        "Ama yeterli paran yok dilenci."
    };

    public int AllPrices { get; private set; }


    public float GetLowestPrice()
    {
        if (m_prices.Length == 0)
        {
            m_prices = new int[] { 1000, 1500, 2000, 2000, 2500 };
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

        AllPrices = m_prices.Sum();
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_buySound = GetComponent<AudioSource>();
    }

    void Buy(int index, TextMeshProUGUI price, Action func)
    {
        if (m_itemsCount[index] <= 0)
            return;
        if (!m_clicked[index])
        {
            m_dialogueText.text = m_dialogueTexts[index][UIController.Instance.CurrentLanguage];
            m_dialogueText.text += UIController.Instance.GetMoney() >= m_prices[index] ? m_canBuyText[UIController.Instance.CurrentLanguage] : m_cantBuyText[UIController.Instance.CurrentLanguage];
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
            m_dialogueText.text = m_startText[UIController.Instance.CurrentLanguage];
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
