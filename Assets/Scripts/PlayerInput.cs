using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

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

    bool m_inputLocked = false;
    bool m_restart = false;
    bool m_shop = false;
    bool m_pause = false;

    UnityEngine.InputSystem.PlayerInput m_input;

    private void Start()
    {
        Cursor.visible = true;
        m_input = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        m_input.uiInputModule.cancel.action.performed += Menu.Instance.CloseLayout;
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
        Menu.Instance.Pause(m_pause);
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
            UIController.Instance.OpenShop();
        }
    }

    void OnRestart()
    {
        if (m_restart)
        {
            LevelBuilder.Instance.Restart();
            UIController.Instance.Die(false);
            m_restart = false;
        }
    }
}
