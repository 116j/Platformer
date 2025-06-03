using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using Zenject;

public class UIController : MonoBehaviour
{
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
    [SerializeField]
    Button m_firstShopItem;

    [Header("Win")]
    [SerializeField]
    GameObject m_winLayout;
    [SerializeField]
    TextMeshProUGUI m_winTextKB;
    [SerializeField]
    TextMeshProUGUI m_winTextG;

    [Header("Die")]
    [SerializeField]
    GameObject m_dieLayout;
    [SerializeField]
    TextMeshProUGUI m_dieTextKB;
    [SerializeField]
    TextMeshProUGUI m_dieTextG;

    List<Image> m_hearts;
    int m_currentHeart;
    readonly Vector3 m_heratSize = new Vector3(32.5f, 27);

    public int CurrentHearts => m_currentHeart + 1;
    public int AllHerats => m_hearts.Count;

    int m_money = 0;
    int m_currentMoney = 0;

    public int CurrentLanguage { get; set; }
    [Inject]
    PlayerInput m_input;
    [Inject]
    ShopLayout m_shop;

    // Start is called before the first frame update
    void Awake()
    {
        m_hearts = m_healthLayout.GetComponentsInChildren<Image>().ToList();
        m_currentHeart = m_hearts.Count - 1;

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
            x =>
            {
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
        m_firstShopItem.Select();
        m_shop.Greet();
    }

    public void Win()
    {
        m_input.EndGame();
        m_input.LockInput(true);
        m_winLayout.SetActive(true);
        m_winTextG.gameObject.SetActive(m_input.GetCurrentDeviceType() == "Gamepad");
        m_winTextKB.gameObject.SetActive(!m_winTextG.isActiveAndEnabled);
    }

    public void Die(bool active)
    {
        m_dieLayout.SetActive(active);
        m_dieTextG.gameObject.SetActive(m_input.GetCurrentDeviceType() == "Gamepad");
        m_dieTextKB.gameObject.SetActive(!m_dieTextG.isActiveAndEnabled);
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
