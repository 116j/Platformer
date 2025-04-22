using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    static Menu m_instance;
    public static Menu Instance => m_instance;

    [SerializeField]
    Button m_playButton;
    [SerializeField]
    Button m_resumeButton;
    [SerializeField]
    Button m_displayButton;

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
        m_pauseLayoutAnim.SetBool("OpenSettings", true);
    }

    public void Options()
    {
        m_pauseLayoutAnim.SetBool("OpenOptions", true);
    }

    public void SettingsLayout()
    {
        m_pauseLayoutAnim.SetTrigger("OpenSettingsLayout");
    }


    public void CloseLayout(InputAction.CallbackContext ctx)
    {
        m_pauseLayoutAnim.SetTrigger("CloseLayout");
    }

    public void Play()
    {
        m_input.LockInput();
        UIController.Instance.SetStats(true);
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
}
