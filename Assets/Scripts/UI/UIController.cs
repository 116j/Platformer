using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using UnityEngine.Windows;

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
    [SerializeField]
    TextMeshProUGUI m_winText;

    [Header("Die")]
    [SerializeField]
    GameObject m_dieLayout;
    [SerializeField]
    TextMeshProUGUI m_dieText;

    List<Image> m_hearts;
    int m_currentHeart;
    readonly Vector3 m_heratSize = new Vector3(32.5f, 27);

    public int CurrentHearts => m_currentHeart + 1;
    public int AllHerats => m_hearts.Count;

    int m_money = 0;
    int m_currentMoney = 0;

    public int CurrentLanguage { get; set; }
    PlayerInput m_input;

    string[][] m_restartTexts ={
        new string[]{
        "press SELECT to RESTART",
        "pressione SELECT para REINICIAR",
        "нажмите SELECT для ПЕРЕЗАПУСКА",
        "presione SELECT para REINICIAR",
        "YENİDEN başlatmak için SELECT tuşuna basın"
        },
        new string[]{
        "PRESS R TO RESTART",
        "PRESSIONE R PARA REINICIAR",
        "НАЖМИТЕ R ДЛЯ ПЕРЕЗАПУСКА",
        "PRESIONE R PARA REINICIAR",
        "YENİDEN BAŞLATMAK İÇİN R TUŞUNA BASIN"
        }
        };
    string[][] m_winTexts ={
        new string[]{
        "press SELECT to go to the MAIN MENU",
        "pressione SELECT para ir para o MENU PRINCIPAL", 
        "нажмите SELECT, чтобы перейти в ГЛАВНОЕ МЕНЮ",
        "haga clic en SELECCIONAR para ir al MENÚ PRINCIPAL",
        "ANA MENÜYE gitmek için SELECT tuşuna basın"
        },
        new string[]{
        "PRESS M TO GO TO THE MAIN MENU",
        "PRESSIONE M PARA IR PARA O MENU PRINCIPAL",
        "НАЖМИТЕ M, ЧТОБЫ ПЕРЕЙТИ В ГЛАВНОЕ МЕНЮ",
        "PRESIONE M PARA IR AL MENÚ PRINCIPAL",
        "ANA MENÜYE GİTMEK İÇİN M TUŞUNA BASIN"
        }
        };

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
        m_input = GameObject.FindWithTag("Player").GetComponent<PlayerInput>();

        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales[i];
            if (LocalizationSettings.SelectedLocale == locale)
            {
                CurrentLanguage = i;
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
    public void AddMoney(int amount)
    {
        // Рассчитываем длительность анимации (например, 0.001s на единицу, но не меньше 0.5s и не больше 2s)
        float baseDuration = Mathf.Abs(amount) * Time.deltaTime;
        float duration = Mathf.Clamp(baseDuration, 0.5f, 3f);
        m_currentMoney = m_money;
        m_money += amount;
        DOTween.To(
            () => m_currentMoney,
            x => {
                m_currentMoney = x;
                UpdateMoneyText();
            },
            m_money,
            duration
        ).SetEase(Ease.OutQuad);

    }

    void UpdateMoneyText()
    {
        m_moneyText.text = m_currentMoney.ToString();  // Можно добавить "$" или форматирование
    }


    public float GetMoney() => m_money;

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
        m_input.LockInput();
        if (m_input.GetCurrebtDeviceType() == "Gamepad")
        {
            m_dieText.text = m_winTexts[0][CurrentLanguage];
        }
        else
        {
            m_winText.text = m_winTexts[1][CurrentLanguage];
        }
        m_winLayout.SetActive(true);
    }

    public void Die(bool active)
    {
        if (m_input.GetCurrebtDeviceType() == "Gamepad")
        {
            m_dieText.text = m_restartTexts[0][CurrentLanguage];
        }
        else
        {
            m_dieText.text = m_restartTexts[1][CurrentLanguage];
        }
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
