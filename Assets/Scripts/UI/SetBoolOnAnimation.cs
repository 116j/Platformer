using UnityEngine;

public class SetBoolOnAnimation : StateMachineBehaviour
{
    [SerializeField]
    string m_parameterName;
    [SerializeField]
    bool m_value;
    [SerializeField]
    bool m_stateMachine;
    [SerializeField]
    bool m_state;

    [SerializeField]
    bool m_onExit;
    [SerializeField]
    bool m_onEnter;

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        if (m_stateMachine && m_onEnter)
        {
            animator.SetBool(Animator.StringToHash(m_parameterName), m_value);
        }
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_state && m_onEnter)
        {
            animator.SetBool(Animator.StringToHash(m_parameterName), m_value);
        }
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        if (m_stateMachine && m_onExit)
        {
            animator.SetBool(Animator.StringToHash(m_parameterName), m_value);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_state && m_onExit)
        {
            animator.SetBool(Animator.StringToHash(m_parameterName), m_value);
        }
    }
}
