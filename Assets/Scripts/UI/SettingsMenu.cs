using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    static SettingsMenu m_instance;
    public static SettingsMenu Instance => m_instance;

    [Header("Layouts")]
    [SerializeField]
    TextMeshProUGUI m_header;
    [SerializeField]
    RectTransform m_layout;

    [Header("Audio")]
    [SerializeField]
    AudioMixer m_mixer;
    [SerializeField]
    Transform m_gameVolumeFill;
    [SerializeField]
    Slider m_gameVolumeSlider;
    [SerializeField]
    Transform m_musicVolumeFill;
    [SerializeField]
    Slider m_musicVolumeSlider;
    [SerializeField]
    Transform m_sfxVolumeFill;
    [SerializeField]
    Slider m_sfxVolumeSlider;

    [Header("Display")]
    [SerializeField]
    TextMeshProUGUI m_resolutionText;
    [SerializeField]
    TextMeshProUGUI m_languageText;

    [Header("Controls")]
    [SerializeField]
    GameObject m_gamepadContent;
    [SerializeField]
    GameObject m_keyboardContent;

    [Header("Level Builder")]
    [SerializeField]
    Transform m_roomsCountFill;
    [SerializeField]
    Slider m_roomsCountSlider;
    [SerializeField]
    TextMeshProUGUI m_roomsCountText;
    [SerializeField]
    TextMeshProUGUI m_baseRoomWeight;
    [SerializeField]
    TextMeshProUGUI m_ceilRoomWeight;
    [SerializeField]
    TextMeshProUGUI m_gridRoomWeight;
    [SerializeField]
    TextMeshProUGUI m_movPlatfRoomWeight;


    KeyValuePair<int, int>[] m_resolutiions = {
        new(640,480), new(800,600), new(1280,800),
       new(1280,720), new(1440,900),new(1920,1200),
        new(1920,1080),new(2560,1440)
    };
    SystemLanguage[] m_languages = { };

    int m_currentLanguageInd = 0;
    int m_currentResolutionInd;

    SystemLanguage m_currentLanguage;
    KeyValuePair<int, int> m_currentResolution;
    bool m_fullScreen = true;

    int m_roomsCount = 50;
    float[] m_strategyWeights = { 0.6f, 0.15f, 0.3f, 0.15f };

    PlayerInput m_input;

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
    }

    void Start()
    {
        m_input = GameObject.FindWithTag("Player").GetComponent<PlayerInput>();
        for (int i = 0; i < m_resolutiions.Length; i++)
        {
            if (Screen.currentResolution.width == m_resolutiions[i].Key &&
                Screen.currentResolution.height == m_resolutiions[i].Value)
            {
                m_currentResolutionInd = i;
                m_currentResolution = m_resolutiions[i];
                m_resolutionText.text = $"{m_resolutiions[m_currentResolutionInd].Key} x {m_resolutiions[m_currentResolutionInd].Value}";
            }
        }
    }

    public void Display()
    {
        m_header.text = "DISPLAY";
        m_layout.sizeDelta = new Vector2(m_layout.sizeDelta.x, 270);
    }

    public void SetResolutionUp()
    {
        if (m_currentResolutionInd < m_resolutiions.Length - 1)
        {
            m_currentResolutionInd++;
            m_resolutionText.text = $"{m_resolutiions[m_currentResolutionInd].Key} x {m_resolutiions[m_currentResolutionInd].Value}";
        }
    }

    public void SetResolutionDown()
    {
        if (m_currentResolutionInd > 0)
        {
            m_currentResolutionInd--;
            m_resolutionText.text = $"{m_resolutiions[m_currentResolutionInd].Key} x {m_resolutiions[m_currentResolutionInd].Value}";
        }
    }

    public void FullScreen(bool full)
    {
        m_fullScreen = full;
    }

    public void SetLanguageUp()
    {
        if (m_currentLanguageInd < m_languages.Length - 1)
        {
            m_languageText.text = m_languages[++m_currentLanguageInd].ToString();
        }
    }

    public void SetLanguageDown()
    {
        if (m_currentLanguageInd > 0)
        {
            m_languageText.text = m_languages[--m_currentLanguageInd].ToString();
        }
    }

    public void SaveDisplay()
    {
        m_currentLanguage = m_languages[m_currentLanguageInd];
        m_currentResolution = m_resolutiions[m_currentResolutionInd];
        Screen.SetResolution(m_currentResolution.Key, m_currentResolution.Key, m_fullScreen);
    }

    public void Audio()
    {
        m_header.text = "AUDIO";
        m_layout.sizeDelta = new Vector2(m_layout.sizeDelta.x, 300);
    }

    public void ChangeGameVolume(float value)
    {
        int num = Mathf.RoundToInt(value / m_gameVolumeSlider.maxValue * m_gameVolumeFill.childCount) - 1;
        for (int i = 0; i <= num; i++)
        {
            m_gameVolumeFill.GetChild(i).gameObject.SetActive(true);
        }

        for (int i = num + 1; i < m_gameVolumeFill.childCount; i++)
        {
            m_gameVolumeFill.GetChild(i).gameObject.SetActive(false);
        }
        m_mixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
    }

    public void ChangeMusicVolume(float value)
    {
        int num = Mathf.RoundToInt(value / m_musicVolumeSlider.maxValue * m_musicVolumeFill.childCount) - 1;
        for (int i = 0; i <= num; i++)
        {
            m_musicVolumeFill.GetChild(i).gameObject.SetActive(true);
        }

        for (int i = num + 1; i < m_musicVolumeFill.childCount; i++)
        {
            m_musicVolumeFill.GetChild(i).gameObject.SetActive(false);
        }
        m_mixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
    }
    public void ChangeEffectsVolume(float value)
    {
        int num = Mathf.RoundToInt(value / m_sfxVolumeSlider.maxValue * m_sfxVolumeFill.childCount) - 1;
        for (int i = 0; i <= num; i++)
        {
            m_sfxVolumeFill.GetChild(i).gameObject.SetActive(true);
        }

        for (int i = num + 1; i < m_sfxVolumeFill.childCount; i++)
        {
            m_sfxVolumeFill.GetChild(i).gameObject.SetActive(false);
        }
        m_mixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
    }

    public void Mute(bool mute)
    {
        AudioListener.volume = mute ? 0 : 1;
    }

    public void Controls()
    {
        m_header.text = "CONTROLS";
        m_layout.sizeDelta = new Vector2(m_layout.sizeDelta.x, 375);
        if (m_input.GetCurrebtDeviceType() == "Gamepad")
        {
            m_gamepadContent.SetActive(true);
            m_keyboardContent.SetActive(false);
        }
        else
        {
            m_gamepadContent.SetActive(false);
            m_keyboardContent.SetActive(true);
        }
    }

    public void LvlBuilder()
    {
        m_header.text = "LEVEL BUILDER";
        m_layout.sizeDelta = new Vector2(m_layout.sizeDelta.x, 410);
    }

    public void ChangeRoomsCount(float value)
    {
        m_roomsCount = (int)value;
        m_roomsCountText.text = value.ToString();

        int num = Mathf.RoundToInt(value / m_roomsCountSlider.maxValue * m_roomsCountFill.childCount) - 1;
        for (int i = 0; i <= num; i++)
        {
            m_roomsCountFill.GetChild(i).gameObject.SetActive(true);
        }

        for (int i = num + 1; i < m_roomsCountFill.childCount; i++)
        {
            m_roomsCountFill.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void ChangeBaseRoomWeight(float value)
    {
        m_baseRoomWeight.text = value.ToString("F3", CultureInfo.InvariantCulture);
        m_strategyWeights[0] = value;
    }

    public void ChangeCeilRoomWeight(float value)
    {
        m_ceilRoomWeight.text = value.ToString("F3", CultureInfo.InvariantCulture);
        m_strategyWeights[1] = value;
    }

    public void ChangeGridRoomWeight(float value)
    {
        m_gridRoomWeight.text = value.ToString("F3", CultureInfo.InvariantCulture);
        m_strategyWeights[2] = value;
    }

    public void ChangeMovingPlatformRoomWeight(float value)
    {
        m_movPlatfRoomWeight.text = value.ToString("F3", CultureInfo.InvariantCulture);
        m_strategyWeights[3] = value;
    }


    public void SaveLevelBuilder()
    {
        for (int i = 0; i < m_strategyWeights.Length; i++)
        {
            LevelBuilder.Instance.ChangeStrategyWeight(i, m_strategyWeights[i]);
        }

        LevelBuilder.Instance.ChangeMaxRoomsCount(m_roomsCount);
    }
}
