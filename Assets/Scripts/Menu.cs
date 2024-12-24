using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    static Menu m_instance;
    public static Menu Instance => m_instance;

    [Header("Pause")]
    [SerializeField]
    GameObject m_pauseLayout;
    [SerializeField]
    GameObject m_settingsLayout;

    PlayerInput m_input;

    Animator m_pauseLayoutAnim;

    Image m_backgroundTint;

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
        m_input = GameObject.FindWithTag("Player").GetComponent<PlayerInput>();
        m_pauseLayoutAnim = GetComponent<Animator>();
        m_backgroundTint = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Pause(bool show)
    {
        m_backgroundTint.enabled = show;
        if (show)
        {
            m_pauseLayoutAnim.SetBool("OpenOptions", true);
        }
        else
        {
            m_pauseLayoutAnim.SetBool("Close", true);
        }
    }

    public void Resume()
    {
        m_input.OnPause();
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        m_input.LockInput();
        UIController.Instance.SetStats(false);
        SceneManager.LoadScene(0);
    }

    public void Settings()
    {
        m_pauseLayoutAnim.SetBool("OpenSettings",true);
    }

    public void Options()
    {
        m_pauseLayoutAnim.SetBool("OpenOptions", true);
    }

    public void Audio()
    {
        m_pauseLayoutAnim.SetTrigger("OpenSettingsLayout");
    }

    public void Display()
    {
        m_pauseLayoutAnim.SetTrigger("OpenSettingsLayout");
    }

    public void Play()
    {
        m_input.LockInput();
        UIController.Instance.SetStats(true);
    }

}
