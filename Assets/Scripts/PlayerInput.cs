using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    Vector2 m_move;
    bool m_attack;
    bool m_heavyAttack;
    bool m_jump;
    bool m_dash;

    public Vector2 Move => m_inputLocked ? Vector2.zero : m_move;
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
    public bool HeavyAttack => !m_inputLocked && m_heavyAttack;

    bool m_inputLocked = false;

    public void LockInput()
    {
        m_inputLocked = !m_inputLocked;
    }

    void OnMove(InputValue value)
    {
        m_move = value.Get<Vector2>();
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
}
