using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class Menu : MonoBehaviour
{
    [SerializeField]
    Button m_playButton;
    [SerializeField]
    Button m_resumeButton;
    [SerializeField]
    Button m_displayButton;
    [SerializeField]
    GameObject m_maimMenuTitle;
    [SerializeField]
    AudioMixer m_mixer;

    [Inject]
    PlayerInput m_input;
    [Inject]
    UIController m_UI;

    Animator m_pauseLayoutAnim;
    Image m_backgroundTint;
    GameObject m_selected;
    float m_sfxVolume;

    // Start is called before the first frame update
    void Awake()
    {
        m_pauseLayoutAnim = GetComponent<Animator>();
        m_backgroundTint = GetComponent<Image>();

        m_mixer.GetFloat("SFXVolume", out m_sfxVolume);
        m_mixer.SetFloat("SFXVolume", -80f);
    }

    public void Pause(bool show)
    {
        m_backgroundTint.enabled = show;
        if (show)
        {
            m_mixer.GetFloat("SFXVolume", out m_sfxVolume);
            m_mixer.SetFloat("SFXVolume", -80f);
            m_maimMenuTitle.SetActive(false);
            m_pauseLayoutAnim.SetBool("OpenOptions", true);
        }
        else
        {
            m_mixer.SetFloat("SFXVolume", m_sfxVolume);

            m_pauseLayoutAnim.SetBool("Close", true);
        }

    }

    public void Resume()
    {
        m_input.OnPause();
    }

    public void MainMenu()
    {
        m_mixer.SetFloat("SFXVolume", m_sfxVolume);
        Time.timeScale = 1;
        m_UI.SetStats(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Settings()
    {
        m_pauseLayoutAnim.ResetTrigger("CloseLayout");
        m_pauseLayoutAnim.SetBool("OpenSettings", true);
    }

    public void Back()
    {
        m_pauseLayoutAnim?.SetTrigger("CloseSettings");
    }

    public void SettingsLayout()
    {
        m_pauseLayoutAnim.ResetTrigger("CloseLayout");
        m_pauseLayoutAnim.SetTrigger("OpenSettingsLayout");
    }


    public void CloseLayout(InputAction.CallbackContext ctx)
    {
        if(m_pauseLayoutAnim)
        m_pauseLayoutAnim.SetTrigger("CloseLayout");
    }

    public void Play()
    {
        m_mixer.SetFloat("SFXVolume", m_sfxVolume);
        m_input.StartGame();
        m_input.LockInput(false);
        m_pauseLayoutAnim.SetBool("Close", true);
        m_UI.SetStats(true);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
				Application.Quit();
#endif
    }

    public void SelectPlay()
    {
        m_playButton.Select();
    }

    public void SelectDisplay()
    {
        m_displayButton.Select();
    }

    public void SelectResume()
    {
        m_resumeButton.Select();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            EventSystem.current.SetSelectedGameObject(m_selected);
        }
        else
        {
            m_selected = EventSystem.current.currentSelectedGameObject;
        }
    }
}
