using System;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    static UIController m_instance;
    public static UIController Instance => m_instance;

    [Header("Health")]
    [SerializeField]
    Image[] m_hearts;
    [SerializeField]
    Sprite m_fullHeart;
    [SerializeField]
    Sprite m_emptyHeart;

    [Header("Dash")]
    [SerializeField]
    Image m_dashBar;
    [SerializeField]
    Sprite[] m_dashSprites;

    int m_currentHeart;

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
        m_currentHeart = m_hearts.Length - 1;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Add or remove hearts
    /// </summary>
    /// <param name="damage">damage points</param>
    public void ChangeHearts(int damage)
    {
        for (int i = 0; i < Math.Abs(damage); i++)
        {
            if (m_currentHeart < m_hearts.Length - 1 && damage > 0)
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
}
