using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Health")]
    [SerializeField]
    Image[] m_hearts;
    [SerializeField]
    Sprite m_fullHeart;
    [SerializeField]
    Sprite m_emptyHeart;

    int m_currentHeart;
    // Start is called before the first frame update
    void Start()
    {
        m_currentHeart = m_hearts.Length - 1;


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RemoveHeart(int damage)
    {
        for (int i = 0; i<damage; i++)
        {
            if (m_currentHeart >= 0)
            {
                m_hearts[m_currentHeart].sprite = m_emptyHeart;
                m_currentHeart--;
            }
        }
    }

    public void RecoverHeart(int healPoints)
    {
        for (int i = 0; i < healPoints; i++)
        {
            if (m_currentHeart != m_hearts.Length - 1)
            {
                m_currentHeart++;
                m_hearts[m_currentHeart].sprite = m_fullHeart;

            }
        }
    }
}
