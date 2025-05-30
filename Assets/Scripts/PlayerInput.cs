using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using Zenject;

public class PlayerInput : MonoBehaviour
{
    Vector2 m_move;
    Vector2 m_moveCamera;
    bool m_attack;
    bool m_heavyAttack;
    bool m_jump;
    bool m_dash;
    bool m_dodge;
    bool m_pet;
    bool m_restart;
    bool m_shop;
    bool m_pause;

    public Vector2 Move => m_inputLocked ? Vector2.zero : m_move;
    public Vector2 MoveCamera => m_inputLocked ? Vector2.zero : m_moveCamera;
    public bool Jump => !m_inputLocked && m_jump;
    public bool Dash
    {
        get
        {
            if (m_dash)
            {
                m_dash = false;
                return !m_inputLocked && !m_dash;
            }
            else
                return false;
        }
    }
    public bool Dodge
    {
        get
        {
            if (m_dodge)
            {
                m_dodge = false;
                return !m_inputLocked && !m_dodge;
            }
            else
                return false;
        }
    }
    public bool Attack
    {
        get
        {
            if (m_attack)
            {
                m_attack = false;
                return !m_inputLocked && !m_attack;
            }
            else
                return false;
        }
    }
    public bool Pet
    {
        get
        {
            if (m_pet)
            {
                m_pet = false;
                return !m_inputLocked && !m_pet;
            }
            else
                return false;
        }
    }

    public bool HeavyAttack => !m_inputLocked && m_heavyAttack;

    bool m_inputLocked = true;

    UnityEngine.InputSystem.PlayerInput m_input;

    [Inject]
    LevelBuilder m_lvlBuilder;
    [Inject]
    UIController m_UI;
    [Inject]
    Menu m_menu;

    private void Awake()
    {
        Cursor.visible = true;
        m_input = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        m_input.uiInputModule.cancel.action.performed += m_menu.CloseLayout;
    }

    public void LockInput()
    {
        m_inputLocked = !m_inputLocked;
    }

    public string GetCurrebtDeviceType() => m_input.currentControlScheme;

    public void EnableShop()
    {
        m_shop = !m_shop;
    }

    public void EnableRestart()
    {
        m_restart = !m_restart;
    }

    public void OnPause()
    {
        m_pause = !m_pause;
        Time.timeScale = m_pause ? 0 : 1;
        LockInput();
        m_menu.Pause(m_pause);
    }

    void OnMove(InputValue value)
    {
        m_move = value.Get<Vector2>();
    }

    void OnMoveCamera(InputValue value)
    {
        m_moveCamera = value.Get<Vector2>();
    }

    void OnAttack(InputValue value)
    {
        m_attack = value.isPressed;
    }

    void OnHeavyAttack(InputValue value)
    {
        m_heavyAttack = value.isPressed;
    }

    void OnJump(InputValue value)
    {
        m_jump = value.isPressed;
    }

    void OnDash(InputValue value)
    {
        m_dash = value.isPressed;
    }

    void OnDodge(InputValue value)
    {
        m_dodge = value.isPressed;
    }

    void OnPet(InputValue value)
    {
        m_pet = value.isPressed;
    }

    void OnShop()
    {
        if (m_shop)
        {
            LockInput();
            m_UI.OpenShop();
        }
    }

    void OnRestart()
    {
        if (m_restart)
        {
            m_lvlBuilder.Restart();
            m_UI.Die(false);
            m_restart = false;
        }
    }

    void OnMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
