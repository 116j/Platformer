using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    public static FloatingText Instance { get; private set; }
    [SerializeField]
    Transform m_player;
    [SerializeField]
    TextMeshProUGUI m_playerText;
    [SerializeField]
    TextMeshProUGUI m_shopText;

    bool m_showPlayerText;
    bool m_showShopText;

    Vector3 m_shop;

    private void Awake()
    {
        if(Instance == null) 
        { 
        Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_showPlayerText)
        {
            transform.position = m_player.position + Vector3.up * 2f;
        }
        else if(m_showShopText)
        {
            transform.position = m_shop + new Vector3(0.7629f, 1);
        }
    }

    public void ShowPlayerText(bool show)
    {
        m_playerText.gameObject.SetActive(show);
        m_showPlayerText = show;
    }

    public void ShowShopText(bool show, Vector3 shopLocation)
    {
        m_shopText.gameObject.SetActive(show);
        m_showPlayerText = show;
        m_shop = shopLocation;
    }
}
