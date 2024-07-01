using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAnimationStateOnExit : StateMachineBehaviour
{
    [SerializeField]
    string m_parameterName;
    [SerializeField]
    bool m_stateMachine;
    [SerializeField]
    bool m_state;

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        if(m_stateMachine)
        {
            animator.SetBool(Animator.StringToHash(m_parameterName), false);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_state)
        {
            animator.SetBool(Animator.StringToHash(m_parameterName), false);
        }
    }
}
