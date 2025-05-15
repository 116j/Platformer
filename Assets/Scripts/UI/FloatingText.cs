using TMPro;
using UnityEngine;

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

    string[][] m_shopTexts =
    {
        new string[]
        {
            "PRESS SELECT TO OPEN THE SHOP",
            "PRESSIONE SELECT PARA ABRIR A LOJA",
            "НАЖМИТЕ SELECT, ЧТОБЫ ОТКРЫТЬ МАГАЗИН",
            "PRESIONE SELECT PARA ABRIR LA TIENDA",
            "DÜKKANI AÇMAK İÇİN SELECT TUŞUNA BASIN"
        },
       new string[]
        {
            "PRESS S TO OPEN THE SHOP",
            "PRESSIONE S PARA ABRIR A LOJA",
            "НАЖМИТЕ S, ЧТОБЫ ОТКРЫТЬ МАГАЗИН",
            "PRESIONE S PARA ABRIR LA TIENDA",
            "DÜKKANI AÇMAK İÇİN S TUŞUNA BASIN"
        }
    };

    string[][] m_petTexts =
    {
        new string[]
        {
            "PRESS B TO PET",
            "PRESSIONE B PARA PET",
            "НАЖМИТЕ B, ЧТОБЫ ПОГЛАДИТЬ",
            "PRESIONE B PARA ACARICIAR",
            "EVCİL HAYVAN İÇİN B TUŞUNA BASIN"
        },
        new string[]
        {
            "PRESS ENTER TO PET",
            "PRESSIONE ENTER PARA PET",
            "НАЖМИТЕ ENTER, ЧТОБЫ ПОГЛАДИТЬ",
            "PRESIONE ENTER PARA ACARICIAR",
            "EVCİL HAYVAN İÇİN ENTER TUŞUNA BASIN"
        }
    };

    Vector3 m_shop;

    PlayerInput m_input;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        m_input = GameObject.FindWithTag("Player").GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_showPlayerText)
        {
            transform.position = m_player.position + Vector3.up * 2f;
        }
        else if (m_showShopText)
        {
            transform.position = m_shop + new Vector3(0.7629f, 1);
        }
    }

    public void ShowPlayerText(bool show)
    {
        if (show)
        {
            if (m_input.GetCurrebtDeviceType() == "Gamepad")
            {
                m_playerText.text = m_petTexts[0][UIController.Instance.CurrentLanguage];
            }
            else
            {
                m_playerText.text = m_petTexts[1][UIController.Instance.CurrentLanguage];
            }
        }
        m_playerText.gameObject.SetActive(show);
        m_showPlayerText = show;
    }

    public void ShowShopText(bool show, Vector3 shopLocation)
    {
        if (show)
        {
            if (m_input.GetCurrebtDeviceType() == "Gamepad")
            {
                m_playerText.text = m_shopTexts[0][UIController.Instance.CurrentLanguage];
            }
            else
            {
                m_playerText.text = m_shopTexts[1][UIController.Instance.CurrentLanguage];
            }
        }
        m_shopText.gameObject.SetActive(show);
        m_showPlayerText = show;
        m_shop = shopLocation;
    }
}
