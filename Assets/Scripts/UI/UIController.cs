using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    static UIController m_instance;
    public static UIController Instance => m_instance;

    [Header("Health")]
    [SerializeField]
    GameObject m_healthLayout;
    [SerializeField]
    Sprite m_fullHeart;
    [SerializeField]
    Sprite m_emptyHeart;

    [Header("Dash")]
    [SerializeField]
    Image m_dashBar;
    [SerializeField]
    Sprite[] m_dashSprites;

    [Header("Money")]
    [SerializeField]
    GameObject m_moneyLayout;
    [SerializeField]
    TextMeshProUGUI m_moneyText;

    [Header("Shop")]
    [SerializeField]
    GameObject m_shopLayout;

    [Header("Win")]
    [SerializeField]
    GameObject m_winLayout;

    [Header("Die")]
    [SerializeField]
    GameObject m_dieLayout;

    List<Image> m_hearts;
    int m_currentHeart;
    readonly Vector3 m_heratSize = new Vector3(32.5f, 27);

    public int CurrentHearts => m_currentHeart + 1;
    public int AllHerats => m_hearts.Count;

    int m_money = 0;
    float m_addAmount = 0;
    float m_amount = 0;

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        m_hearts = m_healthLayout.GetComponentsInChildren<Image>().ToList();
        m_currentHeart = m_hearts.Count - 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_amount != 0)
        {
            m_addAmount = Mathf.Lerp(m_addAmount, m_amount, Time.deltaTime);
            m_moneyText.text = (m_money + Mathf.CeilToInt(m_addAmount)).ToString();
            if (Mathf.CeilToInt(m_addAmount) == m_amount)
            {
                m_addAmount = 0;
                m_money += (int)m_amount;
                m_amount = 0;
            }
        }
    }

    public void AddHeart()
    {
        GameObject heart = new GameObject();
        Image image = heart.AddComponent<Image>();
        if (m_currentHeart < m_hearts.Count - 1)
        {
            image.sprite = m_emptyHeart;
        }
        else
        {
            image.sprite = m_fullHeart;
        }
        heart.transform.SetParent(m_healthLayout.transform, false);
        image.rectTransform.sizeDelta = m_heratSize;
        m_hearts.Add(image);
    }
    public void AddMoney(float amount)
    {
        m_amount += amount;
    }

    public float GetMoney() => m_money + m_amount;

    /// <summary>
    /// Add or remove hearts
    /// </summary>
    /// <param name="damage">damage points</param>
    public void ChangeHearts(int damage)
    {
        for (int i = 0; i < Mathf.Abs(damage); i++)
        {
            if (m_currentHeart < m_hearts.Count - 1 && damage > 0)
            {
                m_currentHeart++;
                m_hearts[m_currentHeart].sprite = m_fullHeart;
            }
            else if (damage < 0 && m_currentHeart >= 0)
            {
                m_hearts[m_currentHeart].sprite = m_emptyHeart;
                m_currentHeart--;
            }
        }
    }
    /// <summary>
    /// Dash sprite fill
    /// </summary>
    /// <param name="fill">fill amount</param>
    public void SetDashSprite(float fill)
    {
        m_dashBar.sprite = m_dashSprites[Mathf.FloorToInt(fill * (m_dashSprites.Length - 1))];
    }

    public void SetStats(bool set)
    {
        m_healthLayout.SetActive(set);
        m_dashBar.gameObject.SetActive(set);
        m_moneyLayout.SetActive(set);
    }

    public void OpenShop()
    {
        m_shopLayout.SetActive(!m_shopLayout.activeInHierarchy);
    }

    public void Win()
    {
        m_winLayout.SetActive(true);
    }

    public void Die(bool active)
    {
        m_dieLayout.SetActive(active);
    }

    private void OnApplicationPause(bool pause)
    {
        AudioListener.pause = pause;
    }

    private void OnApplicationFocus(bool focus)
    {
        AudioListener.pause = !focus;
    }
}
