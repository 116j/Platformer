using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Layouts")]
    [SerializeField]
    Image m_header;
    [SerializeField]
    GameObject m_displayLayout;
    [SerializeField]
    GameObject m_audioLayout;
    [SerializeField]
    GameObject m_controlsLayout;

    [Header("Audio")]
    [SerializeField]
    AudioMixer m_mixer;
    [SerializeField]
    Sprite m_audioHeader;
    [SerializeField]
    Transform m_gameVolumeFill;
    [SerializeField]
    Transform m_musicVolumeFill;
    [SerializeField]
    Transform m_sfxVolumeFill;

    [Header("Display")]
    [SerializeField]
    Sprite m_displayHeader;
    [SerializeField]
    TextMeshProUGUI m_resolutionText;
    [SerializeField]
    TextMeshProUGUI m_languageText;
    [SerializeField]
    Transform m_brightnessFill;

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

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_resolutiions.Length; i++)
        {
            if(Screen.currentResolution.width== m_resolutiions[i].Key&&
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
        m_header.sprite = m_displayHeader;
    }

    public void SetBrightness(float value)
    {
        for (int i = 0; i <= (int)value; i++)
        {
            m_brightnessFill.GetChild(i).gameObject.SetActive(true);
        }

        for (int i = (int)value + 1; i < m_brightnessFill.childCount; i++)
        {
            m_brightnessFill.GetChild(i).gameObject.SetActive(false);
        }

        Screen.brightness = value/14f;
    }

    public void SetResolutionUp()
    {
        if(m_currentResolutionInd<m_resolutiions.Length-1)
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
        if (m_currentLanguageInd < m_languages.Length-1)
        {
            m_languageText.text = m_languages[m_currentLanguageInd++].ToString();
        }
    }
    
    public void SetLanguageDown()
    {
        if(m_currentLanguageInd > 0)
        {
            m_languageText.text = m_languages[m_currentLanguageInd--].ToString();
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
        m_header.sprite = m_audioHeader;
    }

    public void ChangeGameVolume(float value)
    {
        for (int i = 0;i<=(int)value;i++)
        {
            m_gameVolumeFill.GetChild(i).gameObject.SetActive(true);
        }

        for (int i = (int)value+1; i < m_gameVolumeFill.childCount; i++)
        {
            m_gameVolumeFill.GetChild(i).gameObject.SetActive(false);
        }
        m_mixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
    }

    public void ChangeMusicVolume(float value)
    {
        for (int i = 0; i <= (int)value; i++)
        {
            m_musicVolumeFill.GetChild(i).gameObject.SetActive(true);
        }

        for (int i = (int)value + 1; i < m_musicVolumeFill.childCount; i++)
        {
            m_musicVolumeFill.GetChild(i).gameObject.SetActive(false);
        }
        m_mixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
    }
    public void ChangeEffectsVolume(float value)
    {
        for (int i = 0; i <= (int)value; i++)
        {
            m_sfxVolumeFill.GetChild(i).gameObject.SetActive(true);
        }

        for (int i = (int)value + 1; i < m_sfxVolumeFill.childCount; i++)
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

    }
}
